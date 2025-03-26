using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace livin_paris
{
    internal class Noeud
    {
        private int id;

        public Noeud(int id)
        {
            this.id = id;
        }

        public int Id
        {
            get { return id; }
        }

        public bool isEqual(Noeud n2)
        {
            if (this.id == n2.id)
            {
                return true;
            }

            return false;
        }

    }
}
