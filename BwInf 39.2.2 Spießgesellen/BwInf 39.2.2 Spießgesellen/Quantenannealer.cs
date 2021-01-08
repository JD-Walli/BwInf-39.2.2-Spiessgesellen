using System;
using System.Collections.Generic;
using System.Linq;
using QA_Classification;
using System.Text;
using System.Threading.Tasks;

namespace BwInf_39_2_2_Spießgesellen {
    class Quantenannealer {
        public static Tuple<Spieß, List<Spieß>> quantenannealer(Spieß wunschSpieß, List<Spieß> spieße, int gesamtObst) {
            //TODO: nicht immer sind alle Buchstaben des Alphabets vertreten!!!
            char[] alphabetAllg = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            char[] alphabet = new char[gesamtObst];
            Array.Copy(alphabetAllg, alphabet, gesamtObst);
            float[,] matrix = new float[gesamtObst * gesamtObst, gesamtObst * gesamtObst];

            //O(n)= spießeC*gesamtobst³
            /*Fehlerbild:
             nicht alle Schleifen werden vollständig durchlaufen
             ergo nicht genug -2 werden eingetragen -> gelöst, nicht valuetype übergabe in program an algo und qa
             die results sind nur 600 stellen lang, sollten 100 sein
             falsche Ergebnisse
             -> Idee: QA zerstört Qubits die nicht gebiased und gekoppelt sind, die ich aber zum decoden brauche (als abstandshalter)*/
            foreach (Spieß spieß in spieße) {
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
                            else if (matrix[x, x] == 0) {
                                matrix[x, x] = 2;
                            }
                        }
                    }
                }
            }

            List<Spieß> returnSpieße = new List<Spieß>();
            int[] solution = new int[gesamtObst]; //index entspricht sorte, value entspricht schüssel
            for (int i = 0; i < solution.Length; i++) {
                solution[i] = -1;
            }
            Matrix.printMatrix(matrix);
            try {
                Dictionary<string, string> qaArguments = new Dictionary<string, string>() {
                {"annealing_time","200"},
                {"num_reads","4000"}, //max 10000 (limitation by dwave)
                //{"chain_strength","2" }
                };
                Dictionary<string, string> pyParams = new Dictionary<string, string>() {
                {"problem_type","qubo"}, //qubo //ising
                {"dwave_solver", "Advantage_system1.1"}, //DW_2000Q_6 //Advantage_system1.1
                {"dwave_inspector","true" }
                };
                Task<qaConstellation> constellationTask = QA_Classification.Program.qaCommunication(matrix, qaArguments, pyParams);
                qaConstellation constellation = constellationTask.Result;
                constellation.printConstellation(20);
                QA_Classification.Program.getUserInput(constellation, matrix);
                //constellation.plotEnergyDistribution();
                //constellation.saveInputData();
                //constellation.saveResults();
                var result = constellation.results[constellation.getHighest(2, constellation.getLowest(1, new List<int>()))[0]];
                /*
                 neue Spießliste
                 gehe durch alle obstsorten
                 finde alle unterschiede aller gleich guter results, die innerhalb einer sorte sind
                 -  neues Array kombinierteresults 000000
                 -  gehe durch alle results    
                 -       gehe durch jede schüssel und wenn wert==1 setze wert in kombinierteresults auch auf 1
                 - return Array mit array[position]==1? schüssel[position] gehört zu betrachteter Obstsorte
                 füge sorte und ALLE möglicherweise dazugehörigen schüsseln (aka return) hinzu 
                 dadurch kann auch die info einer nicht-eindeutig-zugeordneten Sorte bekommen werden*/

                for (int sch = 0; sch < gesamtObst; sch++) {
                    for (int sor = 0; sor < gesamtObst; sor++) {
                        if (result.Item4[sch * gesamtObst + sor] == 1) {
                            solution[sor] = sch + 1;
                            returnSpieße.Add(new Spieß(new List<int>() { sch }, new List<string>() { alphabet[sor] + "" }));
                        }
                    }
                }
            }
            catch (Exception e) {
                Console.WriteLine("\nERROR occured:");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

            foreach (string wunschobst in wunschSpieß.obstSorten) {
                int index = Array.FindIndex(alphabet, c => c == wunschobst.ToLower().ElementAt(0));
                if (index == -1) { Console.WriteLine("Es konnte nicht bestimmt werden in welcher Schüssel sich das Obst {0} befindet", wunschobst); }
                else {
                    wunschSpieß.schüsseln.Add(solution[index]);
                }
            }
            Console.WriteLine("\nERGEBNIS");
            foreach (Spieß spieß in returnSpieße) {
                spieß.printSpieß();
            }
            Console.WriteLine("\nWUNSCHSPIESS");
            wunschSpieß.printSpieß();
            Console.WriteLine("\nSOLUTION QANTUM");
            for (int s = 0; s < solution.Length; s++) {
                Console.WriteLine(alphabet[s] + " : " + solution[s]);
            }
            return new Tuple<Spieß, List<Spieß>>(wunschSpieß, returnSpieße);
        }
    }

}
