using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BwInf_39_2_2_Spießgesellen {
    class Algorithmus : basisAlgorithmus {

        public Algorithmus(Spieß orgWunschSpieß, List<Spieß> orgSpieße, int gesamtObst):base(orgWunschSpieß,orgSpieße,gesamtObst) {
            this.orgWunschSpieß = orgWunschSpieß;
            this.orgSpieße = orgSpieße;
            this.gesamtObst = gesamtObst;
        }

        /// <summary>
        /// berechnet Wunschspieß.schüsseln
        /// returns Wunschspieß und gesplitete spieße
        /// </summary>
        /// <param name="wunschSpieß">gewünschte Obstsorten</param>
        /// <param name="spieße">Liste mit beobachteten Spießen</param>
        /// <param name="gesamtObst">anzahl aller Obstsorten</param>
        /// <returns></returns>
        public (Spieß wunschSpieß, List<Spieß> spieße) algorithmus1() {
            List<Spieß> newSpieße = new List<Spieß>();
            foreach (Spieß sp in orgSpieße) { newSpieße.Add(sp.clone()); }

            newSpieße = spießeAufspalten(newSpieße);

            newSpieße = unbeobachteteObstsortenFinden(newSpieße);

            (Spieß wunschSpießNew, List<(Spieß spieß, List<string> unpassendeSorten)> spießeHalbfalsch) = wunschspießZusammensetzen(newSpieße);

            printResult(newSpieße, wunschSpießNew, spießeHalbfalsch);

            return (wunschSpießNew, newSpieße);
        }

        /// <summary>
        /// berechnet Wunschspieß.schüsseln
        /// returns Wunschspieß und gesplitete spieße
        /// </summary>
        /// <param name="wunschSpieß">gewünschte Obstsorten</param>
        /// <param name="spieße">Liste mit beobachteten Spießen</param>
        /// <param name="gesamtObst">anzahl aller Obstsorten</param>
        /// <returns></returns>
        public (Spieß wunschSpieß, List<Spieß> spieße) algorithmus2() {
            List<Spieß> newSpieße = new List<Spieß>();
            foreach (Spieß sp in orgSpieße) { newSpieße.Add(sp.clone()); }

            newSpieße = spießeAufspalten2(orgSpieße);

            newSpieße = unbeobachteteObstsortenFinden(newSpieße);

            (Spieß wunschSpießNew, List<(Spieß spieß, List<string> unpassendeSorten)> spießeHalbfalsch) = wunschspießZusammensetzen(newSpieße);

            printResult(newSpieße, wunschSpießNew, spießeHalbfalsch);

            return (wunschSpießNew, newSpieße);
        }

        #region Spieße Aufspalten

        // O(n)=x*spieße^2
        /*ANSATZ:
         * alle zwei Spieße werden miteinander verglichen. wenn sie gemeinsame Obstsorten/Schüsseln haben werden diese gemeinsamen als 
         * neuer Spieß gespeichert und von den ursprünglichen Spießen entfernt.
         * So kann man möglichst reduzierte Spieße erzeugen, die wiederum zum Wunschspieß zusammengesetzt werden können.
         * */
        List<Spieß> spießeAufspalten(List<Spieß> spieße) {
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
        /*ANSATZ:
         * 1: In einer int Tabelle[obstSorten][schüsseln] wird der Tabellenwert an der Position von jedem obstsorte-schüssel Paar, was in den Spießen vorkommt bzw möglich ist, um 1 erhöht
         * 2: für jede reihe und spalte in der Tabelle werden die indices der jeweils größten Tabellenwerte gefunden
         * 3: wenn ein Tabelleneintrag sowohl der größte der Reihe als auch der größte dieser Spalte ist, ist deren Kombination ein neuer kleinstmöglicher Spieß. Diese Paare werden gesucht und zu newSpieße hinzugefügt.
         */
        List<Spieß> spießeAufspalten2(List<Spieß> spieße) {
            Dictionary<char, int> alphabet = new Dictionary<char, int>();
            char[] alphabetAllg = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            for (int i = 0; i < alphabetAllg.Length; i++) { alphabet.Add(alphabetAllg[i], i); }

            //Tabelle wird mit möglichen obstsorte-schlüssel Paaren befüllt.
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
            for (int e = 0; e < gesamtObst; e++) { spalten[e] = new List<int>(); reihen[e] = new List<int>(); }

            for (int i = 0; i < gesamtObst; i++) {
                int höchsterWertSpalte = -1;
                int höchsterWertReihe = -1;
                for (int j = 0; j < gesamtObst; j++) {
                    //finde indices der höchsten Werte in der betrachteten Spalte i
                    if (überschneidungsTabelle[i, j] > höchsterWertSpalte && überschneidungsTabelle[i, j] > 0) {
                        höchsterWertSpalte = überschneidungsTabelle[i, j];
                        spalten[i] = new List<int>() { j };
                    }
                    else if (überschneidungsTabelle[i, j] == höchsterWertSpalte) {
                        spalten[i].Add(j);
                    }

                    //finde indices der höchsten Werte in der betrachteten Reihe i
                    if (überschneidungsTabelle[j, i] > höchsterWertReihe && überschneidungsTabelle[j, i] > 0) {
                        höchsterWertReihe = überschneidungsTabelle[j, i];
                        reihen[i] = new List<int>() { j };
                    }
                    else if (überschneidungsTabelle[j, i] == höchsterWertReihe) {
                        reihen[i].Add(j);
                    }
                }
            }

            //Spieße aus den ausgewählten höchstewerten der überschneidungstabelle bilden
            List<Spieß> newSpieße = new List<Spieß>();
            for (int spaltenInd = 0; spaltenInd < gesamtObst; spaltenInd++) {
                Spieß intersectionSpieß = new Spieß(new List<int>(), new List<string>());

                //indizes der Schüsseln hinzufügen, die in der Spalte einen Reihenhöchstwert haben.
                foreach (int schüsselInd in spalten[spaltenInd]) {
                    if (reihen[schüsselInd].Contains(spaltenInd)) {
                        intersectionSpieß.schüsseln.Add(schüsselInd + 1);
                    }
                }

                if (intersectionSpieß.schüsseln.Count > 0) {
                    intersectionSpieß.obstSorten.Add(alphabetAllg[spaltenInd] + "");
                    intersectionSpieß.updateLength();
                    //wenn mehrere schüsseln zur obstsorte gefunden wurden müssen weitere obstsorten auch die gleichen Obstsorten zugeordnet haben. 
                    //daher wird bei den bisher hinzugefügten geprüft, ob die Verteilung schon gibt und dementsprechend die Obstsorte zum Spieß hinzugefügt.
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

        #endregion

    }
}
