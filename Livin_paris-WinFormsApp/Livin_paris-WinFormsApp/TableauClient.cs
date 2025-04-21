using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Livin_paris_WinFormsApp.Outils;

namespace Livin_paris_WinFormsApp
{
    public class TableauClient
    {
        public static void AffichageMenuClient()
        {
            Client client = null;
            int choix = MenuCirculaire(3, "connexion", "associer un compte", "creer un compte", "", "Menu Client");
            if (choix == -2)
            {
                return;
            }
            else if (choix == 0)
            {
                client = ConnexionClient();
            }
            else if (choix == 1)
            {
                //client = ConnexionClient();
            }
            else if (choix == 2)
            {
                //client = ConnexionClient();
            }

            NouvelleCommande(client);
        }

        static Client ConnexionClient()
        {
            Console.ResetColor();
            Console.Clear();

            Console.SetCursorPosition(1, 0);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(" ▪ " + "Connexion ");
            Console.WriteLine();

            bool valide = false;
            string messageErreur = "";
            int id_compte = -1;

            while (!valide)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(messageErreur);

                Console.ResetColor();
                id_compte = Convert.ToInt32(Demander("Entrez votre numero d'identifiant de compte", "int", true));
                string idExists = DQL_SQL($"SELECT EXISTS (SELECT * FROM compte WHERE id_compte = {id_compte})", false)[0][0];


                if (idExists == "1")
                {
                    valide = true;
                } else
                {
                    messageErreur = " Identifiant invalide ";
                }
            }

            valide = false;

            Client client = new Client(id_compte);
            if (client.Entreprise)
            {
                Console.WriteLine("Bienvenue " + client.Nom_entreprise);
            }
            else
            {
                Console.WriteLine("Bienvenue " + client.Prenom + " " + client.Nom);
            }

            while (!valide)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(messageErreur);

                Console.ResetColor();
                
                string mot_de_passe = Demander("Entrez votre mot de passe", "string", true);
                if (mot_de_passe == client.Mot_de_passe)
                {
                    valide = true;
                }
                else
                {
                    messageErreur = " Mot de passe invalide ";
                }
            }

            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine();
            Console.WriteLine(" Connexion réussie ✅ ");
            Thread.Sleep(1500);

            Console.ResetColor();

