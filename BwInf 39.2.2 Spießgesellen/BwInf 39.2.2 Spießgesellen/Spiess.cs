using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BwInf_39_2_2_Spießgesellen {
    class Spieß {
        public int länge;
        public List<int> schüsseln = new List<int>();
        public List<string> obstSorten = new List<string>();

        public Spieß(List<int> schüsseln, List<string> obstSorten) {
            this.schüsseln = schüsseln;
            this.obstSorten = obstSorten;
            länge = obstSorten.Count();
        }

        public Spieß(List<string> schüsseln, List<string> obstSorten) {
            foreach (string schüssel in schüsseln) {
                this.schüsseln.Add(int.Parse(schüssel.Trim()));
            }
            this.obstSorten = obstSorten;
            länge = obstSorten.Count();
        }

        public Spieß clone() {
            return new Spieß(schüsseln.ToList(), obstSorten.ToList());
        }
        /// <summary>
        /// vergleicht 2 Spieße; erstellt neuen Spieß mit Schnittmenge von schüsseln und obstSorten; entfernt Schnittmenge aus beiden Ursprungsspießen
        /// returns Tuple von verändertem spieß2 und dem neuen Schnittmengen-Spieß
        /// </summary>
        /// <param name="spieß2">Vergleichsspieß</param>
        /// <returns>Tuple von verändertem spieß2 und dem neuen Schnittmengen-Spieß</returns>
        public (Spieß spieß2neu,Spieß schnittSpieß) vergleicheSpieße(Spieß spieß2) {
            Spieß schnittSpieß = new Spieß(new List<int>(),new List<string>());

            for (int i = 0; i < länge; i++) {
                for (int j = 0; j < spieß2.länge; j++) {
                    if (schüsseln[i] == spieß2.schüsseln[j]) {
                        schnittSpieß.schüsseln.Add(schüsseln[i]);
                    }
                    if (obstSorten[i] == spieß2.obstSorten[j]) {
                        schnittSpieß.obstSorten.Add(obstSorten[i]);
                    }
                }
            }
            schnittSpieß.updateLänge();
            for(int i = 0; i < schnittSpieß.länge;i++) {
                schüsseln.Remove(schnittSpieß.schüsseln[i]);
                spieß2.schüsseln.Remove(schnittSpieß.schüsseln[i]);
                obstSorten.Remove(schnittSpieß.obstSorten[i]);
                spieß2.obstSorten.Remove(schnittSpieß.obstSorten[i]);
            }
            updateLänge();
            spieß2.updateLänge();
            return (spieß2, schnittSpieß);
        }

        /// <summary>
        /// setzt länge auf obstsorten.Count() und überprüft ob sorten.Count==schüsseln.Count
        /// </summary>
        public void updateLänge() {
            länge = obstSorten.Count();
            if (schüsseln.Count != obstSorten.Count) {
                Console.WriteLine("invalid data (amount of overlap between two Spieße not equal in schüsseln and obstSorten). overlap: ");
                printSpieß();
                Console.WriteLine("exit...");
                Console.ReadKey();
            }
        }
        
        /// <summary>
        /// prints obstSorten -> schüsseln
        /// </summary>
        public void printSpieß() {
            string output = "";
            foreach (string obst in obstSorten) {
                output += obst + " ";
            }
            output += " -> ";
            foreach (int schüssel in schüsseln) {
                output += schüssel + " ";
            }
            Console.WriteLine(output);
        }
    }
}
