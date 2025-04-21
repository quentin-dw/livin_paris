using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Livin_paris_WinFormsApp.Outils;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;


namespace Livin_paris_WinFormsApp
{
    public class TableauCuisinier
    {
        public static void AffichageMenuCuisinier()
        {
            Cuisinier cuisinier = null;
            int choix = MenuCirculaire(4, "connexion", "associer un compte", "creer un compte", "trouver mon identifiant", "Menu Cuisinier");
            if (choix == -2)
            {
                return;
            }
            else if (choix == 0)
            {
                cuisinier = ConnexionCuisinier();
            }
            else if (choix == 1)
            {

            }
            else if (choix == 2)
            {
                cuisinier = CreationCompteCuisinier();
                if (cuisinier == null)
                {
                    return;
                }
            }
            else if (choix == 3)
            {
                TrouverIdentifiantCuisinier();
                return;
            }

        }

        static Cuisinier ConnexionCuisinier()
        {
            Console.ResetColor();
            Console.Clear();

            Console.SetCursorPosition(1, 0);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(" ▪ " + "Connexion ");
            Console.WriteLine();

            bool valide = false;
            string messageErreur = "";
            int id_compte = -1;

            while (!valide)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(messageErreur);

                Console.ResetColor();
                id_compte = Convert.ToInt32(Demander("Entrez votre numero d'identifiant de compte", "int", true));
                string idExists = DQL_SQL($"SELECT EXISTS (SELECT * FROM compte WHERE id_compte = {id_compte})", false)[0][0];


                if (idExists == "1")
                {
                    valide = true;
                }
                else
                {
                    messageErreur = " Identifiant invalide ";
                }
            }

            valide = false;

            Cuisinier cuisinier = new Cuisinier(id_compte);
            Console.WriteLine("Bienvenue " + cuisinier.Prenom + " " + cuisinier.Nom);

            while (!valide)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(messageErreur);

                Console.ResetColor();

                string mot_de_passe = Demander("Entrez votre mot de passe", "string", true);
                if (mot_de_passe == cuisinier.Mot_de_passe)
                {
                    valide = true;
                }
                else
                {
                    messageErreur = " Mot de passe invalide ";
                }
            }

            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine();
            Console.WriteLine(" Connexion réussie ✅ ");
            Thread.Sleep(1500);

            Console.ResetColor();

            return cuisinier;
        }

        static Cuisinier CreationCompteCuisinier()
        {
            Cuisinier cuisinier = null;

            Console.ResetColor();
            Console.Clear();

            Console.SetCursorPosition(1, 0);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(" ▪ " + "Creation compte ");
            Console.ResetColor();
            Console.WriteLine();


            string reponse = "";
            string  prenom = "", nom = "", telephone, rue, numero, code_postal, ville, metro_le_plus_proche, email, mot_de_passe;
            Console.WriteLine("Champs obligatoires marqués par *");

            prenom = Demander("Prénom", "string", true);

            nom = Demander("Nom", "string", true);

            telephone = Demander("Numéro de téléphone", "string", true);

            numero = Demander("Numéro de rue de résidence", "int", true);

            rue = Demander("Nom de rue de résidence", "string", true);

            ville = Demander("Ville de résidence", "string", true);

            code_postal = Demander("Code postal de ville de résidence", "int", true);

            metro_le_plus_proche = Demander("Station de metro la plus proche", "string", true);

            email = Demander("Adresse e-mail", "string", true);

            mot_de_passe = Demander("Mot de passe", "string", true);


            string requeteInsertCompte = $"INSERT INTO Compte (prenom, nom, telephone, rue, numero, code_postal, ville, metro_le_plus_proche, email, mot_de_passe) VALUES ('{prenom}', '{nom}', '{telephone}', '{rue}', {Convert.ToInt32(numero)}, {Convert.ToInt32(code_postal)}, '{ville}', '{metro_le_plus_proche}', '{email}', '{mot_de_passe}');";
            bool InsertCompte = DML_SQL(requeteInsertCompte);

            string requete = "SELECT LAST_INSERT_ID();";
            int id_compte = Convert.ToInt32(DQL_SQL(requete, false)[0][0]);

            string requeteInsertCuisinier = $"INSERT INTO Cuisinier (id_compte) VALUES ({id_compte});";
            InsertCompte = DML_SQL(requeteInsertCuisinier);

            if (InsertCompte)
            {
                cuisinier = new Cuisinier(id_compte);

                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine();
                Console.WriteLine(" Compte Cuisinier Créé ✅ ");

                Console.ResetColor();
                Console.WriteLine();

                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.Write(" Votre n° d'identifiant de compte : ");
                Console.BackgroundColor = ConsoleColor.Magenta;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(id_compte + " ");

                Console.ResetColor();
                Console.WriteLine("\n");

                Thread.Sleep(3000);
            }

            return cuisinier;
        }

        static void TrouverIdentifiantCuisinier()
        {
            Console.ResetColor();
            Console.Clear();

            Console.SetCursorPosition(1, 0);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(" ▪ " + "Trouver mon identifiant ");
            Console.ResetColor();
            Console.WriteLine();

            Console.WriteLine("Les moyens d'authentification disponible sont : ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" 1. ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("email");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" 2. ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("numero de telephone");
            Console.WriteLine();
            int modeAuth = Convert.ToInt32(Demander("Quel moyen d'authentification souhaitez vous utiliser ? [1/2]", "int", true));
            Console.WriteLine();

            if (modeAuth == 1)
            {
                string email = Demander("Entrez votre adresse e-mail", "string", true);
                Console.WriteLine();

                if (Convert.ToInt32(DQL_SQL($"SELECT EXISTS (SELECT * FROM compte WHERE email='{email}')", false)[0][0]) == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine("L'adresse email entrée est incorrecte, veuillez réessayer");
                    Thread.Sleep(3000);
                    return;
                }

                string[] retourRequete = DQL_SQL($"SELECT prenom, nom, id_compte FROM compte WHERE email='{email}'", false)[0];
                string prenom = retourRequete[0];
                string nom = retourRequete[1];
                string id_compte = retourRequete[2];

                Console.Write("Ce compte appartient à ");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(prenom + " " + nom);
                Console.ResetColor();

                Console.Write("Votre numéro d'identifiant de compte : ");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write(id_compte);
            }
            else if (modeAuth == 2)
            {
                string telephone = Demander("Entrez votre numero de telephone", "string", true);
                Console.WriteLine();

                if (Convert.ToInt32(DQL_SQL($"SELECT EXISTS (SELECT * FROM compte WHERE telephone='{telephone}')", false)[0][0]) == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine("Le numéro de téléphone entré est incorrecte, veuillez réessayer");
                    Thread.Sleep(3000);
                    return;
                }

                string[] retourRequete = DQL_SQL($"SELECT prenom, nom, id_compte FROM compte WHERE telephone='{telephone}'", false)[0];
                string prenom = retourRequete[0];
                string nom = retourRequete[1];
                string id_compte = retourRequete[2];

                Console.Write("Ce compte appartient à ");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(prenom + " " + nom);
                Console.ResetColor();

                Console.Write("Votre numéro d'identifiant de compte : ");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(id_compte);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nUne erreur s'est produite, veuillez reesayer");
            }
            Console.ResetColor();
            Console.WriteLine("\n\nPressez ENTREE pour sortir");
            Console.ReadLine();
        }
    }
}
