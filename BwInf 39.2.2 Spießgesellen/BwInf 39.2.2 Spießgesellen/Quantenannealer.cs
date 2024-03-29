﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Linq;


namespace BwInf_39_2_2_Spießgesellen {
    class Quantenannealer : basisAlgorithmus {

        public Quantenannealer(Spieß orgWunschSpieß, List<Spieß> orgSpieße, int gesamtObst) : base(orgWunschSpieß, orgSpieße, gesamtObst) {
            this.orgWunschSpieß = orgWunschSpieß;
            this.orgSpieße = orgSpieße;
            this.gesamtObst = gesamtObst;
        }

        /// <summary>
        /// löst das Problem auf einem adiabatischen Quantencomputer
        /// </summary>
        /// <returns></returns>
        public (Spieß wunschSpieß, List<Spieß> neueSpieße) quantenannealing() {
            char[] alphabetAllg = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            char[] alphabet = new char[gesamtObst];
            Array.Copy(alphabetAllg, alphabet, gesamtObst);

            float[,] matrix = new float[gesamtObst * gesamtObst, gesamtObst * gesamtObst];
            List<Spieß> neueSpieße = new List<Spieß>();
            foreach (Spieß sp in orgSpieße) { neueSpieße.Add(sp.clone()); }

            neueSpieße = unbeobachteteObstsortenFinden(neueSpieße);

            //Adjazenzmatrix befüllen
            foreach (Spieß spieß in neueSpieße) {
                for (int sch = 0; sch < spieß.schüsseln.Count; sch++) {
                    for (int sor = 0; sor < spieß.obstSorten.Count; sor++) {
                        int posInMatrix = (spieß.schüsseln[sch] - 1) * gesamtObst + Array.FindIndex(alphabet, c => c == spieß.obstSorten[sor].ToLower().ElementAt(0));
                        matrix[posInMatrix, posInMatrix] += -2;
                    }
                }
            }
            for (int schüssel1 = 0; schüssel1 < gesamtObst; schüssel1++) {
                for (int sorte1 = 0; sorte1 < gesamtObst; sorte1++) {
                    int x = schüssel1 * gesamtObst + sorte1;
                    for (int schüssel2 = 0; schüssel2 < gesamtObst; schüssel2++) {
                        for (int sorte2 = 0; sorte2 < gesamtObst; sorte2++) {
                            int y = schüssel2 * gesamtObst + sorte2;
                            if (x != y) {
                                //nur Qubits connecten, die beide die Möglichkeit haben besetzt zu werden (also grundsätzlich belohnt werden beziehungsweise eine Obst-schüssel kombi darstellen, die vorkommen könnte)
                                if (matrix[x, x] != 0 && matrix[y, y] != 0) {
                                    //gleiche Spalte aka schüssel
                                    if (schüssel1 == schüssel2) {
                                        matrix[x, y] += 2;
                                    }
                                    //gleiche Reihe aka sorte
                                    if (sorte1 == sorte2) {
                                        matrix[x, y] += 2;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            QA_Communication.Matrix.saveMatrix(matrix);

            //Matrix verkleinern (Reihen und Spalten entfernen, die leer sind)
            List<int> leereReihen = findeLeereReihen(matrix);
            matrix = verkleinereMatrix(matrix, leereReihen);
            QA_Communication.Matrix.saveMatrix(matrix);
            //Parameter für Quantencomputer festelegen
            List<int>[] ergebnis = new List<int>[gesamtObst];
            try {
                Dictionary<string, string> qaArguments = new Dictionary<string, string>() {
                {"annealing_time","40"},
                {"num_reads","10000"}, //max 10000 (limitation by dwave)
                {"chain_strength","4" }
                };
                Dictionary<string, string> pyParams = new Dictionary<string, string>() {
                {"problem_type","qubo"}, //qubo //ising
                {"dwave_solver", "Advantage_system1.1"}, //DW_2000Q_6 //Advantage_system1.1
                {"dwave_inspector","true" }
                };

                string qaArgumentsString = string.Join(",", qaArguments.Select(x => x.Key + "=" + x.Value).ToArray());
                string pyParamsString = string.Join(",", pyParams.Select(x => x.Value).ToArray());
                //daten dür python speichern
                Program.saveFile(new string[] { QA_Communication.Matrix.toQUBOString(matrix) }, "qubomatrix");
                Program.saveFile( new string[]{qaArgumentsString, pyParamsString }, "data.txt");

                Console.WriteLine("jetzt die Datei embed-and-run.py ausführen und sobald sie fertig ist auf enter drücken"); Console.ReadLine();

                //Ergebnisse lesen
                QA_Communication.qaConstellation constellation = new QA_Communication.qaConstellation(getResults(), qaArguments, pyParams);
                constellation.printConstellation(20);
                //constellation.plotEnergyDistribution();
                (neueSpieße, ergebnis) = decodiereQCErgebnis(constellation, leereReihen, alphabet);

                (Spieß neuWunschSpieß, List<(Spieß spieß, List<string> unpassendeSorten)> spießeHalbfalsch) = wunschspießZusammensetzen(neueSpieße);

                //Ausgabe
                Console.WriteLine("\nSOLUTION QANTUM");
                for (int s = 0; s < ergebnis.Length; s++) {
                    Console.WriteLine(alphabet[s] + " : " + string.Join(",", ergebnis[s]));
                }
                Console.WriteLine("\nWUNSCHSPIESS");
                neuWunschSpieß.printSpieß();

                QA_Communication.Program.getUserInput(constellation, matrix);

                return (neuWunschSpieß, neueSpieße);
            }
            catch (Exception e) {
                Console.WriteLine("\nERROR occured:");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return (orgWunschSpieß, orgSpieße);
            }
        }

        #region Kommunikation mit Quantencomputer
        
        /// <summary>
        /// lädt results, die vom python script gespeichert wurden
        /// </summary>
        /// <returns></returns>
        List<(float energy, int numOccurrences, float chainBreakFraction, int[] result)> getResults() {
            string[] lines = Program.loadFile("results.txt");
            List<(float energy, int numOccurrences, float chainBreakFraction, int[] result)> results = new List<(float energy, int numOccurrences, float chainBreakFraction, int[] result)>();
            foreach(string line in lines) {
                string[] args = line.Split('\t');
                List<int> result = new List<int>();
                string[] resultString = args[3].Split(' ');
                foreach(string num in resultString) {
                    result.Add(int.Parse(num.Replace("[", "").Replace("]", "").Trim()));
                }
                results.Add((float.Parse(args[0].Replace(".", ",")), int.Parse(args[2]), float.Parse(args[1].Replace(".",",")), result.ToArray()));
            }
            return results;
        }
        
        #endregion
        #region verarbeite Ergebnis/ Matrix

        /// <summary>
        /// findet alle leeren Reihen/Spalten in matrix
        /// </summary>
        List<int> findeLeereReihen(float[,] matrix) {
            List<int> leereReihen = new List<int>();
            for (int i = 0; i < matrix.GetLength(0); i++) {
                bool reiheLeer = true;
                for (int j = 0; j < matrix.GetLength(1); j++) {
                    if (matrix[i, j] != 0) {
                        reiheLeer = false;
                        break;
                    }
                }
                if (reiheLeer) {
                    leereReihen.Add(i);
                }
            }
            return leereReihen;
        }

        /// <summary>
        /// entfernt leere Reihen & Spalten aus matrix, damit Problem für Quantencomputer besser lösbar wird
        /// </summary>
        /// <param name="leereReihen">Liste aller leeren Reihen (da Dreiecksmatrix entsprechen leere Reihen den leeren Spalten)</param>
        /// <returns>verkleinerte Matrix</returns>
        float[,] verkleinereMatrix(float[,] matrix, List<int> leereReihen) {
            int neueLänge = matrix.GetLength(0) - leereReihen.Count;
            float[,] neueMatrix = new float[neueLänge, neueLänge];

            int countX = 0, countY = 0;
            for (int i = 0; i < matrix.GetLength(1); i++) {
                if (!leereReihen.Contains(i)) {
                    for (int j = 0; j < matrix.GetLength(0); j++) {
                        if (!leereReihen.Contains(j)) {
                            neueMatrix[countX, countY] = matrix[i, j];
                            countX++;
                        }
                    }
                    countY++;
                    countX = 0;
                }
            }
            return neueMatrix;
        }

        /// <summary>
        /// aus Ergebnis in 0 und 1 Spieße ableiten
        /// </summary>
        /// <param name="constellation">Konstellation der Eingaben/Ausgaben des Quantencomputers</param>
        /// <returns></returns>
        (List<Spieß> returnSpieße, List<int>[] solution) decodiereQCErgebnis(QA_Communication.qaConstellation constellation, List<int> leereReihen, char[] alphabet) {
            List<int>[] solution = new List<int>[gesamtObst]; //index entspricht sorte, value entspricht schüssel
            for (int i = 0; i < solution.Length; i++) { solution[i] = new List<int>(); }
            List<Spieß> returnSpieße = new List<Spieß>();

            int[] ergebnisKombiniert = new int[gesamtObst * gesamtObst];
            var besteErgebnisse = constellation.getLowest(1, new List<int>()); //beste Ergebnisse finden

            //kombiniere beste Ergebnisse
            foreach (int index in besteErgebnisse) {
                int[] erweitertesErgebnis = vergrößereErgebnis(constellation.results[index].result, leereReihen);
                for (int i = 0; i < gesamtObst * gesamtObst; i++) {
                    ergebnisKombiniert[i] += erweitertesErgebnis[i];
                }
            }
            //da unter den besten Ergebnissen oft ein paar falsche Zuordnungen vorkommen, sucht das Programm für 
            //jede Schüssel die Sorten raus, die am öftesten in allen besten Ergebnissen zugeordnet wurde (bzw deren Wert in ergebnisKombiniert am höchsten ist)
            for (int sch = 0; sch < gesamtObst; sch++) {
                List<int> häufigsteSorten = new List<int>() { 0 }; //häufigsteSorten in ergebnisKombiniert
                for (int sor = 0; sor < gesamtObst; sor++) {
                    if (ergebnisKombiniert[sch * gesamtObst + sor] > ergebnisKombiniert[sch * gesamtObst + häufigsteSorten[0]]) {
                        häufigsteSorten = new List<int>() { sor };
                    }
                    else if (ergebnisKombiniert[sch * gesamtObst + sor] == ergebnisKombiniert[sch * gesamtObst + häufigsteSorten[0]] && ergebnisKombiniert[sch * gesamtObst + sor] > 0 && sor > 0) {
                        häufigsteSorten.Add(sor);
                    }
                }
                foreach (int sorte in häufigsteSorten) {
                    solution[sorte].Add(sch + 1);
                    returnSpieße.Add(new Spieß(new List<int>() { sch + 1 }, new List<string>() { alphabet[sorte] + "" }));
                }
            }
            return (returnSpieße, solution);
        }

        /// <summary>
        /// erweitern des Ergebnisses mit den vorher aus der Matrix herausgenommenen leeren Reihen/Spalten
        /// um eine einfachere Dekodierung zu ermöglichen
        /// </summary>
        /// <returns>erweitertes Ergebnis</returns>
        int[] vergrößereErgebnis(int[] ergebnis, List<int> leereReihen) {
            int[] neuesErgebnis = new int[ergebnis.Length + leereReihen.Count];
            int neuePositionDifferenz = 0;
            for (int i = 0; i < neuesErgebnis.Length; i++) {
                if (!leereReihen.Contains(i)) {
                    neuesErgebnis[i] = ergebnis[i - neuePositionDifferenz];
                }
                else { neuesErgebnis[i] = 0; neuePositionDifferenz++; }
            }
            return neuesErgebnis;
        }

        #endregion
    }

}
