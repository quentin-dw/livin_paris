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
            string depart = "Opera";
            string arrivee = "Exelmans";
            var (chemin, cout) = graphe.TrouverMeilleurChemin(depart, arrivee);

            // Affichage du résultat
            if (chemin.Count == 0)
                Console.WriteLine("Aucun chemin trouvé.");
            else
                Console.WriteLine($"Plus court chemin ({cout} min) :\n {string.Join("\n -> ", chemin)}");

            // COMPARER lES 3 ALGOS DE PLUS COURT CHEMIN (en moyenne Dijkstra = 3ms, BF = 100ms, FW = 7000ms)
            graphe.Chronometrer(() => graphe.Dijkstra(graphe.Noeuds[52], graphe.Noeuds[210]), "Dijkstra");
            graphe.Chronometrer(() => graphe.BellmanFord(graphe.Noeuds[52], graphe.Noeuds[210]), "Bellman-Ford");
            graphe.Chronometrer(() => graphe.FloydWarshall(), "Floyd-Warshall");



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
