using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Livin_paris_WinFormsApp
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

        public double Latitude
        {
            get { return this.latitude; }
        }

        public double Longitude
        {
            get { return this.longitude; }
        }

        public override string ToString()
        {
            return $"ID: {this.id}, Nom: {this.nom}";
        }
    }
}
