using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace livin_paris
{
    internal class Lien
    {
        private Noeud Noeud_1;
        private Noeud Noeud_2;

        public Lien(Noeud noeud_1, Noeud noeud_2)
        {
            this.Noeud_1 = noeud_1;
            this.Noeud_2 = noeud_2;
        }
    }
}
