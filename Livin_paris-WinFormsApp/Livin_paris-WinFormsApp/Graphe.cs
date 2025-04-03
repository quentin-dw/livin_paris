using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Livin_paris_WinFormsApp
{
    internal class Graphe<T>
    {
        private Dictionary<Noeud<int>, Dictionary<Noeud<int>, int>> listeAdjacence;
        private List<Noeud<int>> noeuds = new List<Noeud<int>>();
        private List<Lien<int>> liens = new List<Lien<int>>();

        private int[,] matriceAdjacence;
        private int nbNoeuds;

        public List<Noeud<int>> GetNoeuds()
        {
            return noeuds;
        }

        public List<Lien<int>> GetLiens()
        {
            return liens;
        }


        public Graphe(string noeuds, string arcs)
        {
            this.listeAdjacence = new Dictionary<Noeud<int>, Dictionary<Noeud<int>, int>>();
            this.nbNoeuds = 0;
            ChargerGrapheCSV(noeuds, arcs);

            //List<Noeud<T>> dfs = DFS(new Noeud(1));
            //Console.Write("DFS : ");
            //AfficherListe(dfs);


            //List<Noeud<T>> bfs = BFS(new Noeud(1));
            //Console.Write("BFS : ");
            //AfficherListe(bfs);


            //Console.WriteLine(this.EstConnexe());

            //this.ContientCicuits();
        }

        private void ChargerGrapheCSV(string noeuds, string arcs)
        {
            try
            {
                string[] lignes_noeuds = File.ReadAllLines(noeuds);
                for (int i = 1; i < lignes_noeuds.Length; i++)
                {
                    string[] valeurs = lignes_noeuds[i].Split(new char[] { ',' });
                    int id = Convert.ToInt32(valeurs[0]);
                    string ligne = valeurs[1];
                    string nom = valeurs[2];
                    double latitude = double.Parse(valeurs[3], CultureInfo.InvariantCulture);
                    double longitude = double.Parse(valeurs[4], CultureInfo.InvariantCulture);
                    string nom_commune = valeurs[5];
                    string code_insee = valeurs[6];
                    Noeud<int> noeud = new Noeud<int>(id, ligne, nom, longitude, latitude, nom_commune, code_insee);
                    this.noeuds.Add(noeud);
                    this.listeAdjacence[noeud] = new Dictionary<Noeud<int>, int>();
                }
                string[] lignes_arcs = File.ReadAllLines(arcs);

                for (int i = 1; i < lignes_arcs.Length; i++)
                {
                    string[] valeurs = lignes_arcs[i].Split(new char[] { ',' });
                    Noeud<int> noeud = this.noeuds[Convert.ToInt32(valeurs[0]) - 1];
                    if (!string.IsNullOrWhiteSpace(valeurs[2]))
                    {
                        Noeud<int> noeud_precedent = this.noeuds[Convert.ToInt32(valeurs[2]) - 1];
                        //if (valeurs[valeurs.Length - 1] == "FALSE")
                        //{
                        //    this.liens.Add(new Lien<int>(noeud_precedent, noeud, Convert.ToInt32(valeurs[4])));
                        //}
                    }
                    if (!string.IsNullOrWhiteSpace(valeurs[3]))
                    {
                        Noeud<int> noeud_suivant = this.noeuds[Convert.ToInt32(valeurs[3]) - 1];
                        this.liens.Add(new Lien<int>(noeud, noeud_suivant, Convert.ToInt32(valeurs[4])));
                        this.listeAdjacence[noeud][noeud_suivant] = Convert.ToInt32(valeurs[4]);
                        if (valeurs[valeurs.Length - 1] == "FALSE")
                        {
                            this.liens.Add(new Lien<int>(noeud_suivant, noeud, Convert.ToInt32(valeurs[4])));
                            this.listeAdjacence[noeud_suivant][noeud] = Convert.ToInt32(valeurs[4]);
                        }
                    }
                }
                //foreach (var noeud in listeAdjacence)
                //{
                //    Console.Write($"{noeud.Key} -> ");
                //    foreach (var voisin in noeud.Value)
                //    {
                //        Console.Write($"[{voisin.Key} : {voisin.Value} min]  ");
                //    }
                //    Console.WriteLine();
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
