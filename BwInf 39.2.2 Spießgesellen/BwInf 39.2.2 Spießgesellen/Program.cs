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
            foreach(Spiess spiess in spiesse) {
                spiess.printSpiess();
            }
            Console.ReadLine();
        }

        /// <summary>
        /// reads data; returns Tuple von Wunschspiess und einer Liste von beobachteten Spiessen
        /// </summary>
        /// <param name="number">number of file (eg spiesse3.txt -> number=3)</param>
        /// <returns>Tuple von Wunschspiess und einer Liste von beobachteten Spiessen</returns>
        public static Tuple<Spiess,List<Spiess>> readData(int number) {
            string[] lines = System.IO.File.ReadAllLines(System.IO.Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName+"/spiesse"+number+".txt");
            Spiess wunschspiess = new Spiess(new List<int>(), lines[1].Trim().Split(' ').ToList());
            List<Spiess> spiesse = new List<Spiess>();
            for(int i = 3; i < int.Parse(lines[2])*2+3;i+=2) {
                spiesse.Add(new Spiess(lines[i].Trim().Split(' ').ToList(), lines[i + 1].Trim().Split(' ').ToList()));
            }
            return new Tuple<Spiess, List<Spiess>>(wunschspiess, spiesse);
        }
    }
}
