using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BwInf_39_2_2_Spießgesellen {
    class Spiess {
        int length;
        List<int> bowls= new List<int>();
        List<string> fruits = new List<string>();

        public Spiess(List<int> bowls, List<string> fruits) {
            this.bowls = bowls;
            this.fruits = fruits;
            length = fruits.Count();
        }

        public Spiess(List<string> bowls, List<string> fruits) {
            foreach(string bowl in bowls) {
                this.bowls.Add(int.Parse(bowl.Trim()));
            }
            this.fruits = fruits;
            length = fruits.Count();
        }

        public void printSpiess() {
            string output= "";
            foreach(string fruit in fruits) {
                output += fruit + " ";
            }
            output += " -> ";
            foreach(int bowl in bowls) {
                output += bowl + " ";
            }
            Console.WriteLine(output);
        }
    }
}
