using System;
using System.IO;
using System.Collections.Generic;
using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using MySql.Data.MySqlClient;
using System.Data;
using ZstdSharp.Unsafe;

using static Livin_paris_WinFormsApp.Outils;


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
            Graphe<int> graphe = new Graphe<int>(noeuds, arcs);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1("../../../../../noeuds.csv", "../../../../../arcs.csv"));
            Application.Run(new Form2("../../../../../noeuds.csv", "../../../../../arcs.csv"));

            Console.OutputEncoding = System.Text.Encoding.UTF8;

            string connectionString = "Server=localhost;Database=psi_demougeot_dehecohen_dewolf;User ID=root;Password=root;SslMode=none;";
            connexion = ConnexionSQL(connectionString);

            Thread.Sleep(50000);
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

            int menuSelected = 0;
            while (!finProgramme)
            {
                bool entreeCorrecte = false;
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
                                    TableauClient.AffichageMenuClient();
                                    entreeCorrecte = true;
                                    break;
                                case 2 :
                                    TableauCuisinier.AffichageMenuCuisinier();
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
                            TableauClient.AffichageMenuClient();
                            entreeCorrecte = true;
                        }
                        else if (keyInfo.Key == ConsoleKey.NumPad3 || keyInfo.Key == ConsoleKey.D3)
                        {
                            TableauCuisinier.AffichageMenuCuisinier();
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
                Console.SetCursorPosition(0, Console.WindowHeight - 1);
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
                Console.SetCursorPosition(0, Console.WindowHeight - 1);
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
                Console.SetCursorPosition(0, Console.WindowHeight - 1);
            }
        }


    }
}

        