using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BwInf_39_2_2_Spießgesellen {
    class Spiess {
        int length;
        List<int> bowls = new List<int>();
        List<string> fruits = new List<string>();

        public Spiess(List<int> bowls, List<string> fruits) {
            this.bowls = bowls;
            this.fruits = fruits;
            length = fruits.Count();
        }

        public Spiess(List<string> bowls, List<string> fruits) {
            foreach (string bowl in bowls) {
                this.bowls.Add(int.Parse(bowl.Trim()));
            }
            this.fruits = fruits;
            length = fruits.Count();
        }

        /// <summary>
        /// compares 2 Spiesse; erstellt neuen Spiess mit Schnittmenge von bowls und fruits; entfernt Schnittmenge aus Ursprungsspiessen
        /// </summary>
        /// <param name="spiess2">Vergleichsspiess</param>
        /// <returns>Tuple von verändertem spiess2 und dem neuen Schnittmengen-Spiess</returns>
        public Tuple<Spiess,Spiess> compareSpiesse(Spiess spiess2) {
            Spiess returnSpiess = new Spiess(new List<int>(),new List<string>());
            //für Laufzeitoptimierung
            /*List<int> fruitsToDelete1 = new List<int>();
            List<int> bowlsToDelete1 = new List<int>();
            List<int> fruitsToDelete2 = new List<int>();
            List<int> bowlsToDelete2 = new List<int>();*/
            for (int i = 0; i < length; i++) {
                for (int j = 0; j < spiess2.length; j++) {
                    if (bowls[i] == spiess2.bowls[j]) {
                        returnSpiess.bowls.Add(bowls[i]);
                        //bowlsToDelete1.Add(i);Laufzeitoptimierung
                        //bowlsToDelete2.Add(j);
                    }
                    if (fruits[i] == spiess2.fruits[j]) {
                        returnSpiess.fruits.Add(fruits[i]);
                        //fruitsToDelete1.Add(i);Laufzeitoptimierung
                        //fruitsToDelete2.Add(j);
                    }
                }
            }
            returnSpiess.updateLength();
            for(int i = 0; i < returnSpiess.length;i++) {
                bowls.Remove(returnSpiess.bowls[i]);
                spiess2.bowls.Remove(returnSpiess.bowls[i]);
                fruits.Remove(returnSpiess.fruits[i]);
                spiess2.fruits.Remove(returnSpiess.fruits[i]);
            }
            return new Tuple<Spiess, Spiess>(spiess2, returnSpiess);
        }

        public void updateLength() {
            length = fruits.Count();
        }
        
        /// <summary>
        /// prints fruits -> bowls
        /// </summary>
        public void printSpiess() {
            string output = "";
            foreach (string fruit in fruits) {
                output += fruit + " ";
            }
            output += " -> ";
            foreach (int bowl in bowls) {
                output += bowl + " ";
            }
            Console.WriteLine(output);
        }
    }
}
