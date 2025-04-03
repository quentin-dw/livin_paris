﻿using System;
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

        public override string ToString()
        {
            return $"ID: {this.id}, Nom: {this.nom}";
        }

        //public bool isEqual(Noeud<T> n2)
        //{
        //    if (this.id == n2.id)
        //    {
        //        return true;
        //    }

        //    return false;
        //}

    }
}
