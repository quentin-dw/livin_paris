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
            new Graphe<int>(noeuds, arcs);
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
