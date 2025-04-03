using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace livin_paris
{
    internal class Graphe<T>
    {
        private Dictionary<Noeud<int>, Dictionary<Noeud<int>, int>> listeAdjacence;
        private List<Noeud<int>> noeuds = new List<Noeud<int>>();
        private List<Lien<int>> liens = new List<Lien<int>>();
        private int tempsChangement = 3;
        private int[,] matriceAdjacence;
        private int nbNoeuds;


        public Dictionary<Noeud<int>, Dictionary<Noeud<int>, int>> ListeAdjacence
        {
            get { return listeAdjacence; }
        }

        public List<Noeud<int>> Noeuds
        {
            get { return noeuds; }
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

                foreach (var n1 in this.noeuds)
                {
                    foreach (var n2 in this.noeuds)
                    {
                        if (Noeud<int>.memeStation(n1, n2))
                        {
                            this.listeAdjacence[n1][n2] = this.tempsChangement;
                            this.liens.Add(new Lien<int>(n1, n2, this.tempsChangement));
                        }
                    }
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
            }
        }

        public (List<Noeud<int>>, int) Dijkstra(Noeud<int> depart, Noeud<int> arrivee)
        {
            var distances = new Dictionary<Noeud<int>, int>();
            var precedent = new Dictionary<Noeud<int>, Noeud<int>>();
            var queue = new List<(int, Noeud<int>)>();

            foreach (var noeud in this.listeAdjacence.Keys)
            {
                distances[noeud] = int.MaxValue;
                precedent[noeud] = null;
            }
            distances[depart] = 0;
            queue.Add((0, depart));

            while (queue.Count > 0)
            {
                queue.Sort((a, b) => a.Item1.CompareTo(b.Item1));
                var (distActuelle, noeudActuel) = queue[0];
                queue.RemoveAt(0);

                if (Noeud<int>.isEqual(noeudActuel, arrivee)) break;

                foreach (var voisin in this.listeAdjacence[noeudActuel])
                {
                    int nouvelleDist = distActuelle + voisin.Value;
                    if (nouvelleDist < distances[voisin.Key])
                    {
                        distances[voisin.Key] = nouvelleDist;
                        precedent[voisin.Key] = noeudActuel;
                        queue.Add((nouvelleDist, voisin.Key));
                    }
                }
            }

            List<Noeud<int>> chemin = new List<Noeud<int>>();
            Noeud<int> courant = arrivee;
            while (courant != null)
            {
                chemin.Insert(0, courant);
                courant = precedent[courant];
            }

            if (distances[arrivee] == int.MaxValue)
                return (new List<Noeud<int>>(), -1);

            return (chemin, distances[arrivee]);
        }


        public List<Noeud<int>> TrouverStationsParNom(string nom)
        {
            return this.noeuds.Where(station => station.Nom == nom).ToList();
        }

        public (List<Noeud<int>>, int) TrouverMeilleurChemin(string nomDepart, string nomArrivee)
        {
            List<Noeud<int>> stationsDepart = TrouverStationsParNom(nomDepart);
            List<Noeud<int>> stationsArrivee = TrouverStationsParNom(nomArrivee);

            if (stationsDepart.Count == 0 || stationsArrivee.Count == 0)
            {
                Console.WriteLine("Erreur : Station non trouvée !");
                return (new List<Noeud<int>>(), -1);
            }

            List<Noeud<int>> meilleurChemin = new List<Noeud<int>>();
            int meilleurCout = int.MaxValue;

            foreach (var depart in stationsDepart)
            {
                foreach (var arrivee in stationsArrivee)
                {
                    var (chemin, cout) = Dijkstra(depart, arrivee);

                    if (chemin.Count > 0 && cout < meilleurCout)
                    {
                        meilleurChemin = chemin;
                        meilleurCout = cout;
                    }
                }
            }

            if (meilleurCout == int.MaxValue)
                return (new List<Noeud<int>>(), -1);

            return (meilleurChemin, meilleurCout);
        }


        //private void ChargerGrapheMTX(string fichier)
        //{
        //    try
        //    {
        //        string[] lignes = File.ReadAllLines(fichier);
        //        foreach (string ligne in lignes)
        //        {
        //            if (ligne[0] != '%')
        //            {
        //                string[] noeudsLigne = ligne.Split(' ');

        //                if (noeudsLigne.Length == 3)
        //                {
        //                    this.nbNoeuds = int.Parse(noeudsLigne[0]);
        //                    for (int i = 1; i <= nbNoeuds; i++)
        //                    {
        //                        this.listeAdjacence.Add(i, new List<Noeud>());
        //                    }
        //                }
        //                if (noeudsLigne.Length == 2)
        //                {

        //                    Noeud n1 = new Noeud(int.Parse(noeudsLigne[0]));
        //                    Noeud n2 = new Noeud(int.Parse(noeudsLigne[1]));

        //                    bool verif = true;
        //                    foreach (Noeud noeud in this.listeAdjacence[n1.Id])
        //                    {
        //                        if (noeud.isEqual(n2))
        //                        {
        //                            verif = false;
        //                        }
        //                    }

        //                    if (verif == true)
        //                    {
        //                        this.listeAdjacence[n1.Id].Add(n2);
        //                    }

        //                    verif = true;
        //                    foreach (Noeud noeud in this.listeAdjacence[n2.Id])
        //                    {
        //                        if (noeud.isEqual(n1))
        //                        {
        //                            verif = false;
        //                        }
        //                    }
        //                    if (verif == true)
        //                    {
        //                        this.listeAdjacence[n2.Id].Add(n1);
        //                    }

        //                    Lien l = new Lien(n1, n2);
        //                }
        //            }
        //        }

        //        for (int i = 1; i <= this.nbNoeuds; i++)
        //        {
        //            Console.Write("- " + i + " : ");
        //            foreach (var n in this.listeAdjacence[i])
        //            {
        //                Console.Write(n.Id + ", ");
        //            }
        //            Console.WriteLine();
        //        }

        //        this.matriceAdjacence = new int[this.nbNoeuds, this.nbNoeuds];
        //        foreach (string ligne in lignes)
        //        {
        //            if (ligne[0] != '%')
        //            {
        //                string[] noeudsLigne = ligne.Split(' ');
        //                if (noeudsLigne.Length == 2)
        //                {
        //                    matriceAdjacence[int.Parse(noeudsLigne[0]) - 1, int.Parse(noeudsLigne[1]) - 1]++;
        //                    matriceAdjacence[int.Parse(noeudsLigne[1]) - 1, int.Parse(noeudsLigne[0]) - 1]++;
        //                }
        //            }
        //        }
        //        for (int i = 0; i < matriceAdjacence.GetLength(0); i++)
        //        {
        //            for (int j = 0; j < matriceAdjacence.GetLength(0); j++)
        //            {
        //                Console.Write(matriceAdjacence[i, j]);
        //            }
        //            Console.WriteLine();
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("erreur : " + ex.Message);
        //    }
        //}

        /// <summary>
        /// Algorithme de parcours en profondeur d'abord (DFS) à partir du point de départ en entrée.
        /// </summary>
        /// <param name="depart">Noeud de départ de l'algorithme</param>
        /// <returns></returns>
        //    public List<Noeud> DFS(Noeud depart) //Parcours en profondeur d'abord
        //    {
        //        List<Noeud> noeudsParcourus = new List<Noeud>();
        //        Stack<Noeud> pile = new Stack<Noeud>();
        //        pile.Push(depart);

        //        while (pile.Count > 0)
        //        {
        //            Noeud noeudActuel = pile.Pop();
        //            bool dejaVisite = false;
        //            for (int i = 0; i < noeudsParcourus.Count; i++)
        //            {
        //                if (noeudsParcourus[i].isEqual(noeudActuel))
        //                {
        //                    dejaVisite = true;
        //                }
        //            }
        //            if (dejaVisite == false)
        //            {
        //                noeudsParcourus.Add(noeudActuel);

        //                foreach (Noeud voisin in listeAdjacence[noeudActuel.Id])
        //                {
        //                    bool dejaVisiteVoisin = false;
        //                    for (int j = 0; j < noeudsParcourus.Count; j++)
        //                    {
        //                        if (noeudsParcourus[j].isEqual(voisin))
        //                        {
        //                            dejaVisiteVoisin = true;
        //                        }
        //                    }
        //                    if (dejaVisiteVoisin == false)
        //                    {
        //                        pile.Push(voisin);
        //                    }
        //                }
        //            }
        //        }
        //        return noeudsParcourus;
        //    }

        //    /// <summary>
        //    /// Algorithme de parcours en largeur d'abord (BFS) à partir du point de départ en entrée.
        //    /// </summary>
        //    /// <param name="depart">Noeud de départ de l'algorithme</param>
        //    /// <returns></returns>
        //    public List<Noeud> BFS(Noeud depart)
        //    {
        //        List<Noeud> noeudsParcourus = new List<Noeud>();
        //        Queue<Noeud> file = new Queue<Noeud>();
        //        file.Enqueue(depart);
        //        noeudsParcourus.Add(depart);

        //        while (file.Count > 0)
        //        {
        //            Noeud noeudActuel = file.Dequeue();

        //            foreach (Noeud voisin in listeAdjacence[noeudActuel.Id])
        //            {
        //                bool dejaVisite = false;
        //                for (int i = 0; i < noeudsParcourus.Count; i++)
        //                {
        //                    if (noeudsParcourus[i].isEqual(voisin))
        //                    {
        //                        dejaVisite = true;
        //                    }
        //                }
        //                if (dejaVisite == false)
        //                {
        //                    noeudsParcourus.Add(voisin);
        //                    file.Enqueue(voisin);
        //                }
        //            }
        //        }
        //        return noeudsParcourus;
        //    }

        //    /// <summary>
        //    /// Indique si le graphe courrant est connexe.
        //    /// </summary>
        //    /// <returns></returns>
        //    public bool EstConnexe()
        //    {
        //        List<Noeud> noeudsParcourus = DFS(new Noeud(1));

        //        if (noeudsParcourus.Count == this.nbNoeuds)
        //        {
        //            return true;
        //        }

        //        return false;
        //    }

        //    /// <summary>
        //    /// Vérifie si le graphe contient des circuits (cycles) et les affiches dans la console.
        //    /// La methode se base sur l'algorithme du DFS (parcours en profondeur d'abord) mais est modifiée 
        //    /// pour essayer depuis tous les points du graphe et s'arrête dès qu'un cycle a été trouvé pour en 
        //    /// chercher un autre.
        //    /// </summary>
        //    /// <returns></returns>
        //    public bool ContientCicuits()
        //    {
        //        bool contientCicuits = false;
        //        for (int n = 1; n <= this.nbNoeuds; n++)
        //        {
        //            Noeud noeudDepart = new Noeud(n);

        //            List<Noeud> noeudsParcourus = new List<Noeud>();
        //            Stack<Noeud> pile = new Stack<Noeud>();
        //            Dictionary<int, int> parent = new Dictionary<int, int>();

        //            pile.Push(noeudDepart);
        //            parent[noeudDepart.Id] = -1;

        //            bool boucle = false;
        //            while (pile.Count > 0 && boucle == false)
        //            {
        //                Noeud noeudActuel = pile.Pop();

        //                bool dejaVisite = false;
        //                for (int i = 0; i < noeudsParcourus.Count; i++)
        //                {
        //                    if (noeudsParcourus[i].isEqual(noeudActuel))
        //                    {
        //                        dejaVisite = true;
        //                    }
        //                }
        //                if (dejaVisite == false)
        //                {
        //                    noeudsParcourus.Add(noeudActuel);

        //                    foreach (Noeud voisin in listeAdjacence[noeudActuel.Id])
        //                    {
        //                        bool dejaVisiteVoisin = false;
        //                        for (int j = 0; j < noeudsParcourus.Count; j++)
        //                        {
        //                            if (noeudsParcourus[j].isEqual(voisin))
        //                            {
        //                                dejaVisiteVoisin = true;
        //                            }
        //                        }
        //                        if (dejaVisiteVoisin == false)
        //                        {
        //                            parent[voisin.Id] = noeudActuel.Id;
        //                            pile.Push(voisin);
        //                        }
        //                        else if (parent[noeudActuel.Id] != voisin.Id)
        //                        {
        //                            contientCicuits = true;
        //                            boucle = true;
        //                        }
        //                    }
        //                }
        //            }
        //            Console.Write("Cycle " + n + " : ");
        //            AfficherListe(noeudsParcourus);
        //        }
        //        return contientCicuits;
        //    }

        //    /// <summary>
        //    /// Permet d'afficher dans la console une liste de Noeuds.
        //    /// </summary>
        //    /// <param name="liste">Liste de noeuds à afficher</param>
        //    private void AfficherListe(List<Noeud> liste)
        //    {
        //        for (int i = 0; i < liste.Count; i++)
        //        {
        //            Console.Write(liste[i].Id + ", ");
        //        }
        //        Console.WriteLine();
        //    }
    }
}