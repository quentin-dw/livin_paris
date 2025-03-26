using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace livin_paris
{
    internal class Graphe
    {
        private Dictionary<int, List<Noeud>> listeAdjacence;
        private int[,] matriceAdjacence;
        private int nbNoeuds;

        public Graphe(string fichier)
        {
            this.listeAdjacence = new Dictionary<int, List<Noeud>>();
            this.nbNoeuds = 0;
            ChargerGraphe(fichier);

            List<Noeud> dfs = DFS(new Noeud(1));
            Console.Write("DFS : ");
            AfficherListe(dfs);


            List<Noeud> bfs = BFS(new Noeud(1));
            Console.Write("BFS : ");
            AfficherListe(bfs);


            Console.WriteLine(this.EstConnexe());

            this.ContientCicuits();
        }
        private void ChargerGraphe(string fichier)
        {
            try
            {
                string[] lignes = File.ReadAllLines(fichier);
                foreach (string ligne in lignes)
                {
                    if (ligne[0] != '%')
                    {
                        string[] noeudsLigne = ligne.Split(' ');

                        if (noeudsLigne.Length == 3)
                        {
                            this.nbNoeuds = int.Parse(noeudsLigne[0]);
                            for (int i = 1; i <= nbNoeuds; i++)
                            {
                                this.listeAdjacence.Add(i, new List<Noeud>());
                            }
                        }
                        if (noeudsLigne.Length == 2)
                        {

                            Noeud n1 = new Noeud(int.Parse(noeudsLigne[0]));
                            Noeud n2 = new Noeud(int.Parse(noeudsLigne[1]));

                            bool verif = true;
                            foreach (Noeud noeud in this.listeAdjacence[n1.Id])
                            {
                                if (noeud.isEqual(n2))
                                {
                                    verif = false;
                                }
                            }

                            if (verif == true)
                            {
                                this.listeAdjacence[n1.Id].Add(n2);
                            }

                            verif = true;
                            foreach (Noeud noeud in this.listeAdjacence[n2.Id])
                            {
                                if (noeud.isEqual(n1))
                                {
                                    verif = false;
                                }
                            }
                            if (verif == true)
                            {
                                this.listeAdjacence[n2.Id].Add(n1);
                            }

                            Lien l = new Lien(n1, n2);
                        }
                    }
                }

                for (int i = 1; i <= this.nbNoeuds; i++)
                {
                    Console.Write("- " + i + " : ");
                    foreach (var n in this.listeAdjacence[i])
                    {
                        Console.Write(n.Id + ", ");
                    }
                    Console.WriteLine();
                }

                this.matriceAdjacence = new int[this.nbNoeuds, this.nbNoeuds];
                foreach (string ligne in lignes)
                {
                    if (ligne[0] != '%')
                    {
                        string[] noeudsLigne = ligne.Split(' ');
                        if (noeudsLigne.Length == 2)
                        {
                            matriceAdjacence[int.Parse(noeudsLigne[0]) - 1, int.Parse(noeudsLigne[1]) - 1]++;
                            matriceAdjacence[int.Parse(noeudsLigne[1]) - 1, int.Parse(noeudsLigne[0]) - 1]++;
                        }
                    }
                }
                for (int i = 0; i < matriceAdjacence.GetLength(0); i++)
                {
                    for (int j = 0; j < matriceAdjacence.GetLength(0); j++)
                    {
                        Console.Write(matriceAdjacence[i, j]);
                    }
                    Console.WriteLine();
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine("erreur : " + ex.Message);
            }
        }

        /// <summary>
        /// Algorithme de parcours en profondeur d'abord (DFS) à partir du point de départ en entrée.
        /// </summary>
        /// <param name="depart">Noeud de départ de l'algorithme</param>
        /// <returns></returns>
        public List<Noeud> DFS(Noeud depart) //Parcours en profondeur d'abord
        {
            List<Noeud> noeudsParcourus = new List<Noeud>();
            Stack<Noeud> pile = new Stack<Noeud>();
            pile.Push(depart);

            while (pile.Count > 0)
            {
                Noeud noeudActuel = pile.Pop();
                bool dejaVisite = false;
                for (int i = 0; i < noeudsParcourus.Count; i++)
                {
                    if (noeudsParcourus[i].isEqual(noeudActuel))
                    {
                        dejaVisite = true;
                    }
                }
                if (dejaVisite == false)
                {
                    noeudsParcourus.Add(noeudActuel);

                    foreach (Noeud voisin in listeAdjacence[noeudActuel.Id])
                    {
                        bool dejaVisiteVoisin = false;
                        for (int j = 0; j < noeudsParcourus.Count; j++)
                        {
                            if (noeudsParcourus[j].isEqual(voisin))
                            {
                                dejaVisiteVoisin = true;
                            }
                        }
                        if (dejaVisiteVoisin == false)
                        {
                            pile.Push(voisin);
                        }
                    }
                }
            }
            return noeudsParcourus;
        }

        /// <summary>
        /// Algorithme de parcours en largeur d'abord (BFS) à partir du point de départ en entrée.
        /// </summary>
        /// <param name="depart">Noeud de départ de l'algorithme</param>
        /// <returns></returns>
        public List<Noeud> BFS(Noeud depart)
        {
            List<Noeud> noeudsParcourus = new List<Noeud>();
            Queue<Noeud> file = new Queue<Noeud>();
            file.Enqueue(depart);
            noeudsParcourus.Add(depart);

            while (file.Count > 0)
            {
                Noeud noeudActuel = file.Dequeue();

                foreach (Noeud voisin in listeAdjacence[noeudActuel.Id])
                {
                    bool dejaVisite = false;
                    for (int i = 0; i < noeudsParcourus.Count; i++)
                    {
                        if (noeudsParcourus[i].isEqual(voisin))
                        {
                            dejaVisite = true;
                        }
                    }
                    if (dejaVisite == false)
                    {
                        noeudsParcourus.Add(voisin);
                        file.Enqueue(voisin);
                    }
                }
            }
            return noeudsParcourus;
        }

        /// <summary>
        /// Indique si le graphe courrant est connexe.
        /// </summary>
        /// <returns></returns>
        public bool EstConnexe()
        {
            List<Noeud> noeudsParcourus = DFS(new Noeud(1));

            if (noeudsParcourus.Count == this.nbNoeuds)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Vérifie si le graphe contient des circuits (cycles) et les affiches dans la console.
        /// La methode se base sur l'algorithme du DFS (parcours en profondeur d'abord) mais est modifiée 
        /// pour essayer depuis tous les points du graphe et s'arrête dès qu'un cycle a été trouvé pour en 
        /// chercher un autre.
        /// </summary>
        /// <returns></returns>
        public bool ContientCicuits()
        {
            bool contientCicuits = false;
            for (int n = 1; n <= this.nbNoeuds; n++)
            {
                Noeud noeudDepart = new Noeud(n);

                List<Noeud> noeudsParcourus = new List<Noeud>();
                Stack<Noeud> pile = new Stack<Noeud>();
                Dictionary<int, int> parent = new Dictionary<int, int>();

                pile.Push(noeudDepart);
                parent[noeudDepart.Id] = -1;

                bool boucle = false;
                while (pile.Count > 0 && boucle == false)
                {
                    Noeud noeudActuel = pile.Pop();

                    bool dejaVisite = false;
                    for (int i = 0; i < noeudsParcourus.Count; i++)
                    {
                        if (noeudsParcourus[i].isEqual(noeudActuel))
                        {
                            dejaVisite = true;
                        }
                    }
                    if (dejaVisite == false)
                    {
                        noeudsParcourus.Add(noeudActuel);

                        foreach (Noeud voisin in listeAdjacence[noeudActuel.Id])
                        {
                            bool dejaVisiteVoisin = false;
                            for (int j = 0; j < noeudsParcourus.Count; j++)
                            {
                                if (noeudsParcourus[j].isEqual(voisin))
                                {
                                    dejaVisiteVoisin = true;
                                }
                            }
                            if (dejaVisiteVoisin == false)
                            {
                                parent[voisin.Id] = noeudActuel.Id;
                                pile.Push(voisin);
                            }
                            else if (parent[noeudActuel.Id] != voisin.Id)
                            {
                                contientCicuits = true;
                                boucle = true;
                            }
                        }
                    }
                }
                Console.Write("Cycle " + n + " : ");
                AfficherListe(noeudsParcourus);
            }
            return contientCicuits;
        }

        /// <summary>
        /// Permet d'afficher dans la console une liste de Noeuds.
        /// </summary>
        /// <param name="liste">Liste de noeuds à afficher</param>
        private void AfficherListe(List<Noeud> liste)
        {
            for (int i = 0; i < liste.Count; i++)
            {
                Console.Write(liste[i].Id + ", ");
            }
            Console.WriteLine();
        }
    }
}