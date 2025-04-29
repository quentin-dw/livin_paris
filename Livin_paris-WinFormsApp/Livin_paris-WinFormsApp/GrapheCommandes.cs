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
            AfficherListe(listeAdjacence);
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

        public static Dictionary<Compte<int>, int> WelshPowell(Dictionary<Compte<int>, List<Compte<int>>> graphe)
        {
            Dictionary<Compte<int>, int> couleurs = new Dictionary<Compte<int>, int>();

            // creer la liste des sommets
            List<Compte<int>> sommets = new List<Compte<int>>();
            foreach (var pair in graphe)
            {
                sommets.Add(pair.Key);
            }

            // trier les sommets par degré décroissant (tri à bulles)
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

            // appliquer l'algorithme de Welsh Powell
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

        public static Dictionary<string, object> AnalyserColorationGraphe(Dictionary<Compte<int>, List<Compte<int>>> graphe,
                                            Dictionary<Compte<int>, int> coloration)
        {
            Dictionary<string, object> resultats = new Dictionary<string, object>();
            Console.WriteLine("\nAnalyse du graphe des commandes entre clients et cuisiniers :");

            // nombre minimal de couleurs
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

            // Groupes independants (par couleur)
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

            // verifier si le graphe est biparti (= 2 couleurs sans conflit)
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

            // planarité : test d’euler (m ≤ 3n - 6)
            Console.WriteLine("\nPlanarité :");
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

        public static void ExporterEnJson(Dictionary<string, object> resultats, string cheminFichier)
        {
            string json = JsonSerializer.Serialize(resultats, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(cheminFichier, json);
            Console.WriteLine($"Les résultats ont été exportés en JSON dans le fichier : {cheminFichier}");
        }

        public static void ExporterEnXml(ResultatsAnalyse resultats, string cheminFichier)
        {
            // Sérialiser l'objet personnalisé en XML
            var xmlSerializer = new XmlSerializer(typeof(ResultatsAnalyse));

            using (var writer = new StreamWriter(cheminFichier))
            {
                xmlSerializer.Serialize(writer, resultats);
            }

            Console.WriteLine($"Les résultats ont été exportés en XML dans le fichier : {cheminFichier}");
        }
    }
}
