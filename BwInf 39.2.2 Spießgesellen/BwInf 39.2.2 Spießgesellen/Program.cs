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
            var tup = readData(dataToLoad);
            List<Spieß> spieße = tup.Item2;
            Console.WriteLine("WUNSCHSORTEN:\n{0}", string.Join(", ", tup.Item1.obstSorten));
            Console.WriteLine("\nBEOBACHTETE SPIESSE:");
            foreach (Spieß spieß in spieße) {
                spieß.printSpieß();
            }
            Quantenannealer.quantenannealer(new Spieß(tup.Item1.schüsseln, tup.Item1.obstSorten), spieße, tup.Item3);
            tup = readData(dataToLoad);
            Algorithmus.algorithmus(new Spieß(tup.Item1.schüsseln,tup.Item1.obstSorten), spieße, tup.Item3);
            
            Console.ReadLine();
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
