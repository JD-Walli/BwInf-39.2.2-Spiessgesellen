using System;
using System.Collections.Generic;
using System.Linq;
using QA_Communication;
using System.Text;
using System.Threading.Tasks;


//TODO: bestrafen wenn zwei Felder nicht gleichzeitig besetzt sein dürfen/Zeilen und Spalten rauslöschen die nicht gebraucht werden: wo kein qubit, kann auch keine unerwünschte 1 sein
//TODO: Matrix tauschen: erst sorten, dann schüssel (macht decoding und andere einfacher zu verstehen)
namespace BwInf_39_2_2_Spießgesellen {
    class Quantenannealer : basisAlgorithmus {

        public Quantenannealer(Spieß orgWunschSpieß, List<Spieß> orgSpieße, int gesamtObst) : base(orgWunschSpieß, orgSpieße, gesamtObst) {
            this.orgWunschSpieß = orgWunschSpieß;
            this.orgSpieße = orgSpieße;
            this.gesamtObst = gesamtObst;
        }

        public (Spieß wunschSpieß, List<Spieß> newSpieße) quantenannealer() {
            char[] alphabetAllg = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            char[] alphabet = new char[gesamtObst];
            Array.Copy(alphabetAllg, alphabet, gesamtObst);
            float[,] matrix = new float[gesamtObst * gesamtObst, gesamtObst * gesamtObst];
            List<Spieß> newSpieße = new List<Spieß>();
            foreach (Spieß sp in orgSpieße) { newSpieße.Add(sp.clone()); }

            #region gewünschte, unbeobachtete Obstsorten verarbeiten
            newSpieße = unbeobachteteObstsortenFinden(newSpieße);
            #endregion
            //O(n)= spießeC*gesamtobst³
            //Adjazenzmatrix befüllen
            foreach (Spieß spieß in newSpieße) {
                for (int sch = 0; sch < spieß.schüsseln.Count; sch++) {
                    for (int sor = 0; sor < spieß.obstSorten.Count; sor++) {
                        int posInMatrix = (spieß.schüsseln[sch] - 1) * gesamtObst + Array.FindIndex(alphabet, c => c == spieß.obstSorten[sor].ToLower().ElementAt(0));
                        matrix[posInMatrix, posInMatrix] += -2;
                    }
                }
            }
            for (int sch1 = 0; sch1 < gesamtObst; sch1++) {
                for (int sor1 = 0; sor1 < gesamtObst; sor1++) {
                    int x = sch1 * gesamtObst + sor1;
                    for (int sch2 = 0; sch2 < gesamtObst; sch2++) {
                        for (int sor2 = 0; sor2 < gesamtObst; sor2++) {
                            int y = sch2 * gesamtObst + sor2;
                            if (x != y) {
                                //nur Qubits connecten, die beide die Möglichkeit haben besetzt zu werden (=grundsätzlich belohnt werden;=eine Obst-schüssel kombi darstellen, die vorkommen könnte)
                                if (matrix[x, x] != 0 && matrix[y, y] != 0) {
                                    //gleiche Spalte aka schüssel
                                    if (sch1 == sch2) {
                                        matrix[x, y] += 2;
                                    }
                                    //gleiche Reihe aka sorte
                                    if (sor1 == sor2) {
                                        matrix[x, y] += 2;
                                    }
                                }
                            }
                            else if (matrix[x, y] == 0) {
                                //matrix[x, y] = 1;
                            }
                        }
                    }
                }
            }
            //Matrix verkleinern (Reihen und Spalten entfernen, die leer sind)
            List<int> emptyCol = findEmptyColumns(matrix);
            matrix = reduceMatrix(matrix, emptyCol);

            List<int>[] solution = new List<int>[gesamtObst];
            try {
                Dictionary<string, string> qaArguments = new Dictionary<string, string>() {
                {"annealing_time","20"},
                {"num_reads","5000"}, //max 10000 (limitation by dwave)
                {"chain_strength","3" }
                };
                Dictionary<string, string> pyParams = new Dictionary<string, string>() {
                {"problem_type","qubo"}, //qubo //ising
                {"dwave_solver", "Advantage_system1.1"}, //DW_2000Q_6 //Advantage_system1.1
                {"dwave_inspector","false" }
                };
                Task<qaConstellation> constellationTask = QA_Communication.Program.qaCommunication(matrix, qaArguments, pyParams);
                qaConstellation constellation = constellationTask.Result;
                constellation.printConstellation(20);
                QA_Communication.Program.getUserInput(constellation, matrix);
                //constellation.plotEnergyDistribution();
                //constellation.saveInputData();
                //constellation.saveResults();
                (newSpieße, solution) = decodeQCResult(constellation, emptyCol, alphabet);

                (Spieß newWunschSpieß, List<(Spieß spieß, List<string> unpassendeSorten)> spießeHalbfalsch) = wunschspießZusammensetzen(newSpieße);

                Console.WriteLine("\nSOLUTION QANTUM");
                for (int s = 0; s < solution.Length; s++) {
                    Console.WriteLine(alphabet[s] + " : " + string.Join(",", solution[s]));
                }
                Console.WriteLine("\nWUNSCHSPIESS");
                newWunschSpieß.printSpieß();
                return (newWunschSpieß, newSpieße);

            }
            catch (Exception e) {
                Console.WriteLine("\nERROR occured:");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return (orgWunschSpieß, orgSpieße);
            }
        }

