using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BwInf_39_2_2_Spießgesellen {
    class Program {
        static void Main(string[] args) {
            var tup = readData(1);
            List<Spieß> spieße = tup.Item2;
            Console.WriteLine("Wunsch:\n{0}", string.Join(", ", tup.Item1.obstSorten));
            Console.WriteLine("Ausgangsdaten:");
            foreach (Spieß spieß in spieße) {
                spieß.printSpieß();
            }
            algorithmus(tup.Item1, spieße, tup.Item3);
            Console.ReadLine();
        }

        //for durch foreach ersetzen
        public static Tuple<Spieß,List<Spieß>> algorithmus(Spieß wunschSpieß, List<Spieß> spieße, int gesamtObst) {
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
            int beobachteteSorten = 0;
            foreach (Spieß sp in spieße) {
                beobachteteSorten += sp.length;
            }
            if (beobachteteSorten != gesamtObst) {
                List<string> unbeobachteteSorten = new List<string>();
                foreach (string wunschObst in wunschSpieß.obstSorten) {
                    bool wunschObstBeobachtet = false;
                    foreach (Spieß sp in spieße) {
                        if (sp.obstSorten.Contains(wunschObst)) {
                            wunschObstBeobachtet = true;
                        }
                    }
                    if (wunschObstBeobachtet == false) {
                        unbeobachteteSorten.Add(wunschObst);
                    }
                }
                switch (unbeobachteteSorten.Count) {
                    case 0:
                    break;

                    case 1:
                    int unbeobachteteSchüsselNum = ((gesamtObst) * (gesamtObst + 1)) / 2; //Summe der Zahlen 1 bis n (alle schüssel-nummern addiert)
                    unbeobachteteSchüsselNum -= spieße.Sum(sp => sp.schüsseln.Sum());
                    spieße.Add(new Spieß(new List<int>() { unbeobachteteSchüsselNum }, unbeobachteteSorten));
                    break;

                    default:
                    Console.WriteLine("{0} unbeobachtete Obstsorten stehen auf der Wunschliste:", unbeobachteteSorten.Count);
                    foreach (string obst in unbeobachteteSorten) {
                        Console.WriteLine("- " + obst);
                    }
                    break;
                }
            }
            #endregion

            #region Wunschspieß berechnen
            //wenn ganzer Spieß in wunschSpieß enthalten ist, werden Spieß.schüsseln zu wunschSpieß.schüsseln hinzugefügt
            foreach (Spieß spieß in spieße) {
                bool ganzerSpießGewunscht = true;
                foreach (string obst in spieß.obstSorten) {
                    if (!wunschSpieß.obstSorten.Contains(obst)) {
                        ganzerSpießGewunscht = false;
                    }
                }
                if (ganzerSpießGewunscht) {
                    wunschSpieß.schüsseln.AddRange(spieß.schüsseln);
                }
            }
            #endregion

            Console.WriteLine("\nSPIESSE:");
            foreach (Spieß spieß in spieße) {
                spieß.printSpieß();
            }
            Console.WriteLine("\nWUNSCHSPIESS:");
            wunschSpieß.printSpieß();

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
