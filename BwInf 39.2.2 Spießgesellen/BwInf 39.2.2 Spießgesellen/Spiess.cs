using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BwInf_39_2_2_Spießgesellen {
    class Spieß {
        public int length;
        public List<int> schüsseln = new List<int>();
        public List<string> obstSorten = new List<string>();

        public Spieß(List<int> schüsseln, List<string> obstSorten) {
            this.schüsseln = schüsseln;
            this.obstSorten = obstSorten;
            length = obstSorten.Count();
        }

        public Spieß(List<string> schüsseln, List<string> obstSorten) {
            foreach (string schüssel in schüsseln) {
                this.schüsseln.Add(int.Parse(schüssel.Trim()));
            }
            this.obstSorten = obstSorten;
            length = obstSorten.Count();
        }

        public Spieß clone() {
            return new Spieß(schüsseln.ToList(), obstSorten.ToList());
        }
        /// <summary>
        /// compares 2 Spieße; erstellt neuen Spieß mit Schnittmenge von schüsseln und obstSorten; entfernt Schnittmenge aus Ursprungsspießen
        /// returns Tuple von verändertem spieß2 und dem neuen Schnittmengen-Spieß
        /// </summary>
        /// <param name="spieß2">Vergleichsspieß</param>
        /// <returns>Tuple von verändertem spieß2 und dem neuen Schnittmengen-Spieß</returns>
        public (Spieß spieß2neu,Spieß schnittSpieß) vergleicheSpieße(Spieß spieß2) {
            Spieß returnSpieß = new Spieß(new List<int>(),new List<string>());
            //für Laufzeitoptimierung
            /*List<int> fruitsToDelete1 = new List<int>();
            List<int> schüsselnToDelete1 = new List<int>();
            List<int> fruitsToDelete2 = new List<int>();
            List<int> schüsselnToDelete2 = new List<int>();*/
            for (int i = 0; i < length; i++) {
                for (int j = 0; j < spieß2.length; j++) {
                    if (schüsseln[i] == spieß2.schüsseln[j]) {
                        returnSpieß.schüsseln.Add(schüsseln[i]);
                        //schüsselnToDelete1.Add(i);Laufzeitoptimierung
                        //schüsselnToDelete2.Add(j);
                    }
                    if (obstSorten[i] == spieß2.obstSorten[j]) {
                        returnSpieß.obstSorten.Add(obstSorten[i]);
                        //fruitsToDelete1.Add(i);Laufzeitoptimierung
                        //fruitsToDelete2.Add(j);
                    }
                }
            }
            returnSpieß.updateLength();
            for(int i = 0; i < returnSpieß.length;i++) {
                schüsseln.Remove(returnSpieß.schüsseln[i]);
                spieß2.schüsseln.Remove(returnSpieß.schüsseln[i]);
                obstSorten.Remove(returnSpieß.obstSorten[i]);
                spieß2.obstSorten.Remove(returnSpieß.obstSorten[i]);
            }
            updateLength();
            spieß2.updateLength();
            return (spieß2, returnSpieß);
        }

        public void updateLength() {
            length = obstSorten.Count();
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
