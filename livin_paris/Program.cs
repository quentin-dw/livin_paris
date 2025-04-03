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
            //string filePath = "../../../../soc-karate.mtx";
            string noeuds = "../../../../noeuds.csv";
            string arcs = "../../../../arcs.csv";
            Graphe<int> graphe = new Graphe<int>(noeuds, arcs);

            var (chemin, cout) = graphe.TrouverMeilleurChemin("Opera", "Exelmans");

            // Affichage du résultat
            if (chemin.Count == 0)
                Console.WriteLine("Aucun chemin trouvé.");
            else
                Console.WriteLine($"Plus court chemin ({cout} min) :\n {string.Join("\n -> ", chemin)}");
            /*

            string connectionString = "Server=localhost;Database=film;User ID=root;Password=root;SslMode=none;";
            MySqlConnection conn = ConnexionSQL(connectionString);

            Console.ReadLine();
            */
        }

        /*
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
        }*/
    }
}
