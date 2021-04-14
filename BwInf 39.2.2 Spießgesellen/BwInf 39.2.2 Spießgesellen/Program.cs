using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BwInf_39_2_2_Spießgesellen {
    class Program {
        [STAThread]

        static void Main(string[] args) {
            int datenSet = 1;
            (Spieß wunschSpieß, List<Spieß> spieße, int gesamtObst) = readData(datenSet);
            basisAlgorithmus algo = new basisAlgorithmus(wunschSpieß, spieße, gesamtObst);

            if (!validiereDaten(wunschSpieß, spieße)) { return; }

            Console.WriteLine("WUNSCHSORTEN:\n{0}", string.Join(", ", wunschSpieß.obstSorten));
            Console.WriteLine("\nBEOBACHTETE SPIESSE:");
            foreach (Spieß spieß in spieße) {
                spieß.printSpieß();
            }
            
            Console.WriteLine("\nALGORITHMUS 1:");
            new Algorithmus(wunschSpieß, spieße, gesamtObst).algorithmus1();
            Console.WriteLine("\n\n\n");

            Console.WriteLine("\nALGORITHMUS 2:");
            //new Algorithmus(wunschSpieß, spieße, gesamtObst).algorithmus2();
            Console.WriteLine("\n\n\n");

            Console.WriteLine("\nQUANTENCOMPUTER:");
            //new Quantenannealer(wunschSpieß, spieße, gesamtObst).quantenannealer();

            Console.ReadLine();
        }

        

        /// <summary>
        /// reads data; returns Tuple von Wunschspieß, Liste von beobachteten Spießen, Anzahl an obstSorten/schüsseln
        /// </summary>
        /// <param name="datenSet">number of file (eg spieße3.txt -> number=3)</param>
        /// <returns>Tuple von Wunschspieß und einer Liste von beobachteten Spießen</returns>
        public static (Spieß wunschSpieß, List<Spieß> spieße, int gesamtObst) readData(int datenSet) {
            string[] lines = System.IO.File.ReadAllLines(System.IO.Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "/spieße" + datenSet + ".txt");
            Spieß wunschspieß = new Spieß(new List<int>(), lines[1].Trim().Split(' ').ToList());
            List<Spieß> spieße = new List<Spieß>();
            for (int i = 3; i < int.Parse(lines[2]) * 2 + 3; i += 2) {
                spieße.Add(new Spieß(lines[i].Trim().Split(' ').ToList(), lines[i + 1].Trim().Split(' ').ToList()));
            }
            return (wunschspieß, spieße, int.Parse(lines[0].Trim()));
        }

        public static bool validiereDaten(Spieß wunschSpieß, List<Spieß> spieße) {
            bool valide = true;
            foreach (Spieß sp in spieße) {
                if (sp.schüsseln.Count != sp.obstSorten.Count) {
                    valide = false;
                    Console.WriteLine("invalid data (schüsseln.Count!=obstSorten.Count):");
                    sp.printSpieß();
                }
            }
            foreach (Spieß sp in spieße) {
                for (int i = 0; i < sp.schüsseln.Count; i++) {
                    for (int j = i; j < sp.schüsseln.Count; j++) {
                        if (i != j) {
                            if (sp.schüsseln[i] == sp.schüsseln[j]) {
                                Console.WriteLine("invalid data (same value multiple times):");
                                valide = false;
                                sp.printSpieß();
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < wunschSpieß.obstSorten.Count; i++) {
                for (int j = 0; j < wunschSpieß.obstSorten.Count; j++) {
                    if (i != j) {
                        if (wunschSpieß.obstSorten[i] == wunschSpieß.obstSorten[j]) {
                            Console.WriteLine("invalid data (wunschSpieß same obstsorte multiple times):");
                            valide = false;
                            wunschSpieß.printSpieß();
                        }
                    }
                }
            }
            if (valide) {
                Console.WriteLine("data is valid!");
                return true;
            }
            else {
                Console.Write("proceed (p) or exit (e) program?");
                var key = Console.ReadKey();
                if (key.Key.ToString() == "P") {
                    return true;
                }
                Console.WriteLine("\n\n");
            }
            return false;
        }
    }
}
