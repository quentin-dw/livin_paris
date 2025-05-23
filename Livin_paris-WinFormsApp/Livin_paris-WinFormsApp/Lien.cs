﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Livin_paris_WinFormsApp
{
    public class Lien<T>
    {
        private Noeud<T> noeud_1;
        private Noeud<T> noeud_2;
        private int temps_station_suivante;
        private int temps_changement;

        public Lien(Noeud<T> noeud_1, Noeud<T> noeud_2, int temps_station_suivante)
        {
            this.noeud_1 = noeud_1;
            this.noeud_2 = noeud_2;
            this.temps_station_suivante = temps_station_suivante;
        }

        public Noeud<T> Noeud1
        {
            get { return this.noeud_1; }
        }

        public Noeud<T> Noeud2
        {
            get { return this.noeud_2; }
        }

        /// <summary>
        /// Retourne une représentation sous forme de chaîne de caractères du noeud de depart, d'arrivee et de la durée
        /// </summary>
        /// <returns>
        /// Une chaîne de caractères représentant le noeud, sous la forme : "{nom station depart} -> {nom station arrivee} : {duree} min"
        /// </returns>
        public string toString()
        {
            return $"{noeud_1.Nom} -> {noeud_2.Nom} : {temps_station_suivante} min";
        }
    }
}
