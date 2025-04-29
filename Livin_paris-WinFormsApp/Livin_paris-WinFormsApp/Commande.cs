using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Livin_paris_WinFormsApp
{
    public class Commande<T>
    {
        private T id;
        private string type;    // cuisinier ou client

        public T Id { get { return id; } }

        public string Type { get { return type; } }

        public Commande(T id, string type)
        {
            this.id = id;
            this.type = type;
        }
    }
}