            return client;
        }

        /// <summary>
        /// Permet de créer de nouvelles commandes en faisant apparaître l'interface appropriée
        /// </summary>
        static void NouvelleCommande(Client client)
        {
            Console.ResetColor();
            Console.Clear();
            Console.SetCursorPosition(1, 0);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(" ▪ " + "Nouvelle Commande ");
            Console.WriteLine();
            Console.ResetColor();

            DML_SQL($"INSERT INTO commande (avis_client, note_client, cout_total, id_client) VALUES (NULL, NULL, 0, {client.Id_client})");
            string requete_id_commande = "SELECT LAST_INSERT_ID();";
            string id_commande = DQL_SQL(requete_id_commande, false)[0][0];

            bool continuerAjout = true;
            while (continuerAjout)
            {
                Console.BackgroundColor = ConsoleColor.DarkYellow;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" Liste des plats disponibles aujourd'hui : ");
                Console.ResetColor();
                Console.WriteLine();

                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write("id_plat | nom_plat | prix | type | date_fabrication | date_peremption | nationnalite_de_cuisine | regime_alimentaire");
                Console.ResetColor();
                Console.WriteLine();

                string plats_dispo_requete = "SELECT id_plat, nom_plat, prix, type, date_fabrication, date_peremption, nationnalite_de_cuisine, regime_alimentaire FROM plat WHERE date_peremption >= CURDATE() AND date_fabrication <= CURDATE();";
                DQL_SQL(plats_dispo_requete, true);
                Console.WriteLine();

                string id_plat = Demander("Entrez l'identifiant du plat que vous souhaitez", "int", true);
                int quantite = Convert.ToInt32(Demander("Quelle quantité souhaitez vous ?", "int", true));
                string date_livraison = Demander("A quelle date souhaitez vous vous le faire livrer ? (Format : AAAA-MM-JJ)", "string", true);

                string cout = DQL_SQL($"SELECT prix FROM plat WHERE id_plat={id_plat};", false)[0][0];
                string cout_bon_format = "";
                for (int i = 0; i < cout.Length; i++)
                {
                    if (cout[i] == ',')
                    {
                        cout_bon_format += '.';
                    }
                    else if (cout[i] == ' ')
                    {

                        cout_bon_format += "";
                    }
                    else
                    {
                        cout_bon_format += cout[i];
                    }
                }

                string insertion_plat = $"INSERT INTO Ligne_de_commande (lieu_livraison, quantite, date_livraison, cout, id_commande, id_plat) VALUES ('{client.Adresse()}', '{Convert.ToString(quantite)}', '{date_livraison}', {cout_bon_format}, {id_commande}, {id_plat});";
                bool insertionReussie = DML_SQL(insertion_plat);
                if (insertionReussie)
                {
                    double cout_total_actuel = Convert.ToDouble(DQL_SQL($"SELECT cout_total FROM commande WHERE id_commande={id_commande};", false)[0][0]);
                    string cout_total_maj = Convert.ToString(cout_total_actuel + (Convert.ToDouble(cout) * quantite));
                    string cout_total_maj_bon_format = "";

                    for (int i = 0; i < cout_total_maj.Length; i++)
                    {
                        if (cout_total_maj[i] == ',')
                        {
                            cout_total_maj_bon_format += '.';
                        }
                        else if (cout_total_maj[i] == ' ')
                        {

                            cout_total_maj_bon_format += "";
                        }
                        else
                        {
                            cout_total_maj_bon_format += cout_total_maj[i];
                        }
                    }

                    DML_SQL($"UPDATE commande SET cout_total = {cout_total_maj_bon_format} WHERE id_commande = {id_commande};");
                }
                else
                {
                    DML_SQL($"DELETE FROM commande WHERE id_commande = {id_commande};");
                    DML_SQL($"DELETE FROM ligne_de_commande WHERE id_commande = {id_commande};");
                    Console.WriteLine("Une des entrées est incorrecte, appuyez sur la touche ENTREE puis réessayez");
                    Console.ReadLine();
                    return;
                }

                continuerAjout = Convert.ToBoolean(Demander("Voulez vous commander un autre plat ? [Oui/Non]", "bool", true));
            }

            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" Résumé de la commande : ");
            Console.ResetColor();
            Console.WriteLine();

            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("nom_plat | prix | quantite | date_livraison");
            Console.ResetColor();
            Console.WriteLine();

            string affichage_commande_requete = $"SELECT P.nom_plat, LDC.cout, LDC.quantite, LDC.date_livraison FROM plat P JOIN Ligne_de_commande LDC ON P.id_plat = LDC.id_plat WHERE LDC.id_commande = {id_commande};";
            DQL_SQL(affichage_commande_requete, true);

            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" Cout total : ");
            Console.ResetColor();

            string cout_total = DQL_SQL($"SELECT cout_total FROM commande WHERE id_commande = {id_commande}", false)[0][0];
            Console.WriteLine(" " + cout_total);

            Console.WriteLine();

            bool validation = Convert.ToBoolean(Demander("Souhaitez vous valider la transaction ? [Oui/Non]", "bool", true));
            if (validation)
            {
                Console.WriteLine("Transaction confirmée");
            }
            else
            {
                Console.WriteLine("Commande annulée");
                DML_SQL($"DELETE FROM commande WHERE id_commande = {id_commande};");
                DML_SQL($"DELETE FROM ligne_de_commande WHERE id_commande = {id_commande};");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Livraison à votre adresse...");

            List<string[]> id_ligne_de_commande = DQL_SQL($"SELECT LDC.id_ligne_de_commande FROM ligne_de_commande LDC WHERE id_commande = {id_commande}", false);
            Console.WriteLine("Nombre de commandes à livrer : " + id_ligne_de_commande.Count + "\n");

            for (int i = 0; i < id_ligne_de_commande.Count; i++)
            {
                Console.WriteLine($"Livraison {i + 1}e ligne de commande ");
                string id_cuisinier = DQL_SQL($"SELECT P.id_cuisinier FROM Plat P JOIN ligne_de_commande ldc ON P.id_plat = ldc.id_plat WHERE id_ligne_de_commande = {id_ligne_de_commande[i][0]};", false)[0][0];
                string id_compte_cuisinier = DQL_SQL($"SELECT Cpt.id_compte FROM Compte Cpt JOIN cuisinier Cui ON cpt.id_compte = cui.id_compte WHERE cui.id_cuisinier = {id_cuisinier};", false)[0][0];
                string metro_le_plus_proche_client = DQL_SQL($"SELECT lieu_livraison FROM ligne_de_commande WHERE id_ligne_de_commande = {id_ligne_de_commande[i][0]};", false)[0][0];
                string metro_le_plus_proche_cusinier = DQL_SQL($"SELECT metro_le_plus_proche FROM compte WHERE id_compte ={id_compte_cuisinier};", false)[0][0];

                string noeuds = "../../../../../noeuds.csv";
                string arcs = "../../../../../arcs.csv";

                Console.WriteLine("station depart : " + metro_le_plus_proche_cusinier);
                Console.WriteLine("station arrivée : " + metro_le_plus_proche_client);

                Graphe<int> graphe = new Graphe<int>(noeuds, arcs);
                string depart = metro_le_plus_proche_cusinier;
                string arrivee = metro_le_plus_proche_client;
                var (chemin, coutG) = graphe.TrouverMeilleurChemin(depart, arrivee);

                if (chemin.Count == 0)
                    Console.WriteLine("Aucun chemin trouvé.");
                else
                    Console.WriteLine($"Plus court chemin ({coutG} min) :\n {string.Join("\n -> ", chemin)}");
            }
            Console.WriteLine("\n Pressez la touche ENTREE pour sortir ");
            Console.ReadLine();
        }
    }
}
