﻿using System;
using System.Collections.Generic;
using System.Linq;
using QA_Classification;
using System.Text;
using System.Threading.Tasks;


//TODO: bestrafen wenn zwei Felder nicht gleichzeitig besetzt sein dürfen/Zeilen und Spalten rauslöschen die nicht gebraucht werden: wo kein qubit, kann auch keine unerwünschte 1 sein
//TODO: Matrix tauschen: erst sorten, dann schüssel (macht decoding und andere einfacher zu verstehen)
namespace BwInf_39_2_2_Spießgesellen {
    class Quantenannealer {
        public static Tuple<Spieß, List<Spieß>> quantenannealer(Spieß wunschSpieß, List<Spieß> spieße, int gesamtObst) {
            char[] alphabetAllg = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            char[] alphabet = new char[gesamtObst];
            Array.Copy(alphabetAllg, alphabet, gesamtObst);
            float[,] matrix = new float[gesamtObst * gesamtObst, gesamtObst * gesamtObst];
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
                                matrix[x, x] = 1;
                            }
                        }
                    }
                }
            }

            List<Spieß> returnSpieße = new List<Spieß>();
            List<int>[] solution = new List<int>[gesamtObst]; //index entspricht sorte, value entspricht schüssel
            for(int i = 0; i < solution.Length; i++) {
                solution[i] = new List<int>();
            }
            Matrix.printMatrix(matrix);
            try {
                Dictionary<string, string> qaArguments = new Dictionary<string, string>() {
                {"annealing_time","2"},
                {"num_reads","4000"}, //max 10000 (limitation by dwave)
                {"chain_strength","3" }
                };
                Dictionary<string, string> pyParams = new Dictionary<string, string>() {
                {"problem_type","qubo"}, //qubo //ising
                {"dwave_solver", "Advantage_system1.1"}, //DW_2000Q_6 //Advantage_system1.1
                {"dwave_inspector","false" }
                };
                Task<qaConstellation> constellationTask = QA_Classification.Program.qaCommunication(matrix, qaArguments, pyParams);
                qaConstellation constellation = constellationTask.Result;
                constellation.printConstellation(20);
                QA_Classification.Program.getUserInput(constellation, matrix);
                //constellation.plotEnergyDistribution();
                //constellation.saveInputData();
                //constellation.saveResults();
                var resultCombined = new int[gesamtObst * gesamtObst];
                var bestResults= constellation.getLowest(1, new List<int>());

                foreach (int index in bestResults) {
                    for(int i = 0; i < gesamtObst * gesamtObst; i++) {
                        resultCombined[i] += constellation.results[index].Item4[i];
                    }
                }
                Console.WriteLine(String.Join(" ", resultCombined));
                for (int sch = 0; sch < gesamtObst; sch++) {
                    List<int> biggestSorNumIndices = new List<int>() { 0 };
                    for (int sor = 0; sor < gesamtObst; sor++) {
                        if (resultCombined[sch * gesamtObst + sor] > resultCombined[sch * gesamtObst + biggestSorNumIndices[0]]) {
                            biggestSorNumIndices=new List<int>() { sor };
                        } else if(resultCombined[sch * gesamtObst + sor] == resultCombined[sch * gesamtObst+biggestSorNumIndices[0]]  && resultCombined[sch * gesamtObst + sor]>0  &&  sor>0) {
                            biggestSorNumIndices.Add(sor);
                        }
                    }
                    foreach(int sorte in biggestSorNumIndices) {
                        Console.WriteLine("=> "+sorte);
                        solution[sorte].Add(sch + 1);
                        returnSpieße.Add(new Spieß(new List<int>() { sch+1 }, new List<string>() { alphabet[sorte] + "" }));
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
                else{
                    wunschSpieß.schüsseln.AddRange(solution[index]);
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
                Console.WriteLine(alphabet[s] + " : " + string.Join(",", solution[s]));
            }
            return new Tuple<Spieß, List<Spieß>>(wunschSpieß, returnSpieße);
        }
    }

}
