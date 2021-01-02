using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BwInf_39_2_2_Spießgesellen {
    class Program {
        static void Main(string[] args) {
            var tup = readData(7);
            List<Spieß> spieße = tup.Item2;
            Console.WriteLine("WUNSCHSORTEN:\n{0}", string.Join(", ", tup.Item1.obstSorten));
            Console.WriteLine("\nBEOBACHTETE SPIESSE:");
            foreach (Spieß spieß in spieße) {
                spieß.printSpieß();
            }
            algorithmus(tup.Item1, spieße, tup.Item3);
            Console.ReadLine();
        }

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
            }
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
                //spießeHalbfalsch.Sort((tup1, tup2) => ((tup1.Item1.length-tup1.Item2.Count) / tup1.Item1.length).CompareTo((tup2.Item1.length - tup2.Item2.Count) / tup2.Item1.length));//elemente mit einem besseren gewünschteSortenImSpieß zu Spießlänge Verhältnis weiter oben

                var query = spießeHalbfalsch.OrderBy(tup1 => (float)tup1.Item1.length / (float)tup1.Item2.Count);//((tup1.Item1.length - tup1.Item2.Count) / tup1.Item1.length));
                for (int i = 0; i < spießeHalbfalsch.Count; i++) {
                    foreach (string obst in spießeHalbfalsch[i].Item1.obstSorten) {
                        Console.ForegroundColor = spießeHalbfalsch[i].Item2.Contains(obst) ? ConsoleColor.DarkRed : ConsoleColor.White;
                        Console.Write(obst + " ");
                    }
                    Console.Write(" -> ");
                    foreach (int schüssel in spießeHalbfalsch[i].Item1.schüsseln) {
                        Console.Write(schüssel + " ");
                    }
                    Console.WriteLine();
                }
            }
            return new Tuple<Spieß, List<Spieß>>(wunschSpieß, spieße);
        }

        /// <summary>
        /// reads data; returns Tuple von Wunschspieß, Liste von beobachteten Spießen, Anzahl an obstSorten/schüsseln
        /// </summary>
        /// <param name="number">number of file (eg spieße3.txt -> number=3)</param>
        /// <returns>Tuple von Wunschspieß und einer Liste von beobachteten Spießen</returns>
        public static Tuple<Spieß, List<Spieß>, int> readData(int number) {
            string[] lines = System.IO.File.ReadAllLines(System.IO.Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "/spieße" + number + ".txt");
            Spieß wunschspieß = new Spieß(new List<int>(), lines[1].Trim().Split(' ').ToList());
            List<Spieß> spieße = new List<Spieß>();
            for (int i = 3; i < int.Parse(lines[2]) * 2 + 3; i += 2) {
                spieße.Add(new Spieß(lines[i].Trim().Split(' ').ToList(), lines[i + 1].Trim().Split(' ').ToList()));
            }
            return new Tuple<Spieß, List<Spieß>, int>(wunschspieß, spieße, int.Parse(lines[0].Trim()));
        }
    }
}
