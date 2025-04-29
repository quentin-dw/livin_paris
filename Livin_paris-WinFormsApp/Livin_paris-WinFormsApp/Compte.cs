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
        public override bool Equals(object obj)
        {
            return obj is Compte<T> other && EqualityComparer<T>.Default.Equals(Id, other.Id);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<T>.Default.GetHashCode(Id);
        }

        public override string ToString()
        {
            return Id.ToString();
        }
    }
}
