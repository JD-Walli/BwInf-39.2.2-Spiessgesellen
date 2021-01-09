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
        public static Tuple<Spieß, List<Spieß>> algorithmus(Spieß wunschSpieß, List<Spieß> spieße, int gesamtObst) {
            #region Spieße aufspalten
            bool didChange = true;
            int counter = 0;
            while (didChange) {
                didChange = false;
                for (int i = 0; i < spieße.Count; i++) {
                    for (int j = i; j < spieße.Count; j++) {
                        if (i != j) {
                            Tuple<Spieß, Spieß> tup1 = spieße[i].vergleicheSpieße(spieße[j]);
                            if (tup1.Item2.length > 0) {
                                spieße[j] = tup1.Item1;
                                spieße.Add(tup1.Item2);
                                didChange = true;
                            }
                        }
                    }
                }
                counter++;
            }
            Console.WriteLine(counter);
            spieße.RemoveAll(sp => sp.length == 0);//laufzeitoptimierung?
            #endregion

            #region gewünschte, unbeobachtete Obstsorten verarbeiten
            //Abfangen, dass unbeobachtete Obstsorten gewünscht werden
            //wenn n sorten unbeobachtet und gewünscht und sonst keine, kann TROTZDEM eine Lösung ausgegeben werden
            int beobachteteSorten = 0;
            foreach (Spieß sp in spieße) {
                beobachteteSorten += sp.length;
            }
            if (beobachteteSorten != gesamtObst) {
                //gewünschte Sorten, die nicht beobachtet werden ausfindig machen
                List<string> unbeobachteteSorten = new List<string>();
                foreach (string wunschObst in wunschSpieß.obstSorten) {
                    bool wunschObstBeobachtet = false;
                    foreach (Spieß sp in spieße) {
                        if (sp.obstSorten.Contains(wunschObst)) { wunschObstBeobachtet = true; }
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
                        if (spieß.schüsseln.Contains(i)) { schüsselBeobachtet = true; }
                    }
                    if (!schüsselBeobachtet) { unbeobachteteSchüsseln.Add(i); }
                }

                spieße.Add(new Spieß(unbeobachteteSchüsseln, unbeobachteteSorten));
            }
            #endregion

            #region Wunschspieß berechnen
            //wenn ganzer Spieß in wunschSpieß enthalten ist, werden Spieß.schüsseln zu wunschSpieß.schüsseln hinzugefügt
            List<Tuple<Spieß, List<string>>> spießeHalbfalsch = new List<Tuple<Spieß, List<string>>>(); //Spieß und nummern der falschen Sorten
            foreach (Spieß spieß in spieße) {
                List<string> unpassendeSorten = new List<string>();
                foreach (string obst in spieß.obstSorten) {
                    if (!wunschSpieß.obstSorten.Contains(obst)) {
                        unpassendeSorten.Add(obst);
                    }
                }
                if (unpassendeSorten.Count == 0) {//ganzerSpießGewunscht
                    wunschSpieß.schüsseln.AddRange(spieß.schüsseln);
                }
                else if (unpassendeSorten.Count != spieß.length) {
                    spießeHalbfalsch.Add(new Tuple<Spieß, List<string>>(spieß, unpassendeSorten));
                }
            }
            #endregion


            Console.WriteLine("\nAUFGESPLITETE SPIESSE:");
            foreach (Spieß spieß in spieße) {
                spieß.printSpieß();
            }
            Console.WriteLine("\nWUNSCHSPIESS:");
            wunschSpieß.printSpieß();

            if (spießeHalbfalsch.Count > 0) {
                printHalbfalschKurz(spießeHalbfalsch);
            }

            return new Tuple<Spieß, List<Spieß>>(wunschSpieß, spieße);
        }

        /// <summary>
        /// Ausgabe der teilweise passenden Spieße in Kurzform.
        /// Bsp.: *rot*Banane Ugli  -> 25 18
        /// </summary>
        /// <param name="spießeHalbfalsch">Liste der teilweise richtigen Spieße mit List<string> der ungewünschten Obstsorten</param>
        public static void printHalbfalschKurz(List<Tuple<Spieß, List<string>>> spießeHalbfalsch) {
            //spießeHalbfalsch.Sort((tup1, tup2) => ((tup1.Item1.length-tup1.Item2.Count) / tup1.Item1.length).CompareTo((tup2.Item1.length - tup2.Item2.Count) / tup2.Item1.length));//elemente mit einem besseren gewünschteSortenImSpieß zu Spießlänge Verhältnis weiter oben
            Console.WriteLine();
            for (int i = 0; i < spießeHalbfalsch.Count; i++) {
                foreach (string obst in spießeHalbfalsch[i].Item1.obstSorten) {
                    Console.ForegroundColor = spießeHalbfalsch[i].Item2.Contains(obst) ? ConsoleColor.DarkRed : ConsoleColor.White;
                    Console.Write(obst + " ");
                }
                Console.Write(" -> ");
                foreach (int schüssel in spießeHalbfalsch[i].Item1.schüsseln) { Console.Write(schüssel + " "); }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Ausgabe der teilweise passenden Spieße in ausformulierter Form.
        /// Bsp.: "Ugli" ist in den Schüsseln 25, 18; in einer der Schüsseln ist die nicht-gewünschte Sorte "Banane" enthalten
        /// </summary>
        /// <param name="spießeHalbfalsch">Liste der teilweise richtigen Spieße mit List<string> der ungewünschten Obstsorten</string></param>
        public static void printHalbfalschWortform(List<Tuple<Spieß, List<string>>> spießeHalbfalsch) {
            Console.Write("\nfolgende gewünschte Obssorten konnten nicht zugeordnet werden: ");
            string output = "\"";
            for (int i = 0; i < spießeHalbfalsch.Count; i++) {
                for (int j = 0; j < spießeHalbfalsch[i].Item1.obstSorten.Count; j++) {
                    if (!spießeHalbfalsch[i].Item2.Contains(spießeHalbfalsch[i].Item1.obstSorten[j]))
                        output += spießeHalbfalsch[i].Item1.obstSorten[j] + "\", \"";
                }
            }
            output = output.Remove(output.Length - 3, 3);
            output += "\n\n";
            for (int i = 0; i < spießeHalbfalsch.Count; i++) {
                foreach (string sorte in spießeHalbfalsch[i].Item1.obstSorten) {
                    if (!spießeHalbfalsch[i].Item2.Contains(sorte)) {
                        output += "\"" + sorte + "\", ";
                    }
                }
                output = output.Remove(output.Length - 2, 2);
                output += (spießeHalbfalsch[i].Item1.obstSorten.Count - spießeHalbfalsch[i].Item2.Count == 1 ? " ist" : " sind") + " in den Schüsseln ";
                output += string.Join(", ", spießeHalbfalsch[i].Item1.schüsseln);
                output += spießeHalbfalsch[i].Item2.Count == 1 ? "; in einer der Schüsseln ist die nicht-gewünschte Sorte \"" : "; in mehreren der Schüsseln sind die nicht-gewünschten Sorten \"";
                output += string.Join("\", ", spießeHalbfalsch[i].Item2);
                output += "\" enthalten \n";
            }
            Console.WriteLine(output);
        }
    }
}
