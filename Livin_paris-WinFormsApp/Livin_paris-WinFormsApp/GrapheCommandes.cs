using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using System.Xml.Serialization;
using static System.Windows.Forms.LinkLabel;
using static Livin_paris_WinFormsApp.Outils;

namespace Livin_paris_WinFormsApp
{

    public class ResultatsAnalyse
    {
        public int NombreCouleurs { get; set; }
        public bool Biparti { get; set; }
        public bool Planaire { get; set; }
    }

    public class GrapheCommandes
    {
        private Dictionary<Compte<int>, List<Compte<int>>> listeAdjacence;
        private string requete;
        public GrapheCommandes()
        {
            this.listeAdjacence = new Dictionary<Compte<int>, List<Compte<int>>>();
            this.requete = "SELECT DISTINCT cli.id_compte AS id_compte_client, cui.id_compte AS id_compte_cuisinier FROM commande co JOIN ligne_de_commande ldc ON co.id_commande = ldc.id_commande JOIN plat pl ON ldc.id_plat = pl.id_plat JOIN client cli ON co.id_client = cli.id_client JOIN cuisinier cui ON pl.id_cuisinier = cui.id_cuisinier;";
            List<string[]> result = DQL_SQL(requete, false);
            for (int i = 0; i < result.Count; i++)
            {
                Compte<int> client = new Compte<int>(Convert.ToInt32(result[i][0]), "client");
                Compte<int> cuisinier = new Compte<int>(Convert.ToInt32(result[i][1]), "cuisinier");

                if (!this.listeAdjacence.ContainsKey(cuisinier))
                {
                    this.listeAdjacence[cuisinier] = new List<Compte<int>>() { client };
                }
                else
                {
                    this.listeAdjacence[cuisinier].Add(client);
                }
                if (!this.listeAdjacence.ContainsKey(client))
                {
                    this.listeAdjacence[client] = new List<Compte<int>>() { cuisinier };
                }
                else
                {
                    this.listeAdjacence[client].Add(cuisinier);
                }
            }
            Dictionary<Compte<int>, int> coloration = WelshPowell(listeAdjacence);
            Dictionary<string, object> resultats = AnalyserColorationGraphe(listeAdjacence, coloration);
            string cheminJSON = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..", "resultats.json");
            string cheminXML = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..", "resultats.xml");
            ExporterEnJson(resultats, cheminJSON);
            var resultats_xml = new ResultatsAnalyse
            {
                NombreCouleurs = (int)resultats["nombre_couleurs"],
                Biparti = (bool)resultats["biparti"],
                Planaire = (bool)resultats["planaire"]
            };
            ExporterEnXml(resultats_xml, cheminXML);
        }

        /// <summary>
        /// Affiche dans la console le contenu d'une liste d'adjacence représentant un graphe
        /// </summary>
        /// <param name="liste">
        /// La liste d'adjacence, sous forme de dictionnaire où chaque compte est associé à une liste de ses voisins.
        /// </param>
        public static void AfficherListe(Dictionary<Compte<int>, List<Compte<int>>> liste)
        {
            foreach (var pair in liste)
            {
                Compte<int> cle = pair.Key;
                List<Compte<int>> listeAssociee = pair.Value;

                Console.WriteLine($"{cle.Id}");

                foreach (var compte in listeAssociee)
                {
                    Console.WriteLine($"   -> {compte.Id}");
                }
            }

        }

        /// <summary>
        /// Applique l'algorithme de Welsh-Powell pour colorier le graphe
        /// </summary>
        /// <param name="graphe">
        /// Le graphe représenté sous forme de liste d'adjacence : chaque compte est associé à la liste des comptes avec lesquels il il a passé ou recu une commande
        /// </param>
        /// <returns>
        /// Un dictionnaire associant chaque sommet à un entier représentant sa couleur. Aucun sommet adjacent ne partage la même couleur.
        /// </returns>
        public static Dictionary<Compte<int>, int> WelshPowell(Dictionary<Compte<int>, List<Compte<int>>> graphe)
        {
            Dictionary<Compte<int>, int> couleurs = new Dictionary<Compte<int>, int>();

            List<Compte<int>> sommets = new List<Compte<int>>();
            foreach (var pair in graphe)
            {
                sommets.Add(pair.Key);
            }

            for (int i = 0; i < sommets.Count - 1; i++)
            {
                for (int j = i + 1; j < sommets.Count; j++)
                {
                    int degreI = graphe.ContainsKey(sommets[i]) ? graphe[sommets[i]].Count : 0;
                    int degreJ = graphe.ContainsKey(sommets[j]) ? graphe[sommets[j]].Count : 0;

                    if (degreJ > degreI)
                    {
                        var temp = sommets[i];
                        sommets[i] = sommets[j];
                        sommets[j] = temp;
                    }
                }
            }

            int couleurActuelle = 0;

            for (int i = 0; i < sommets.Count; i++)
            {
                Compte<int> sommet = sommets[i];

                if (!couleurs.ContainsKey(sommet))
                {
                    couleurs[sommet] = couleurActuelle;

                    for (int j = 0; j < sommets.Count; j++)
                    {
                        Compte<int> autre = sommets[j];

                        if (!couleurs.ContainsKey(autre))
                        {
                            bool enConflit = false;

                            if (graphe.ContainsKey(autre))
                            {
                                foreach (var voisin in graphe[autre])
                                {
                                    if (couleurs.ContainsKey(voisin) && couleurs[voisin] == couleurActuelle)
                                    {
                                        enConflit = true;
                                        break;
                                    }
                                }
                            }

                            if (!enConflit)
                            {
                                couleurs[autre] = couleurActuelle;
                            }
                        }
                    }

                    couleurActuelle++;
                }
            }

            return couleurs;
        }

