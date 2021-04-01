using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BwInf_39_2_2_Spießgesellen {
    class Algorithmus {
        /// <summary>
        /// berechnet Wunschspieß.schüsseln
        /// returns Wunschspieß und gesplitete spieße
        /// </summary>
        /// <param name="wunschSpieß">gewünschte Obstsorten</param>
        /// <param name="spieße">Liste mit beobachteten Spießen</param>
        /// <param name="gesamtObst">anzahl aller Obstsorten</param>
        /// <returns></returns>
        public static (Spieß wunschSpieß, List<Spieß> spieße) algorithmus1(Spieß wunschSpieß, List<Spieß> spieße, int gesamtObst) {
            spieße = spießeAufspalten(spieße);

            spieße = unbeobachteteObstsortenFinden(spieße, wunschSpieß, gesamtObst);

            (Spieß wunschSpießNew, List<(Spieß spieß, List<string> unpassendeSorten)> spießeHalbfalsch) = wunschspießZusammensetzen(spieße, wunschSpieß);
            wunschSpieß = wunschSpießNew;

            printResult(spieße, wunschSpieß, spießeHalbfalsch);

            return (wunschSpieß, spieße);
        }


        /// <summary>
        /// berechnet Wunschspieß.schüsseln
        /// returns Wunschspieß und gesplitete spieße
        /// </summary>
        /// <param name="wunschSpieß">gewünschte Obstsorten</param>
        /// <param name="spieße">Liste mit beobachteten Spießen</param>
        /// <param name="gesamtObst">anzahl aller Obstsorten</param>
        /// <returns></returns>
        public static (Spieß wunschSpieß, List<Spieß> spieße) algorithmus2(Spieß wunschSpieß, List<Spieß> spieße, int gesamtObst) {
            spieße = spießeAufspalten2(spieße,gesamtObst);

            spieße = unbeobachteteObstsortenFinden(spieße, wunschSpieß, gesamtObst);

            (Spieß wunschSpießNew, List<(Spieß spieß, List<string> unpassendeSorten)> spießeHalbfalsch) = wunschspießZusammensetzen(spieße, wunschSpieß);
            wunschSpieß = wunschSpießNew;

            printResult(spieße, wunschSpieß, spießeHalbfalsch);

            return (wunschSpieß, spieße);
        }


        /// <summary>
        /// Ausgabe der teilweise passenden Spieße in Kurzform.
        /// Bsp.: *rot*Banane Ugli  -> 25 18
        /// </summary>
        /// <param name="spießeHalbfalsch">Liste der teilweise richtigen Spieße mit List<string> der ungewünschten Obstsorten</param>
        public static void printHalbfalschKurz(List<(Spieß spieß, List<string> unpassendeSorten)> spießeHalbfalsch) {
            //spießeHalbfalsch.Sort((tup1, tup2) => ((tup1.Item1.length-tup1.Item2.Count) / tup1.Item1.length).CompareTo((tup2.Item1.length - tup2.Item2.Count) / tup2.Item1.length));//elemente mit einem besseren gewünschteSortenImSpieß zu Spießlänge Verhältnis weiter oben
            Console.WriteLine();
            for (int i = 0; i < spießeHalbfalsch.Count; i++) {
                foreach (string obst in spießeHalbfalsch[i].spieß.obstSorten) {
                    Console.ForegroundColor = spießeHalbfalsch[i].unpassendeSorten.Contains(obst) ? ConsoleColor.DarkRed : ConsoleColor.White;
                    Console.Write(obst + " ");
                }
                Console.Write(" -> ");
                foreach (int schüssel in spießeHalbfalsch[i].spieß.schüsseln) { Console.Write(schüssel + " "); }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Ausgabe der teilweise passenden Spieße in ausformulierter Form.
        /// Bsp.: "Ugli" ist in den Schüsseln 25, 18; in einer der Schüsseln ist die nicht-gewünschte Sorte "Banane" enthalten
        /// </summary>
        /// <param name="spießeHalbfalsch">Liste der teilweise richtigen Spieße mit List<string> der ungewünschten Obstsorten</string></param>
        public static void printHalbfalschWortform(List<(Spieß spieß, List<string> unpassendeSorten)> spießeHalbfalsch) {
            Console.Write("\nfolgende gewünschte Obssorten konnten nicht zugeordnet werden: ");
            string output = "\"";
            for (int i = 0; i < spießeHalbfalsch.Count; i++) {
                for (int j = 0; j < spießeHalbfalsch[i].spieß.obstSorten.Count; j++) {
                    if (!spießeHalbfalsch[i].unpassendeSorten.Contains(spießeHalbfalsch[i].spieß.obstSorten[j]))
                        output += spießeHalbfalsch[i].spieß.obstSorten[j] + "\", \"";
                }
            }
            output = output.Remove(output.Length - 3, 3);
            output += "\n\n";
            for (int i = 0; i < spießeHalbfalsch.Count; i++) {
                foreach (string sorte in spießeHalbfalsch[i].spieß.obstSorten) {
                    if (!spießeHalbfalsch[i].unpassendeSorten.Contains(sorte)) {
                        output += "\"" + sorte + "\", ";
                    }
                }
                output = output.Remove(output.Length - 2, 2);
                output += (spießeHalbfalsch[i].spieß.obstSorten.Count - spießeHalbfalsch[i].unpassendeSorten.Count == 1 ? " ist" : " sind") + " in den Schüsseln ";
                output += string.Join(", ", spießeHalbfalsch[i].spieß.schüsseln);
                output += spießeHalbfalsch[i].unpassendeSorten.Count == 1 ? "; in einer der Schüsseln ist die nicht-gewünschte Sorte \"" : "; in mehreren der Schüsseln sind die nicht-gewünschten Sorten \"";
                output += string.Join("\", ", spießeHalbfalsch[i].unpassendeSorten);
                output += "\" enthalten \n";
            }
            Console.WriteLine(output);
        }

        static void printResult(List<Spieß> spieße, Spieß wunschSpieß, List<(Spieß spieß, List<string> unpassendeSorten)> spießeHalbfalsch) {
            Console.WriteLine("\nAUFGESPLITETE SPIESSE:");
            foreach (Spieß spieß in spieße) {
                spieß.printSpieß();
            }
            Console.WriteLine("\nWUNSCHSPIESS:");
            wunschSpieß.printSpieß();

            Console.WriteLine("\nTEILWEISE PASSENDE:");
            if (spießeHalbfalsch.Count > 0) {
                printHalbfalschKurz(spießeHalbfalsch);
            }
        }

        // O(n)=x*spieße^2
        static List<Spieß> spießeAufspalten(List<Spieß> spieße) {
            bool didChange = true;
            int counter = 0;
            while (didChange) {
                didChange = false;
                for (int i = 0; i < spieße.Count; i++) {
                    for (int j = i; j < spieße.Count; j++) {
                        if (i != j) {
                            (Spieß spieß2neu, Spieß schnittSpieß) = spieße[i].vergleicheSpieße(spieße[j]);
                            if (schnittSpieß.length > 0) {
                                spieße[j] = spieß2neu;
                                spieße.Add(schnittSpieß);
                                didChange = true;
                            }
                        }
                    }
                }
                counter++;
            }
            Console.WriteLine(counter);
            spieße.RemoveAll(sp => sp.length == 0);//laufzeitoptimierung?
            return spieße;
        }

        //O(n)= spieße^2 + gesamtObst^3 + spieße+schüsseln*sorten
        static List<Spieß> spießeAufspalten2(List<Spieß> spieße, int gesamtObst) {
            Dictionary<char, int> alphabet = new Dictionary<char, int>();
            char[] alphabetAllg = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            for (int i = 0; i < alphabetAllg.Length; i++) { alphabet.Add(alphabetAllg[i], i); }

            int[,] überschneidungsTabelle = new int[gesamtObst, gesamtObst];
            foreach (Spieß sp in spieße) {
                foreach (int schüssel in sp.schüsseln) {
                    foreach (string sorte in sp.obstSorten) {
                        überschneidungsTabelle[alphabet[sorte.ToLower()[0]], schüssel - 1]++;
                    }
                }
            }
            //TODO: aufräumen (kommentieren, varnamen (i,j) )
            //zusammenrechnen der Reihen und Spalten der Überschneidungstabelle
            List<int>[] spalten = new List<int>[gesamtObst];
            List<int>[] reihen = new List<int>[gesamtObst];
            for (int r = 0; r < gesamtObst; r++) { spalten[r] = new List<int>(); reihen[r] = new List<int>(); }
            for (int i = 0; i < gesamtObst; i++) {
                int highestVal = -1;
                for (int j = 0; j < gesamtObst; j++) {
                    if (überschneidungsTabelle[i, j] > highestVal && überschneidungsTabelle[i, j] > 0) {
                        highestVal = überschneidungsTabelle[i, j];
                        spalten[i] = new List<int>() { j };
                    }
                    else if (überschneidungsTabelle[i, j] == highestVal) {
                        spalten[i].Add(j);
                    }
                }
            }
            for (int i = 0; i < gesamtObst; i++) {
                int highestVal = -1;
                for (int j = 0; j < gesamtObst; j++) {
                    if (überschneidungsTabelle[j, i] > highestVal && überschneidungsTabelle[j, i] > 0) {
                        highestVal = überschneidungsTabelle[j, i];
                        reihen[i] = new List<int>() { j };
                    }
                    else if (überschneidungsTabelle[j, i] == highestVal) {
                        reihen[i].Add(j);
                    }
                }
            }
            

            List<Spieß> newSpieße = new List<Spieß>();
            for (int sp = 0; sp < gesamtObst; sp++) {
                Spieß intersectionSpieß = new Spieß(new List<int>(), new List<string>());
                foreach (int re in spalten[sp]) {
                    if (reihen[re].Contains(sp)) {
                        intersectionSpieß.schüsseln.Add(re + 1);
                    }
                }
                if (intersectionSpieß.schüsseln.Count > 0) {
                    intersectionSpieß.obstSorten.Add(alphabetAllg[sp] + "");
                    intersectionSpieß.updateLength();
                    if (intersectionSpieß.schüsseln.Count != intersectionSpieß.obstSorten.Count) {
                        bool foundPair = false;
                        foreach (Spieß spieß in newSpieße) {
                            if (spieß.schüsseln.Intersect(intersectionSpieß.schüsseln).ToList().Count == intersectionSpieß.schüsseln.Count) {
                                foundPair = true;
                                spieß.obstSorten.AddRange(intersectionSpieß.obstSorten);
                                spieß.updateLength();
                            }
                        }
                        if (foundPair == false) {
                            newSpieße.Add(intersectionSpieß);
                        }
                    }
                    else {
                        newSpieße.Add(intersectionSpieß);
                    }
                }
            }
            return newSpieße;
        }


        static (Spieß wunschSpieß, List<(Spieß spieß, List<string> unpassendeSorten)> spießeHalbfalsch) wunschspießZusammensetzen(List<Spieß> spieße, Spieß wunschSpieß) {
            //wenn ganzer Spieß in wunschSpieß enthalten ist, werden Spieß.schüsseln zu wunschSpieß.schüsseln hinzugefügt
            List<(Spieß spieß, List<string> unpassendeSorten)> spießeHalbfalsch = new List<(Spieß spieß, List<string> unpassendeSorten)>(); //Spieß und nummern der falschen Sorten
            char[] wunschObstChar = new char[wunschSpieß.obstSorten.Count];
            for (int i = 0;i < wunschObstChar.Length;i++) { wunschObstChar[i] = wunschSpieß.obstSorten[i].ToLower()[0]; }
            foreach (Spieß spieß in spieße) {
                List<string> unpassendeSorten = new List<string>();
                foreach (string obst in spieß.obstSorten) {
                    if (!wunschObstChar.Contains(obst.ToLower()[0])) {
                        unpassendeSorten.Add(obst);
                    }
                }
                if (unpassendeSorten.Count == 0) {//ganzerSpießGewunscht
                    wunschSpieß.schüsseln.AddRange(spieß.schüsseln);
                }
                else if (unpassendeSorten.Count != spieß.length) {
                    spießeHalbfalsch.Add((spieß, unpassendeSorten));
                }
            }
            return (wunschSpieß, spießeHalbfalsch);
        }

        static List<Spieß> unbeobachteteObstsortenFinden(List<Spieß> spieße, Spieß wunschSpieß, int gesamtObst) {
            //Abfangen, dass unbeobachtete Obstsorten gewünscht werden
            //wenn n sorten unbeobachtet und gewünscht und sonst keine, kann TROTZDEM eine Lösung ausgegeben werden
            int beobachteteSorten = 0;
            foreach (Spieß sp in spieße) {
                beobachteteSorten += sp.length;
            }
            if (beobachteteSorten != gesamtObst) {
                //gewünschte Sorten, die nicht beobachtet wurden ausfindig machen
                List<string> unbeobachteteSorten = new List<string>();
                foreach (string wunschObst in wunschSpieß.obstSorten) {
                    bool wunschObstBeobachtet = false;
                    foreach (Spieß sp in spieße) {
                        if (sp.obstSorten.Contains(wunschObst.ToLower()[0]+"") || sp.obstSorten.Contains(wunschObst)) { wunschObstBeobachtet = true; break; }
                    }
                    if (!wunschObstBeobachtet) { unbeobachteteSorten.Add(wunschObst); }
                }
                //sollten Sorten weder gewünscht noch beobachtet sein, aber existieren, werden sie als unbekannte Obstsorte hinterlegt
                for (int i = 0; unbeobachteteSorten.Count < gesamtObst - beobachteteSorten; i++) {
                    unbeobachteteSorten.Add("unbekannte Obstsorte " + i);
                }

                //Schüsselnummern, die nicht genannt wurden, aber existieren müssen, werden ausfindig gemacht
                List<int> unbeobachteteSchüsseln = new List<int>();
                for (int i = 1; i <= gesamtObst; i++) {
                    bool schüsselBeobachtet = false;
                    foreach (Spieß spieß in spieße) {
                        if (spieß.schüsseln.Contains(i)) { schüsselBeobachtet = true; break; }
                    }
                    if (!schüsselBeobachtet) { unbeobachteteSchüsseln.Add(i); }
                }

                spieße.Add(new Spieß(unbeobachteteSchüsseln, unbeobachteteSorten));
            }
            return spieße;
        }
    }
}
