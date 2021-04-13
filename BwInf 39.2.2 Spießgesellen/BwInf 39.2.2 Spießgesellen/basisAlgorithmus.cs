using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BwInf_39_2_2_Spießgesellen {
    class basisAlgorithmus {
        protected Spieß orgWunschSpieß; protected List<Spieß> orgSpieße; protected int gesamtObst;

        public basisAlgorithmus(Spieß orgWunschSpieß, List<Spieß> orgSpieße, int gesamtObst) {
            this.orgWunschSpieß = orgWunschSpieß;
            this.orgSpieße = orgSpieße;
            this.gesamtObst = gesamtObst;
        }

        #region Spieße verarbeiten

        /* Fall abfangen, dass eine Obstsorte gewünscht ist, die nicht beobachtet wurde. 
         * Der Vollständigkeit halber werden unbeobachtete Obstsorten auch unabhängig davon ob sie gewünscht wurden ergänzt.
         * ANSATZ:
         * 1: unbeobachtete Obstsorten sammeln (entweder mit Namen von wunschSpieß oder als "unbeobachtetes Obst n")
         * 2: unbeobachtete Schüsselnummern sammeln
         * 3: neuer Spieß aus obstsorten und schüsseln
         * */
        protected List<Spieß> unbeobachteteObstsortenFinden(List<Spieß> spieße) {
            //Abfangen, dass unbeobachtete Obstsorten gewünscht werden
            //wenn n sorten unbeobachtet und gewünscht und sonst keine, kann TROTZDEM eine Lösung ausgegeben werden
            int beobachteteSorten = 0;
            foreach (Spieß sp in spieße) {
                beobachteteSorten += sp.length;
            }
            if (beobachteteSorten != gesamtObst) {
                //gewünschte Sorten, die nicht beobachtet wurden ausfindig machen
                List<string> unbeobachteteSorten = new List<string>();
                foreach (string wunschObst in orgWunschSpieß.obstSorten) {
                    bool wunschObstBeobachtet = false;
                    foreach (Spieß sp in spieße) {
                        if (sp.obstSorten.Contains(wunschObst.ToLower()[0] + "") || sp.obstSorten.Contains(wunschObst)) { wunschObstBeobachtet = true; break; }
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

        //wenn ganzer Spieß in wunschSpieß enthalten ist, werden Spieß.schüsseln zu wunschSpieß.schüsseln hinzugefügt
        protected (Spieß wunschSpieß, List<(Spieß spieß, List<string> unpassendeSorten)> spießeHalbfalsch) wunschspießZusammensetzen(List<Spieß> spieße) {
            List<(Spieß spieß, List<string> unpassendeSorten)> spießeHalbfalsch = new List<(Spieß spieß, List<string> unpassendeSorten)>(); //Spieße mit nur teils gewünschten Obstsorten
            Spieß wunschSpieß = orgWunschSpieß.clone();
            char[] wunschObstChar = new char[wunschSpieß.obstSorten.Count]; //flexibler mit chars statt strings (notwendig für Quantenannealer)
            for (int i = 0; i < wunschObstChar.Length; i++) { wunschObstChar[i] = wunschSpieß.obstSorten[i].ToLower()[0]; }

            foreach (Spieß spieß in spieße) {
                List<string> unpassendeSorten = new List<string>();
                foreach (string obst in spieß.obstSorten) {
                    if (!wunschObstChar.Contains(obst.ToLower()[0])) {
                        unpassendeSorten.Add(obst);
                    }
                }
                if (unpassendeSorten.Count == 0) {//ganzer Spieß gewünscht
                    wunschSpieß.schüsseln.AddRange(spieß.schüsseln);
                }
                else if (unpassendeSorten.Count != spieß.length) {//wenn ein Teil des Spießes gewünscht ist -> spießeHalbfalsch
                    spießeHalbfalsch.Add((spieß, unpassendeSorten));
                }
            }
            return (wunschSpieß, spießeHalbfalsch);
        }

        #endregion
        #region Ergebnisausgabe

        protected virtual void printResult(List<Spieß> spieße, Spieß wunschSpieß, List<(Spieß spieß, List<string> unpassendeSorten)> spießeHalbfalsch) {
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

        /// <summary>
        /// Ausgabe der teilweise passenden Spieße in Kurzform.
        /// Bsp.: *rot*Banane Ugli  -> 25 18
        /// </summary>
        /// <param name="spießeHalbfalsch">Liste der teilweise richtigen Spieße mit List<string> der ungewünschten Obstsorten</param>
        protected void printHalbfalschKurz(List<(Spieß spieß, List<string> unpassendeSorten)> spießeHalbfalsch) {
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
        protected void printHalbfalschWortform(List<(Spieß spieß, List<string> unpassendeSorten)> spießeHalbfalsch) {
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

        #endregion

        
    }
}