        /// <summary>
        /// Analyse les résultats de la coloration du graphe et extrait plusieurs propriétés du graphe.
        /// </summary>
        /// <param name="graphe">
        /// Le graphe représenté sous forme de liste d’adjacence
        /// </param>
        /// <param name="coloration">
        /// Un dictionnaire contenant la coloration du graphe, associant chaque sommet à un numéro de couleur.
        /// </param>
        /// <returns>
        /// Un dictionnaire contenant les résultats de l’analyse, avec les clés suivantes :
        /// "nombre_couleurs" (int), "groupes_independants" (List<List<Compte<int>>>), 
        /// "biparti" (bool) et "planaire" (bool).
        /// </returns>
        public static Dictionary<string, object> AnalyserColorationGraphe(Dictionary<Compte<int>, List<Compte<int>>> graphe,
                                            Dictionary<Compte<int>, int> coloration)
        {
            Dictionary<string, object> resultats = new Dictionary<string, object>();
            Console.WriteLine("\nAnalyse du graphe des commandes entre clients et cuisiniers :");

            int nombreCouleurs = 0;
            List<int> couleursUtilisees = new List<int>();

            foreach (var kvp in coloration)
            {
                if (!couleursUtilisees.Contains(kvp.Value))
                {
                    couleursUtilisees.Add(kvp.Value);
                    nombreCouleurs++;
                }
            }
            resultats["nombre_couleurs"] = nombreCouleurs;
            Console.WriteLine($"\nnombre minimal de couleurs : {nombreCouleurs}");

            Console.WriteLine("\nGroupes indépendants :");
            List<List<Compte<int>>> groupes = new List<List<Compte<int>>>();

            for (int i = 0; i < nombreCouleurs; i++)
            {
                groupes.Add(new List<Compte<int>>());
            }

            foreach (var kvp in coloration)
            {
                int couleur = kvp.Value;
                groupes[couleur].Add(kvp.Key);
            }
            resultats["groupes_independants"] = groupes;
            for (int i = 0; i < groupes.Count; i++)
            {
                Console.Write($"Groupe {i} (couleur {i}): ");
                foreach (var sommet in groupes[i])
                {
                    Console.Write(sommet.Id + " ");
                }
                Console.WriteLine();
            }


            bool estBiparti = false;
            if (nombreCouleurs == 2)
            {
                estBiparti = true;
            }
            Console.WriteLine("\nBiparti :");
            if (estBiparti)
            {
                foreach (var kvp in graphe)
                {
                    var sommet = kvp.Key;
                    int couleurSommet = coloration[sommet];

                    foreach (var voisin in kvp.Value)
                    {
                        if (coloration.ContainsKey(voisin) && coloration[voisin] == couleurSommet)
                        {
                            estBiparti = false;
                            break;
                        }
                    }
                    if (!estBiparti)
                        break;
                }
            }
            resultats["biparti"] = estBiparti;
            if (estBiparti)
                Console.WriteLine("Le graphe est biparti : 2 couleurs sans conflit entre voisins");
            else
                Console.WriteLine("Le graphe n’est pas biparti : plus de 2 couleurs ou conflit detecte");


            Console.WriteLine("\nPlanaire :");
            bool planaire = true;
            int n = graphe.Count;
            int m = 0;

            foreach (var kvp in graphe)
            {
                m += kvp.Value.Count;
            }
            m /= 2;

            if (n >= 3)
            {
                int borne = 3 * n - 6;
                if (m <= borne)
                {
                    Console.WriteLine($"Le graphe est planaire : m = {m} ≤ 3n - 6 = {borne}.");
                }
                else
                {
                    Console.WriteLine($"Le graphe n'est pas planaire : m = {m} > 3n - 6 = {borne}.");
                    planaire = false;
                }
            }
            else
            {
                Console.WriteLine("Moins de 3 sommets : le graphe est planaire");
            }
            resultats["planaire"] = planaire;
            return resultats;
        }

        /// <summary>
        /// Exporte les résultats de l’analyse du graphe au format JSON dans un fichier.
        /// </summary>
        /// <param name="resultats">
        /// Un dictionnaire contenant les résultats de l’analyse de la coloration
        /// </param>
        /// <param name="cheminFichier">
        /// Le chemin du fichier dans lequel le JSON sera enregistré.
        /// </param>
        public static void ExporterEnJson(Dictionary<string, object> resultats, string cheminFichier)
        {
            string json = JsonSerializer.Serialize(resultats, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(cheminFichier, json);
            Console.WriteLine($"\nLes résultats ont été exportés en JSON dans le fichier : {cheminFichier}");
        }

        /// <summary>
        /// Exporte les résultats de l’analyse du graphe au format XML dans un fichier.
        /// </summary>
        /// <param name="resultats">
        /// Un dictionnaire contenant les résultats de l’analyse de la coloration
        /// </param>
        /// <param name="cheminFichier">
        /// Le chemin du fichier dans lequel le XML sera enregistré.
        /// </param>
        public static void ExporterEnXml(ResultatsAnalyse resultats, string cheminFichier)
        {

            var xmlSerializer = new XmlSerializer(typeof(ResultatsAnalyse));

            using (var writer = new StreamWriter(cheminFichier))
            {
                xmlSerializer.Serialize(writer, resultats);
            }

            Console.WriteLine($"\nLes résultats ont été exportés en XML dans le fichier : {cheminFichier}\n");
        }
    }
}
