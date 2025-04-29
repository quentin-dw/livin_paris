using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Livin_paris_WinFormsApp
{
    public class Compte<T>
    {
        private T id;
        private string type;    // cuisinier ou client

        public T Id { get { return id; } }

        public string Type { get { return type; } }

        public Compte(T id, string type)
        {
            this.id = id;
            this.type = type;
        }

        public Compte() { } // pour pouvoir serialiser pour l'exportation xml

        /// <summary>
        /// Compare l'égalité entre le compte actuel et un autre compte/>.
        /// </summary>
        /// <param name="other">
        /// Un autre objet de type Compte<T> à comparer avec le compte actuel
        /// </param>
        /// <returns>
        /// Retourne true si les comptes sont égaux, sinon false. Deux comptes sont considérés égaux si leur identifiant (Id) est identique.
        /// </returns>
        public bool Equals(Compte<T> other)
        {
            if (other == null) return false;
            return Id.Equals(other.Id);
        }

        /// <summary>
        /// Retourne une représentation sous forme de string du compte
        /// </summary>
        /// <returns>
        /// Retourne l'identifiant (Id) de l'objet sous forme de string
        /// </returns>
        public override string ToString()
        {
            return Id.ToString();
        }
    }
}
