using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Livin_paris_WinFormsApp
{
    public static class Outils
    {
        #region Outils
        /// <summary>
        /// Permet la gestion centralisée des demandes d'entrée au près de l'utilisateur sur la console. L'entrée est vérifiée et sécurisée dans une certaine mesure.
        /// </summary>
        /// <param name="question">Question qui va être affichée</param>
        /// <param name="type">Type de réponse attendue (int, string, bool)</param>
        /// <param name="required">Indique si le champ est requis à l'endroit où il est placé</param>
        /// <returns></returns>
        public static string Demander(string question, string type, bool required, Graphe<int> graphe = null)
        {// daire prise en compte de la date
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
                    } else if (type == "mdp" && reponse.Trim() != "")
                    {
                        correcte = true;
                        reponse = reponse.Trim();
                    } else if (type == "station" && graphe.GetNoeuds().Any(noeud => noeud.Nom == reponse.Trim()))
                    {
                        correcte = true;
                        reponse = reponse.Trim();
                    } else if (type == "email" && Regex.IsMatch(reponse.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    {
                        correcte = true;
                        reponse = reponse.Trim();
                    } else if (type == "date" && Regex.IsMatch(reponse.Trim(), @"^\d{2}/\d{2}/\d{4}$"))
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
        public static int MenuCirculaire(int nbChoix, string libelle1, string libelle2, string libelle3, string libelle4, string message)
        {
            int choixSelected = -1;
            bool end = false;
            while (!end)
            {
                Console.ResetColor();
                Console.Clear();

                Console.SetCursorPosition(1, 0);
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write(" ▪ " + message+ " ");
                Console.ResetColor();
                Console.SetCursorPosition(1, 1);
                Console.Write("Utilisez les touches flechées de votre clavier");
                Console.SetCursorPosition(1, 2);
                Console.Write("Appuyez sur ECHAP pour sortir");

                string libelle = "";
                int width = Console.WindowWidth;
                int height = Console.WindowHeight;

                if (nbChoix > 0)
                {
                    libelle = " " + libelle1.ToUpper() + " ";
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Magenta;
                }
                else
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
                }
                else
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
                }
                else
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

                }
                else
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

                Console.SetCursorPosition(0, height-1);

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
            MySqlCommand command = Program.connexion.CreateCommand();
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
            MySqlCommand command = Program.connexion.CreateCommand();
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
        public static MySqlConnection ConnexionSQL(string connectionString)
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
