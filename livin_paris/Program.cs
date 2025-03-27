using System;
using System.IO;
using System.Collections.Generic;
using livin_paris;
using MySql.Data.MySqlClient;


namespace livin_paris
{
    internal class Program
    {
        static void Main()
        {

            Console.OutputEncoding = System.Text.Encoding.UTF8; // Active le support Unicode
                                                                //string filePath = "../../../../soc-karate.mtx";
                                                                //new Graphe(filePath);


            string connectionString = "Server=localhost;Database=psi;User ID=root;Password=root;SslMode=none;";
            MySqlConnection conn = ConnexionSQL(connectionString);

            AffichageMenu();

            Console.Read();
        }

        static void AffichageMenu()
        {
            bool entreeCorrecte = false;
            string messageErreur = "";
            while (!entreeCorrecte)
            {
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Bienvenue sur l'application Liv'in Paris ! \n\n\n");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(messageErreur);
                Console.ResetColor();

                Console.WriteLine("Interface Administrateur");
                Console.WriteLine("Selectionnez une option ci-dessous : ");
                Console.WriteLine("\t 1) Module client");
                Console.WriteLine("\t 2) Module cuisinier");
                Console.WriteLine("\t 3) Module commande");
                Console.WriteLine("\t 4) Module statistiques");
                Console.WriteLine("\t 5) Module autre");
                Console.Write("=> ");
                Console.ForegroundColor = ConsoleColor.Yellow;

                string reponse = Console.ReadLine();
                Console.ResetColor();
                switch (reponse)
                {
                    case "1":
                        entreeCorrecte=true;
                        ModuleClient();
                        break;
                    case "2":
                        entreeCorrecte = true;
                        //moduleClient();
                        break;
                    case "3":
                        entreeCorrecte = true;
                        //moduleClient();
                        break;
                    case "4":
                        entreeCorrecte = true;
                        //moduleClient();
                        break;
                    case "5":
                        entreeCorrecte = true;
                        //moduleClient();
                        break;
                    default:
                        messageErreur = "ERREUR : Entrée incorrecte !" ;
                        break;

                }
            }
        }

        static void ModuleClient()
        {
            Console.Clear();
            //Console.SetCursorPosition(10, 5); // Colonne 10, Ligne 5
            //Console.Write("Texte placé ici !");

            string libelle = "";
            int width = Console.WindowWidth;   // Nombre de colonnes (largeur)
            int height = Console.WindowHeight; // Nombre de lignes (hauteur)

            libelle = " AJOUTER CLIENT ";
            Console.SetCursorPosition(width/2 - (libelle.Length / 2), height/4);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(libelle);
            Console.ResetColor();

            libelle = " MODIFIER CLIENT ";
            Console.SetCursorPosition((width / 4)-(libelle.Length/2), height / 2);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(libelle);
            Console.ResetColor();

            libelle = " SUPPRIMER CLIENT⇧ ◄▲◣";
            Console.SetCursorPosition(3*(width / 4) - (libelle.Length / 2), height / 2);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(libelle);
            Console.ResetColor();

            Console.SetCursorPosition(width / 2, height / 2);

            bool entreeCorrecte = false;
            while (!entreeCorrecte)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true); 
                    if (keyInfo.Key == ConsoleKey.UpArrow)
                    {
                        entreeCorrecte = true;
                        AjouterClient();
                    }
                }

                Thread.Sleep(50);
            }

            Console.WriteLine("Programme terminé !");
            Console.WriteLine("  ▲\n" +
                "◢ ■ ◣");


            static void AjouterClient(){
                Console.Clear();
                Console.WriteLine("Ajout client\n");

                string reponse = "";
                string requeteInsert = "INSERT INTO Client (entreprise, prenom, nom, nom_etreprise) VALUES (";
                Console.WriteLine("Champs obligatoires marqués par *");
                Console.WriteLine("Ajoutez un espace pour ne pas répondre à une question\n");

                do
                {
                    Console.WriteLine("Ce client est il une entreprise ? [Oui/Non] *");
                    Console.Write("=> ");
                    reponse = Console.ReadLine();
                    if (reponse.ToLower() == "oui")
                    {
                        requeteInsert += "TRUE, ";

                        Console.WriteLine("\nPrénom référent *");
                        Console.Write("=> ");
                        requeteInsert += Console.ReadLine() + ", ";

                        Console.WriteLine("\nNom référent *");
                        Console.Write("=> ");
                        requeteInsert += Console.ReadLine() + ", ";

                        Console.WriteLine("\nNom de l'entreprise *");
                        Console.Write("=> ");
                        requeteInsert += Console.ReadLine();

                    }
                    else if (reponse.ToLower() == "non")
                    {
                        requeteInsert += "FALSE, ";

                        Console.WriteLine("\nPrénom *");
                        Console.Write("=> ");
                        requeteInsert += Console.ReadLine()+", ";

                        Console.WriteLine("\nNom *");
                        Console.Write("=> ");
                        requeteInsert += Console.ReadLine() + ", ";

                        requeteInsert += "NULL";
                    }

                    requeteInsert += ");";
                } while (reponse.ToLower() != "oui" && reponse.ToLower() != "non");
            }
        }
        
        static MySqlConnection ConnexionSQL(string connectionString)
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                Console.WriteLine("Connexion réussie !");

            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }

            return conn;
        }
    }
}
