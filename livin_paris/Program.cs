using System;
using System.IO;
using System.Collections.Generic;
using livin_paris;
using MySql.Data.MySqlClient;


namespace livin_paris
{
    internal class Program
    {
        static MySqlConnection connexion;
        static void Main()
        {

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            //string filePath = "../../../../soc-karate.mtx";
            //new Graphe(filePath);

            string connectionString = "Server=localhost;Database=psi;User ID=root;Password=root;SslMode=none;";
            connexion = ConnexionSQL(connectionString);

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
                        entreeCorrecte = true;
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
                        messageErreur = "ERREUR : Entrée incorrecte !";
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
            Console.SetCursorPosition((width / 2) - (libelle.Length / 2), height /4);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(libelle);
            Console.ResetColor();

            libelle = " MODIFIER CLIENT ";
            Console.SetCursorPosition((width / 4) - (libelle.Length / 2), height / 2);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(libelle);
            Console.ResetColor();

            libelle = " SUPPRIMER CLIENT ";
            Console.SetCursorPosition(3 * (width / 4) - (libelle.Length / 2), height /2);
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


            static void AjouterClient()
            {
                Console.Clear();
                Console.WriteLine("Ajout client\n");

                string reponse = "";
                string entreprise = "", prenom = "", nom = "", nom_entreprise, telephone, rue, numero, code_postal, ville, metro_le_plus_proche, email, mot_de_passe;
                Console.WriteLine("Champs obligatoires marqués par *");


                while (reponse.ToLower() != "oui" && reponse.ToLower() != "non")
                {
                    {
                        reponse = Demander("Ce client est il une entreprise ? [Oui/Non]", "string", true);
                        if (reponse.ToLower() == "oui")
                        {
                            entreprise = "TRUE";

                            prenom = Demander("Prénom référent", "string", true);

                            nom = Demander("Prénom référent", "string", true);

                            nom_entreprise = Demander("Nom de l'entreprise", "string", true);

                        }
                        else if (reponse.ToLower() == "non")
                        {
                            entreprise = "FALSE";

                            prenom = Demander("Prénom", "string", true);

                            nom = Demander("Prénom", "string", true);

                            nom_entreprise = "NULL";
                        }

                    }
                }

                
                telephone = Demander("Numéro de téléphone", "string", true);

                rue = Demander("Nom de rue de résidence", "string", true);

                numero = Demander("Numéro de rue de résidence", "int", true);

                code_postal = Demander("Code postal de résidence", "int", true);

                ville = Demander("Ville de résidence", "string", true);

                metro_le_plus_proche = Demander("Station de metro la plus proche", "string", true);

                email = Demander("Adresse e-mail", "string", true);

                mot_de_passe = Demander("Mot de passe", "string", true);


                string requeteInsert = $"INSERT INTO Compte (prenom, nom, telephone, rue, numero, code_postal, ville, metro_le_plus_proche, email, mot_de_passe) VALUES ('{prenom}', '{nom}', '{telephone}', '{rue}', {Convert.ToInt32(numero)}, {Convert.ToInt32(code_postal)}, '{ville}', '{metro_le_plus_proche}', '{email}', '{mot_de_passe}');";
                Console.WriteLine(requeteInsert);

                InsertSQL(requeteInsert);
            }
        }

        static string Demander(string question, string type, bool required)
        {
            string reponse = "";
            bool correcte = false;
            while (correcte == false)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write(" " + question + " ");

                if (required)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("* ");
                } else
                {
                    Console.WriteLine();
                }

                    Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("➜  ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                reponse = Console.ReadLine();
                Console.ResetColor();

                if (required && reponse != null)
                {
                    if (type == "string" && reponse.Trim() != "")
                    {
                        correcte = true;
                        reponse = reponse.ToLower().Trim();
                    }
                    else if (type == "int" && int.TryParse(reponse, out int n)) 
                    {
                        correcte = true;
                        reponse = reponse.Trim();
                    }
                }
                else if (!required)
                {
                    if (reponse == null)
                    {
                        correcte = true;
                    } else if (type == "int" && int.TryParse(reponse, out int n))
                    {
                        correcte = true;
                        reponse = reponse.Trim();
                    }
                }

                if (!correcte) { 
                    Console.Write("\t");
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(" Erreur : Format incorrect ! ");
                    Console.ResetColor();
                }

                Console.WriteLine();
            }
            return reponse;
        }

        static void InsertSQL(string req)
        {
            MySqlCommand command = connexion.CreateCommand();
            command.CommandText = req;
            try
            {
                command.ExecuteNonQuery();
            }
            catch (MySqlException e)
            {
                Console.WriteLine(" ErreurConnexion : " + e.ToString());
                Console.ReadLine();
                return;
            }

            command.Dispose();
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
