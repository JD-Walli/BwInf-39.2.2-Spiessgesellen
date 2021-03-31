using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BwInf_39_2_2_Spießgesellen {
    class Program {
        [STAThread]

        static void Main(string[] args) {
            int dataToLoad = 1;
            (Spieß wunschSpieß, List<Spieß> spieße, int gesamtObst) = readData(dataToLoad);
            Console.WriteLine("WUNSCHSORTEN:\n{0}", string.Join(", ", wunschSpieß.obstSorten));
            Console.WriteLine("\nBEOBACHTETE SPIESSE:");
            foreach (Spieß spieß in spieße) {
                spieß.printSpieß();
            }
            Console.WriteLine("\nQUANTENCOMPUTER:");
            //Quantenannealer.quantenannealer(new Spieß(wunschSpieß.schüsseln, wunschSpieß.obstSorten), spieße, gesamtObst);
            Console.WriteLine("\n\n\n");
            Console.WriteLine("\nALGORITHMUS:");
            (Spieß wunschSpieß2, List<Spieß> spieße2, int gesamtObst2) = readData(dataToLoad);
            Algorithmus.algorithmus1(new Spieß(wunschSpieß2.schüsseln,wunschSpieß2.obstSorten), spieße2, gesamtObst2);
            
            Console.ReadLine();
        }

        

        /// <summary>
        /// reads data; returns Tuple von Wunschspieß, Liste von beobachteten Spießen, Anzahl an obstSorten/schüsseln
        /// </summary>
        /// <param name="number">number of file (eg spieße3.txt -> number=3)</param>
        /// <returns>Tuple von Wunschspieß und einer Liste von beobachteten Spießen</returns>
        public static (Spieß wunschSpieß, List<Spieß> spieße, int gesamtObst) readData(int number) {
            string[] lines = System.IO.File.ReadAllLines(System.IO.Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "/spieße" + number + ".txt");
            Spieß wunschspieß = new Spieß(new List<int>(), lines[1].Trim().Split(' ').ToList());
            List<Spieß> spieße = new List<Spieß>();
            for (int i = 3; i < int.Parse(lines[2]) * 2 + 3; i += 2) {
                spieße.Add(new Spieß(lines[i].Trim().Split(' ').ToList(), lines[i + 1].Trim().Split(' ').ToList()));
            }
            return (wunschspieß, spieße, int.Parse(lines[0].Trim()));
        }
    }
}
