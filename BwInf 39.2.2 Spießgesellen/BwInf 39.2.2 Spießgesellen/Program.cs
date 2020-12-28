using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BwInf_39_2_2_Spießgesellen {
    class Program {
        static void Main(string[] args) {
            var tup = readData(1);
            List<Spiess> spiesse = tup.Item2;
            Console.WriteLine("Wunsch:\n{0}", string.Join(", ", tup.Item1.fruits));
            Console.WriteLine("Ausgangsdaten:");
            foreach (Spiess spiess in spiesse) {
                spiess.printSpiess();
            }
            algorithmus(tup.Item1, spiesse, tup.Item3);
            Console.ReadLine();
        }

        //for durch foreach ersetzen
        public static void algorithmus(Spiess wunschSpiess, List<Spiess> spiesse, int gesamtObst) {
            Boolean didChange = true;
            while (didChange) {
                didChange = false;
                for (int i = 0; i < spiesse.Count; i++) {
                    for (int j = i; j < spiesse.Count; j++) {
                        if (i != j) {
                            Tuple<Spiess, Spiess> tup1 = spiesse[i].compareSpiesse(spiesse[j]);
                            if (tup1.Item2.length > 0) {
                                spiesse[j] = tup1.Item1;
                                spiesse.Add(tup1.Item2);
                                didChange = true;
                            }
                        }
                    }
                }
            }
            spiesse.RemoveAll(sp => sp.length == 0);//laufzeitoptimierung?

            //Abfangen, dass unbeobachtete Obstsorten gewünscht werden
            //zu verbessern
            int beobachteteSorten = 0;
            foreach (Spiess sp in spiesse) {
                beobachteteSorten += sp.length;
            }
            if (beobachteteSorten != gesamtObst) {
                List<string> unbeobachteteSorten = new List<string>();
                for (int i = 0; i < wunschSpiess.length; i++) {
                    bool wunschObstBeobachtet = false;
                    for (int j = 0; j < spiesse.Count; j++) {
                        for (int k = 0; k < spiesse[j].length; k++) {
                            if (wunschSpiess.fruits[i] == spiesse[j].fruits[k]) {
                                wunschObstBeobachtet = true;
                            }
                        }
                    }
                    if (wunschObstBeobachtet == false) {
                        unbeobachteteSorten.Add(wunschSpiess.fruits[i]);
                    }
                }
                switch (unbeobachteteSorten.Count) {
                    case 0:
                    break;

                    case 1:
                    int unbeobachtetBowlNum = ((gesamtObst)*(gesamtObst+1))/2; //Summe der Zahlen 1 bis n (alle bowl-nummern addiert)
                    unbeobachtetBowlNum -= spiesse.Sum(sp => sp.bowls.Sum());
                    spiesse.Add(new Spiess(new List<int>(unbeobachtetBowlNum), unbeobachteteSorten));
                    break;

                    default:
                    Console.WriteLine("{0} unbeobachtete Obstsorten stehen auf der Wunschliste:", unbeobachteteSorten.Count);
                    foreach (string fruit in unbeobachteteSorten) {
                        Console.WriteLine("- " + fruit);
                    }
                    break;
                }
            }

            Console.WriteLine("\nSPIESSE:");
            foreach (Spiess spiess in spiesse) {
                spiess.printSpiess();
            }

        }

        /// <summary>
        /// reads data; returns Tuple von Wunschspiess, Liste von beobachteten Spiessen, Anzahl an fruits/bowls
        /// </summary>
        /// <param name="number">number of file (eg spiesse3.txt -> number=3)</param>
        /// <returns>Tuple von Wunschspiess und einer Liste von beobachteten Spiessen</returns>
        public static Tuple<Spiess, List<Spiess>, int> readData(int number) {
            string[] lines = System.IO.File.ReadAllLines(System.IO.Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "/spiesse" + number + ".txt");
            Spiess wunschspiess = new Spiess(new List<int>(), lines[1].Trim().Split(' ').ToList());
            List<Spiess> spiesse = new List<Spiess>();
            for (int i = 3; i < int.Parse(lines[2]) * 2 + 3; i += 2) {
                spiesse.Add(new Spiess(lines[i].Trim().Split(' ').ToList(), lines[i + 1].Trim().Split(' ').ToList()));
            }
            return new Tuple<Spiess, List<Spiess>, int>(wunschspiess, spiesse, int.Parse(lines[0].Trim()));
        }
    }
}
