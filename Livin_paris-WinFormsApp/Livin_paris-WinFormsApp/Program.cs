using System;
using System.IO;
using System.Collections.Generic;
using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using MySql.Data.MySqlClient;
using System.Data;
using ZstdSharp.Unsafe;



namespace Livin_paris_WinFormsApp
{


    public class Program
    {


        public static MySqlConnection connexion;
        [DllImport("kernel32.dll")]
        static extern bool AllocConsole();
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AllocConsole();
            Console.WriteLine("Console attachée !");

            string noeuds = "../../../../../noeuds.csv";
            string arcs = "../../../../../arcs.csv";
            new Graphe<int>(noeuds, arcs);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1("../../../../../noeuds.csv", "../../../../../arcs.csv"));

            Console.OutputEncoding = System.Text.Encoding.UTF8;

            string connectionString = "Server=localhost;Database=psi_demougeot_dehecohen_dewolf;User ID=root;Password=root;SslMode=none;";
            connexion = ConnexionSQL(connectionString);

            int width = Console.WindowWidth;
            int height = Console.WindowHeight;

            string[] fondAccueil = ["░", "▒", "▓"];
            ConsoleColor[] couleurAccueil = [ConsoleColor.Cyan, ConsoleColor.Green, ConsoleColor.Yellow, ConsoleColor.Red, ConsoleColor.Magenta];

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < width; j = j + 2)
                {
                    Console.ForegroundColor = couleurAccueil[i];
                    for (int k = 0; k < height; k++)
                    {
                        Console.SetCursorPosition(j, k);
                        Console.Write(fondAccueil[(i + j + k) % 3]);
                        Console.Write(fondAccueil[(i + j + k) % 3]);
                    }
                }
                Thread.Sleep(5);
            }

            AffichageMenuPrincipal();


        }

        static void AffichageMenuPrincipal()
        {
            AffichageLivinParis();
            MenuAdminSelected();

            bool finProgramme = false;
            while (!finProgramme)
            {
                bool entreeCorrecte = false;
                int menuSelected = 0;
                while (!entreeCorrecte)
                {
                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                        if (keyInfo.Key == ConsoleKey.LeftArrow && menuSelected > 0)
                        {
                            menuSelected--;
                        }
                        else if (keyInfo.Key == ConsoleKey.RightArrow && menuSelected < 2)
                        {
                            menuSelected++;
                        }
                        else if (keyInfo.Key == ConsoleKey.Enter)
                        {
                            switch (menuSelected)
                            {
                                case 0 :
                                    TableauAdmin.AffichageMenuAdmin();
                                    entreeCorrecte = true;
                                    break;
                                case 1 :
                                    entreeCorrecte = true;
                                    break;
                                case 2 :
                                    entreeCorrecte = true;
                                    break;
                            }
                        }
                        else if (keyInfo.Key == ConsoleKey.NumPad1 || keyInfo.Key == ConsoleKey.D1)
                        {
                            TableauAdmin.AffichageMenuAdmin();
                            entreeCorrecte = true;
                        }
                        else if (keyInfo.Key == ConsoleKey.NumPad2 || keyInfo.Key == ConsoleKey.D2)
                        {
                            AffichageMenuClient();
                            entreeCorrecte = true;
                        }
                        else if (keyInfo.Key == ConsoleKey.NumPad3 || keyInfo.Key == ConsoleKey.D3)
                        {
                            entreeCorrecte = true;
                        }
                        else if (keyInfo.Key == ConsoleKey.Escape)
                        {
                            entreeCorrecte = true;
                            finProgramme = true;
                        }

                        switch (menuSelected)
                        {
                            case 0:
                                AffichageLivinParis();
                                MenuAdminSelected();
                                break;
                            case 1:
                                AffichageLivinParis();
                                MenuClientSelected();
                                break;
                            case 2:
                                AffichageLivinParis();
                                MenuCuisinierSelected();
                                break;
                        }

                    }
                }

            }

            static void AffichageLivinParis()
            {
                Console.ResetColor();
                Console.Clear();

                int width = Console.WindowWidth;
                int height = Console.WindowHeight;

                string[] asciiBienvenue = new string[] {"██╗░░░░░██╗██╗░░░██╗██╗██╗███╗░░██╗  ██████╗░░█████╗░██████╗░██╗░██████╗",
                "██║░░░░░██║██║░░░██║╚█║██║████╗░██║  ██╔══██╗██╔══██╗██╔══██╗██║██╔════╝",
                "██║░░░░░██║╚██╗░██╔╝░╚╝██║██╔██╗██║  ██████╔╝███████║██████╔╝██║╚█████╗░",
                "██║░░░░░██║░╚████╔╝░░░░██║██║╚████║  ██╔═══╝░██╔══██║██╔══██╗██║░╚═══██╗",
                "███████╗██║░░╚██╔╝░░░░░██║██║░╚███║  ██║░░░░░██║░░██║██║░░██║██║██████╔╝",
                "╚══════╝╚═╝░░░╚═╝░░░░░░╚═╝╚═╝░░╚══╝  ╚═╝░░░░░╚═╝░░╚═╝╚═╝░░╚═╝╚═╝╚═════╝░"
                };

                //string textBienvenue = "\r\n ██▓     ██▓ ██▒   █▓ ██▓ ███▄    █     ██▓███   ▄▄▄       ██▀███   ██▓  ██████ \r\n▓██▒    ▓██▒▓██░   █▒▓██▒ ██ ▀█   █    ▓██░  ██▒▒████▄    ▓██ ▒ ██▒▓██▒▒██    ▒ \r\n▒██░    ▒██▒ ▓██  █▒░▒██▒▓██  ▀█ ██▒   ▓██░ ██▓▒▒██  ▀█▄  ▓██ ░▄█ ▒▒██▒░ ▓██▄   \r\n▒██░    ░██░  ▒██ █░░░██░▓██▒  ▐▌██▒   ▒██▄█▓▒ ▒░██▄▄▄▄██ ▒██▀▀█▄  ░██░  ▒   ██▒\r\n░██████▒░██░   ▒▀█░  ░██░▒██░   ▓██░   ▒██▒ ░  ░ ▓█   ▓██▒░██▓ ▒██▒░██░▒██████▒▒\r\n░ ▒░▓  ░░▓     ░ ▐░  ░▓  ░ ▒░   ▒ ▒    ▒▓▒░ ░  ░ ▒▒   ▓▒█░░ ▒▓ ░▒▓░░▓  ▒ ▒▓▒ ▒ ░\r\n░ ░ ▒  ░ ▒ ░   ░ ░░   ▒ ░░ ░░   ░ ▒░   ░▒ ░       ▒   ▒▒ ░  ░▒ ░ ▒░ ▒ ░░ ░▒  ░ ░\r\n  ░ ░    ▒ ░     ░░   ▒ ░   ░   ░ ░    ░░         ░   ▒     ░░   ░  ▒ ░░  ░  ░  \r\n    ░  ░ ░        ░   ░           ░                   ░  ░   ░      ░        ░  \r\n                 ░                                                              \r\n";
                //string textBienvenue = "\r\n   __ _       _           ___           _     \r\n  / /(_)_   _(_)_ __     / _ \\__ _ _ __(_)___ \r\n / / | \\ \\ / / | '_ \\   / /_)/ _` | '__| / __|\r\n/ /__| |\\ V /| | | | | / ___/ (_| | |  | \\__ \\\r\n\\____/_| \\_/ |_|_| |_| \\/    \\__,_|_|  |_|___/\r\n                                              \r\n";
                int startRow = (height / 2) - (asciiBienvenue.Length / 2);
                for (int i = 0; i <= asciiBienvenue.Length; i++)
                {

                    if (i == asciiBienvenue.Length)
                    {
                        Console.SetCursorPosition((width - asciiBienvenue[i - 1].Length) / 2, startRow + i);
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.Write("Tom DEHE COHEN - Quentin DE WOLF - Nils DEMOUGEOT");
                    }
                    else
                    {
                        Console.SetCursorPosition((width - asciiBienvenue[i].Length) / 2, startRow + i);
                        for (int j = 0; j < asciiBienvenue[i].Length; j++)
                        {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            if (asciiBienvenue[i][j] != '█')
                            {
                                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                            }
                            Console.Write(asciiBienvenue[i][j]);
                        }
                    }
                }

                Console.ResetColor();
                Console.SetCursorPosition((width - 53) / 2, height-5);
                Console.Write("◀ ■ ▶ Utilisez les touches flechées de votre clavier");
                Console.SetCursorPosition((width - 39) / 2, height - 4);
                Console.Write("●  Appuyez sur ENTRER pour selectionner");
                Console.SetCursorPosition((width - 32) / 2, height - 3);
                Console.Write("○  Appuyez sur ECHAP pour sortir");

            }

            static void MenuAdminSelected()
            {
                Console.ResetColor();
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.SetCursorPosition(10, 3);
                Console.Write("┏━━━━━━━━━━━━━━━━━━━━━━━┓");
                Console.SetCursorPosition(10, 4);
                Console.Write("┃ ");
                Console.BackgroundColor = ConsoleColor.DarkYellow;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" Connexion Admin (1) ");
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write(" ┃");
                Console.SetCursorPosition(10, 5);
                Console.Write("┗━━━━━━━━━━━━━━━━━━━━━━━┛");

                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.SetCursorPosition(8, 2);
                Console.Write("╭");
                Console.SetCursorPosition(8, 6);
                Console.Write("╰");
                Console.SetCursorPosition(36, 2);
                Console.Write("╮");
                Console.SetCursorPosition(36, 6);
                Console.Write("╯");

                Console.ResetColor();
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.SetCursorPosition(48, 3);
                Console.Write("                        ");
                Console.SetCursorPosition(48, 4);
                Console.Write("  Connexion Client (2)  ");
                Console.SetCursorPosition(48, 5);
                Console.Write("                        ");

                Console.ResetColor();
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.SetCursorPosition(87, 3);
                Console.Write("                           ");
                Console.SetCursorPosition(87, 4);
                Console.Write("  Connexion Cuisinier (3)  ");
                Console.SetCursorPosition(87, 5);
                Console.Write("                           ");

                Console.ForegroundColor = ConsoleColor.White;
            }

            static void MenuClientSelected()
            {
                Console.ResetColor();
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.SetCursorPosition(10, 3);
                Console.Write("                       ");
                Console.SetCursorPosition(10, 4);
                Console.Write("  Connexion Admin (1)  ");
                Console.SetCursorPosition(10, 5);
                Console.Write("                       ");


                Console.ResetColor();
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.SetCursorPosition(48, 3);
                Console.Write("┏━━━━━━━━━━━━━━━━━━━━━━━━┓");
                Console.SetCursorPosition(48, 4);
                Console.Write("┃ ");
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" Connexion Client (2) ");
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write(" ┃");
                Console.SetCursorPosition(48, 5);
                Console.Write("┗━━━━━━━━━━━━━━━━━━━━━━━━┛");

                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.SetCursorPosition(46, 2);
                Console.Write("╭");
                Console.SetCursorPosition(46, 6);
                Console.Write("╰");
                Console.SetCursorPosition(75, 2);
                Console.Write("╮");
                Console.SetCursorPosition(75, 6);
                Console.Write("╯");


                Console.ResetColor();
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.SetCursorPosition(87, 3);
                Console.Write("                           ");
                Console.SetCursorPosition(87, 4);
                Console.Write("  Connexion Cuisinier (3)  ");
                Console.SetCursorPosition(87, 5);
                Console.Write("                           ");

                Console.ForegroundColor = ConsoleColor.White;
            }

            static void MenuCuisinierSelected()
            {
                Console.ResetColor();
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.SetCursorPosition(10, 3);
                Console.Write("                       ");
                Console.SetCursorPosition(10, 4);
                Console.Write("  Connexion Admin (1)  ");
                Console.SetCursorPosition(10, 5);
                Console.Write("                       ");

                Console.ResetColor();
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.SetCursorPosition(48, 3);
                Console.Write("                        ");
                Console.SetCursorPosition(48, 4);
                Console.Write("  Connexion Client (2)  ");
                Console.SetCursorPosition(48, 5);
                Console.Write("                        ");


                Console.ResetColor();
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.SetCursorPosition(87, 3);
                Console.Write("┏━━━━━━━━━━━━━━━━━━━━━━━━━━━┓");
                Console.SetCursorPosition(87, 4);
                Console.Write("┃ ");
                Console.BackgroundColor = ConsoleColor.Green;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" Connexion Cuisinier (3) ");
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(" ┃");
                Console.SetCursorPosition(87, 5);
                Console.Write("┗━━━━━━━━━━━━━━━━━━━━━━━━━━━┛");

                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.SetCursorPosition(85, 2);
                Console.Write("╭");
                Console.SetCursorPosition(85, 6);
                Console.Write("╰");
                Console.SetCursorPosition(117, 2);
                Console.Write("╮");
                Console.SetCursorPosition(117, 6);
                Console.Write("╯");

                Console.ForegroundColor = ConsoleColor.White;
            }
        }


        /// <summary>
        /// S'occupe de l'affichage du menu pricnipal / selection des modules
        /// </summary>

        static void AffichageMenuClient()
        {
            int choix = MenuCirculaire(3, "connexion", "associer un compte", "creer un compte", "");
            if (choix == -2)
            {
                return;
            }
            else if (choix == 0)
            {

            }
            else if (choix == 1)
            {

            }
            else if (choix == 2)
            {

            }
        }



        #region Outils
        /// <summary>
        /// Permet la gestion centralisée des demandes d'entrée au près de l'utilisateur sur la console. L'entrée est vérifiée et sécurisée dans une certaine mesure.
        /// </summary>
        /// <param name="question">Question qui va être affichée</param>
        /// <param name="type">Type de réponse attendue (int, string, bool)</param>
        /// <param name="required">Indique si le champ est requis à l'endroit où il est placé</param>
        /// <returns></returns>
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
                    Console.Write("* ");
                }
                Console.ResetColor();
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("➜  ");
                Console.ForegroundColor = ConsoleColor.Yellow;

                reponse = Console.ReadLine();

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
                    else if (type == "bool")
                    {
                        if (reponse.ToLower() == "oui")
                        {
                            correcte = true;
                            reponse = "True";
                        }
                        else if (reponse.ToLower() == "non")
                        {
                            correcte = true;
                            reponse = "False";
                        }
                    }
                }
                else if (!required)
                {
                    if (reponse == null)
                    {
                        correcte = true;
                    }
                    else if (type == "int" && int.TryParse(reponse, out int n))
                    {
                        correcte = true;
                        reponse = reponse.Trim();
                    }
                    else if (type == "string")
                    {
                        correcte = true;
                        reponse = reponse.Trim();
                    }
                }

                if (!correcte)
                {
                    Console.Write("\t");
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(" Erreur : Format incorrect ! ");
                    Console.ResetColor();
                }

                Console.WriteLine();
            }

            Console.ResetColor();
            return reponse;
        }

        /// <summary>
        /// Affichage et interaction avec le menu circulaire utilisé un peu partout dans l'application
        /// </summary>
        /// <param name="nbChoix">Nombre de choix possibles pour l'utilisateur (<=4)</param>
        /// <param name="libelle1">Premier choix proposé à l'utilisateur</param>
        /// <param name="libelle2">Deuxième choix proposé à l'utilisateur</param>
        /// <param name="libelle3">Troisième choix proposé à l'utilisateur</param>
        /// <param name="libelle4">Quatrieme choix proposé à l'utilisateur</param>
        /// <returns>Retourne un chiffre entre 0 et 3 si un choix a été fait (sens de rotation antihoraire), 
        /// retourne -1 si erreur, retourne -2 si l'utilisateur souhaite quitter le menu</returns>
        public static int MenuCirculaire(int nbChoix, string libelle1, string libelle2, string libelle3, string libelle4)
        {
            int choixSelected = -1;
            bool end = false;
            while (!end)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();

                Console.WriteLine("Utilisez les touches flechées de votre clavier");
                Console.WriteLine("Appuyez sur ECHAP pour sortir");

                string libelle = "";
                int width = Console.WindowWidth;
                int height = Console.WindowHeight;

                if (nbChoix > 0)
                {
                    libelle = " " + libelle1.ToUpper() + " ";
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Magenta;
                } else
                {
                    libelle = " - ";
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                    Console.SetCursorPosition((width / 2) - (libelle.Length / 2), height / 4);
                Console.WriteLine(libelle);
                Console.ResetColor();

                if (nbChoix > 1)
                {
                    libelle = " " + libelle2.ToUpper() + " ";
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                } else
                {
                    libelle = " - ";
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.SetCursorPosition((width / 4) - (libelle.Length / 2), height / 2);
                Console.WriteLine(libelle);
                Console.ResetColor();

                if (nbChoix > 2)
                {
                    libelle = " " + libelle3.ToUpper() + " ";
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Red;
                } else
                {
                    libelle = " - ";
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.SetCursorPosition((width / 2) - (libelle.Length / 2), 3 * (height / 4) + 2);
                Console.WriteLine(libelle);
                Console.ResetColor();

                if (nbChoix > 3)
                {
                    libelle = " " + libelle4.ToUpper() + " ";
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Green;
                    
                } else
                {
                    libelle = " - ";
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.SetCursorPosition(3 * (width / 4) - (libelle.Length / 2), height / 2);
                Console.WriteLine(libelle);
                Console.ResetColor();


                Console.SetCursorPosition(width / 2, height / 2 - 1);
                Console.Write("▲");

                Console.SetCursorPosition(width / 2 - 2, height / 2);
                Console.Write("◀ ■ ▶");

                Console.SetCursorPosition(width / 2, height / 2 + 1);
                Console.Write("▼");

                Console.SetCursorPosition(width / 2, height / 2 + 2);

                bool entreeCorrecte = false;
                while (!entreeCorrecte)
                {
                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                        if (keyInfo.Key == ConsoleKey.UpArrow && nbChoix > 0)
                        {
                            entreeCorrecte = true;

                            end = true;
                            choixSelected = 0;
                        }
                        else if (keyInfo.Key == ConsoleKey.LeftArrow && nbChoix > 1)
                        {
                            entreeCorrecte = true;

                            end = true;
                            choixSelected = 1;
                        }
                        else if (keyInfo.Key == ConsoleKey.DownArrow && nbChoix > 2)
                        {
                            entreeCorrecte = true;

                            end = true;
                            choixSelected = 2;
                        }
                        else if (keyInfo.Key == ConsoleKey.RightArrow && nbChoix > 3)
                        {
                            entreeCorrecte = true;

                            end = true;
                            choixSelected = 3;
                        }
                        else if (keyInfo.Key == ConsoleKey.Escape)
                        {
                            entreeCorrecte = true;
                            end = true;
                            choixSelected = -2;
                        }
                    }

                    Thread.Sleep(50);
                }
            }

            return choixSelected;
        }

        /// <summary>
        /// Permet de facilement exécuter des commandes de manipulation des données (DML)
        /// </summary>
        /// <param name="req">requete sql à executer</param>
        /// <returns>retourne true si la requete a bien été exécutée, false sinon</returns>
        public static bool DML_SQL(string req)
        {
            bool reussi = true;
            MySqlCommand command = connexion.CreateCommand();
            command.CommandText = req;
            try
            {
                command.ExecuteNonQuery();
            }
            catch (MySqlException e)
            {
                Console.WriteLine(" Erreur : " + e.ToString());
                Console.ReadLine();
                reussi = false;
                return reussi;
            }
            Console.ResetColor();
            command.Dispose();
            Console.WriteLine();

            return reussi;
        }

        /// <summary>
        /// Permet de facilement exécuter des commandes de requête de données (DQL)
        /// </summary>
        /// <param name="req">requete sql à executer</param>
        /// <param name="affichage">affiche les resultats de la requete sous forme de tableau si 'true'</param>
        /// <returns></returns>
        public static List<string[]> DQL_SQL(string req, bool affichage)
        {
            MySqlCommand command = connexion.CreateCommand();
            command.CommandText = req;

            List<string[]> resultat = new List<string[]>();
            MySqlDataReader reader = command.ExecuteReader();

            string[] valueString = new string[reader.FieldCount];
            while (reader.Read())
            {
                if (affichage)
                {
                    Console.Write("◌ ");
                }
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    valueString[i] = reader.GetValue(i).ToString();
                    if (affichage && i < reader.FieldCount - 1)
                    {
                        Console.Write(valueString[i] + ", ");
                    }
                    else if (affichage && i == reader.FieldCount - 1)
                    {
                        Console.Write(valueString[i]);
                    }
                }
                resultat.Add(valueString);
                if (affichage)
                {
                    Console.WriteLine();
                }
            }
            reader.Close();
            command.Dispose();

            return resultat;
        }

        /// <summary>
        /// Permet l'établissement d'une connexion SQL
        /// </summary>
        /// <param name="connectionString">informations de connexion avec la base de donnée</param>
        /// <returns>renvoie une instance de connexion avec la base de donnée</returns>
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
        #endregion
    }
}

        