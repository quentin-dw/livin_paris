using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Livin_paris_WinFormsApp.Outils;

namespace Livin_paris_WinFormsApp
{
    internal class Client
    {
        private int id_client;
        private bool entreprise;
        private string nom_entreprise;

        private int id_compte;
        private string prenom;
        private string nom;
        private string telephone;
        private string rue;
        private int numero;
        private int code_postal;
        private string ville;
        private string metro_le_plus_proche;
        private string email;
        private string mot_de_passe;

        private bool existe = false;

        public Client(int id_compte)
        {
            if(DQL_SQL($"SELECT EXISTS (SELECT * FROM compte WHERE id_compte = {id_compte})", false)[0][0] == "1")
            {
                this.existe = true;
                this.id_compte = id_compte;
            }

            string[] infosCompte = new string[10];
            infosCompte = DQL_SQL($"SELECT prenom, nom, telephone, rue, numero, code_postal, ville, metro_le_plus_proche, email, mot_de_passe FROM compte WHERE id_compte = {id_compte};", false)[0];

            this.prenom = infosCompte[0];
            this.nom = infosCompte[1];
            this.telephone = infosCompte[2];
            this.rue = infosCompte[3];
            this.numero = Convert.ToInt32(infosCompte[4]);
            this.code_postal = Convert.ToInt32(infosCompte[5]);
            this.ville = infosCompte[6];
            this.metro_le_plus_proche = infosCompte[7];
            this.email = infosCompte[8];
            this.mot_de_passe = infosCompte[9];

            string[] infosClient = new string[3];
            infosClient = DQL_SQL($"SELECT id_client, entreprise, nom_entreprise FROM client WHERE id_compte = {id_compte};", false)[0];
            this.id_client = Convert.ToInt32(infosClient[0]);
            this.entreprise = Convert.ToBoolean(infosClient[1]);
            this.nom_entreprise = infosClient[2];

        }


        public int Id_compte { get { return this.id_compte; } }
        public int Id_client { get { return this.id_client; } }
        public bool Entreprise { get { return this.entreprise; } }
        public string Nom_entreprise { get { return this.nom_entreprise; } }
        public string Prenom { get { return this.prenom; }}
        public string Nom { get { return this.nom; } }
        public string Telephone { get { return this.telephone; } }
        public string Metro_le_plus_proche { get { return this.metro_le_plus_proche; } }
        public string Email { get { return this.email; } }
        public string Mot_de_passe { get { return this.mot_de_passe; } }

        public bool Existe()
        {
            bool retour = true;
            if (!this.existe)
            {
                retour = false;
            }
            return retour;
        }

        public string Adresse()
        {
            return (this.numero+ ","+this.rue+","+this.ville+","+this.code_postal);
        }
    }
}
