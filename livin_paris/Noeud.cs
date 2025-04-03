using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace livin_paris
{
    internal class Noeud<T>
    {
        private T id;
        private string ligne;
        private string nom;
        private double latitude;
        private double longitude;
        private string nom_commune;
        private string code_insee;

        public Noeud(T id, string ligne, string nom, double latitude, double longitude, string nom_commune, string code_insee)
        {
            this.id = id;
            this.ligne = ligne;
            this.nom = nom;
            this.latitude = latitude;
            this.longitude = longitude;
            this.nom_commune = nom_commune;
            this.code_insee = code_insee;
        }

        public T Id
        {
            get { return id; }
        }

        public string Nom
        {
            get { return this.nom; }
        }


        /// <summary>
        /// Retourne une représentation sous forme de chaîne de caractères d'un noeud, comprenant son ID, son nom et sa ligne.
        /// </summary>
        /// <returns>
        /// Une chaîne de caractères représentant le noeud, sous la forme : "ID: [id], [nom] ligne [ligne]".
        /// </returns>
        public override string ToString()
        {
            return $"ID: {this.id}, {this.nom} ligne {this.ligne}";
        }


        /// <summary>
        /// Compare deux noeuds pour vérifier s'ils ont le meme ID
        /// </summary>
        /// <param name="n1">Le premier noeud à comparer</param>
        /// <param name="n2">Le second noeud à comparer</param>
        /// <returns>
        /// True si les deux noeuds ont le même ID, sinon False
        /// </returns>
        public static bool isEqual(Noeud<int> n1, Noeud<int> n2)
        {
            if (n1.Id == n2.Id)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// Vérifie si deux noeuds représentent la meme station, en comparant leur nom
        /// </summary>
        /// <param name="n1">Le premier noeud à comparer</param>
        /// <param name="n2">Le second noeud à comparer</param>
        /// <returns>
        /// True si les deux noeuds ont le meme nom, sinon False
        /// </returns>
        public static bool memeStation(Noeud<int> n1, Noeud<int> n2)
        {
            if (n1.Nom == n2.Nom)
            {
                return true;
            }

            return false;
        }

    }
}
