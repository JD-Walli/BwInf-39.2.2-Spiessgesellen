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
        public (Spieß wunschSpieß, List<Spieß> spieße) algorithmus1() {
            List<Spieß> neueSpieße = new List<Spieß>();
            foreach (Spieß sp in orgSpieße) { neueSpieße.Add(sp.clone()); }

            neueSpieße = spießeAufspalten(neueSpieße);

            neueSpieße = unbeobachteteObstsortenFinden(neueSpieße);

            (Spieß wunschSpießNew, List<(Spieß spieß, List<string> unpassendeSorten)> spießeHalbfalsch) = wunschspießZusammensetzen(neueSpieße);

            printErgebnis(neueSpieße, wunschSpießNew, spießeHalbfalsch);

            return (wunschSpießNew, neueSpieße);
        }

        /// <summary>
        /// berechnet Wunschspieß.schüsseln
        /// returns Wunschspieß und gesplitete spieße
        /// </summary>
        public (Spieß wunschSpieß, List<Spieß> spieße) algorithmus2() {
            List<Spieß> neueSpieße = new List<Spieß>();
            foreach (Spieß sp in orgSpieße) { neueSpieße.Add(sp.clone()); }

            neueSpieße = spießeAufspalten2(orgSpieße);

            neueSpieße = unbeobachteteObstsortenFinden(neueSpieße);

            (Spieß wunschSpießNew, List<(Spieß spieß, List<string> unpassendeSorten)> spießeHalbfalsch) = wunschspießZusammensetzen(neueSpieße);

            printErgebnis(neueSpieße, wunschSpießNew, spießeHalbfalsch);

            return (wunschSpießNew, neueSpieße);
        }

        #region Spieße Aufspalten

        // O(n)=x*spieße^2
        /*ANSATZ:
         * alle zwei Spieße werden miteinander verglichen. wenn sie gemeinsame Obstsorten/Schüsseln haben werden diese gemeinsamen als 
         * neuer Spieß gespeichert und von den ursprünglichen Spießen entfernt.
         * So kann man möglichst reduzierte Spieße erzeugen, die wiederum zum Wunschspieß zusammengesetzt werden können.
         * */
         /// <summary>
         /// möglichst kleinste Obstsorte->Schüssel Zuordnungen finden
         /// Ansatz: Spieße vergleichen und Schnittspieße bilden
         /// </summary>
         /// <returns>aufgespaltete Spieße</returns>
        List<Spieß> spießeAufspalten(List<Spieß> spieße) {
                for (int i = 0; i < spieße.Count; i++) {
                    for (int j = i; j < spieße.Count; j++) {
                        if (i != j) {
                            (Spieß spieß2neu, Spieß schnittSpieß) = spieße[i].vergleicheSpieße(spieße[j]);
                            if (schnittSpieß.länge > 0) {
                                spieße[j] = spieß2neu;
                                spieße.Add(schnittSpieß);
                            }
                        }
                    }
                }
            spieße.RemoveAll(sp => sp.länge == 0);
            return spieße;
        }

        //O(n)= spieße^2 + gesamtObst^3 + spieße+schüsseln*sorten
        /*ANSATZ:
         * 1: In einer int Tabelle[obstSorten][schüsseln] wird der Tabellenwert an der Position von jedem obstsorte-schüssel Paar, was in den Spießen vorkommt bzw möglich ist, um 1 erhöht
         * 2: für jede reihe und spalte in der Tabelle werden die indices der jeweils größten Tabellenwerte gefunden
         * 3: wenn ein Tabelleneintrag sowohl der größte der Reihe als auch der größte dieser Spalte ist, ist deren Kombination ein neuer kleinstmöglicher Spieß. Diese Paare werden gesucht und zu neueSpieße hinzugefügt.
         */
        /// <summary>
        /// möglichst kleinste Obstsorte->Schüssel Zuordnungen finden
        /// Ansatz: in Spießen vorkommende Sorte->Schüssel Kombinationen in Tabelle eintragen und größte Werte auslesen
        /// </summary>
        /// <returns>aufgespaltete Spieße</returns>
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
            List<Spieß> neueSpieße = new List<Spieß>();
            for (int spaltenInd = 0; spaltenInd < gesamtObst; spaltenInd++) {
                Spieß intersectionSpieß = new Spieß(new List<int>(), new List<string>());

                //indizes der Schüsseln hinzufügen, die in der Spalte einen Reihenhöchstwert haben.
                foreach (int schüsselIndex in spalten[spaltenInd]) {
                    if (reihen[schüsselIndex].Contains(spaltenInd)) {
                        intersectionSpieß.schüsseln.Add(schüsselIndex + 1);
                    }
                }

                if (intersectionSpieß.schüsseln.Count > 0) {
                    intersectionSpieß.obstSorten.Add(alphabetAllg[spaltenInd] + "");
                    intersectionSpieß.updateLänge();
                    //wenn mehrere schüsseln zur obstsorte gefunden wurden müssen weitere obstsorten auch die gleichen Obstsorten zugeordnet haben. 
                    //daher wird bei den bisher hinzugefügten geprüft, ob die Verteilung schon gibt und dementsprechend die Obstsorte zum Spieß hinzugefügt.
                    if (intersectionSpieß.schüsseln.Count != intersectionSpieß.obstSorten.Count) {
                        bool paarGefunden = false;
                        foreach (Spieß spieß in neueSpieße) {
                            if (spieß.schüsseln.Intersect(intersectionSpieß.schüsseln).ToList().Count == intersectionSpieß.schüsseln.Count) {
                                paarGefunden = true;
                                spieß.obstSorten.AddRange(intersectionSpieß.obstSorten);
                                spieß.updateLänge();
                            }
                        }
                        if (paarGefunden == false) {
                            neueSpieße.Add(intersectionSpieß);
                        }
                    }
                    else {
                        neueSpieße.Add(intersectionSpieß);
                    }
                }
            }
            return neueSpieße;
        }

        #endregion

    }
}