        List<int> findEmptyColumns(float[,] matrix) {
            List<int> emptyColumns = new List<int>();
            for (int i = 0; i < matrix.GetLength(0); i++) {
                bool columnEmpty = true;
                for (int j = 0; j < matrix.GetLength(1); j++) {
                    if (matrix[i, j] != 0) {
                        columnEmpty = false;
                        break;
                    }
                }
                if (columnEmpty) {
                    emptyColumns.Add(i);
                }
            }
            return emptyColumns;
        }

        float[,] reduceMatrix(float[,] matrix, List<int> emptyColumns) {
            int newLenght = matrix.GetLength(0) - emptyColumns.Count;
            float[,] newMatrix = new float[newLenght, newLenght];

            int countX = 0, countY = 0;
            for (int i = 0; i < matrix.GetLength(1); i++) {
                if (!emptyColumns.Contains(i)) {
                    for (int j = 0; j < matrix.GetLength(0); j++) {
                        if (!emptyColumns.Contains(j)) {
                            newMatrix[countX, countY] = matrix[i, j];
                            countX++;
                        }
                    }
                    countY++;
                    countX = 0;
                }
            }
            return newMatrix;
        }

        int[] expandResult(int[] result, List<int> emptyColumns) {
            int[] newResult = new int[result.Length + emptyColumns.Count];
            int newPosDiff = 0;
            for (int i = 0; i < newResult.Length; i++) {
                if (!emptyColumns.Contains(i)) {
                    newResult[i] = result[i - newPosDiff];
                }
                else { newResult[i] = 0; newPosDiff++; }
            }
            return newResult;
        }

        (List<Spieß> returnSpieße, List<int>[] solution) decodeQCResult(qaConstellation constellation, List<int> emptyCol, char[] alphabet) {
            List<int>[] solution = new List<int>[gesamtObst]; //index entspricht sorte, value entspricht schüssel
            for (int i = 0; i < solution.Length; i++) { solution[i] = new List<int>(); }
            List<Spieß> returnSpieße = new List<Spieß>();

            int[] resultCombined = new int[gesamtObst * gesamtObst];
            var bestResults = constellation.getLowest(1, new List<int>());

            //kombiniere beste Ergebnisse
            foreach (int index in bestResults) {
                int[] thisExpandedResult = expandResult(constellation.results[index].result, emptyCol);
                for (int i = 0; i < gesamtObst * gesamtObst; i++) {
                    resultCombined[i] += thisExpandedResult[i];
                }
            }
            //da unter den besten Ergebnissen oft ein paar falsche Zuordnungen vorkommen, sucht das Programm für 
            //jede Schüssel die Sorten raus, die am öftesten in allen besten Ergebnissen zugeordnet wurde (bzw deren Wert in resultCombined am höchsten ist)
            for (int sch = 0; sch < gesamtObst; sch++) {
                List<int> biggestSorNumIndices = new List<int>() { 0 };
                for (int sor = 0; sor < gesamtObst; sor++) {
                    if (resultCombined[sch * gesamtObst + sor] > resultCombined[sch * gesamtObst + biggestSorNumIndices[0]]) {
                        biggestSorNumIndices = new List<int>() { sor };
                    }
                    else if (resultCombined[sch * gesamtObst + sor] == resultCombined[sch * gesamtObst + biggestSorNumIndices[0]] && resultCombined[sch * gesamtObst + sor] > 0 && sor > 0) {
                        biggestSorNumIndices.Add(sor);
                    }
                }
                foreach (int sorte in biggestSorNumIndices) {
                    solution[sorte].Add(sch + 1);
                    returnSpieße.Add(new Spieß(new List<int>() { sch + 1 }, new List<string>() { alphabet[sorte] + "" }));
                }
            }
            return (returnSpieße, solution);
        }
    }

}
