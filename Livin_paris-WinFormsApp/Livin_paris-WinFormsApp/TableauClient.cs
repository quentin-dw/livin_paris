using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using static Livin_paris_WinFormsApp.Outils;

namespace Livin_paris_WinFormsApp
{
    //faire truc qui affiche historique de ttes les transactions
    public class TableauClient
    {
        /// <summary>
        /// Gestion de la connexion d'un utilisateur vers sont compte client
        /// </summary>
        /// <param name="graphe"></param>
        public static void AffichageMenuClient(Graphe<int> graphe)
        {
            Client client = null;
            int choix = MenuCirculaire(4, "connexion", "associer un compte", "creer un compte", "trouver mon identifiant", "Menu Client");
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
                AssocierCompteClient();
                return;
            }
            else if (choix == 2)
            {
                client = CreationCompteClient(graphe);
                if (client == null)
                {
                    return;
                }
            }
            else if (choix == 3)
            {
                TrouverIdentifiantClient();
                return;
            }

            if(client == null)
            {
                return;
            }
            NouvelleCommande(client);
        }

        #region Connexion, Asociation, Creation de compte, Trouver identifiant
        static Client ConnexionClient()
        {
            Client client = null;

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
                id_compte = Convert.ToInt32(Demander("Entrez votre numero d'identifiant de compte (ou -1 pour sortir)", "int", true));
                if (id_compte == -1)
                {
                    return client;
                }

                string idExists = DQL_SQL($"SELECT EXISTS (SELECT * FROM client WHERE id_compte = {id_compte})", false)[0][0];

                if (idExists == "1")
                {
                    valide = true;
                    messageErreur = "";
                } else
                {
                    if (DQL_SQL($"SELECT EXISTS (SELECT * FROM compte WHERE id_compte = {id_compte})", false)[0][0] == "1")
                    {
                        messageErreur = " Identifiant non associé à un compte client (veuillez d'abord l'associer) ";
                    }
                    else
                    {
                        messageErreur = " Identifiant invalide ";
                    }
                }
            }

            valide = false;

            client = new Client(id_compte);
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
                
                string mot_de_passe = Demander("Entrez votre mot de passe", "mdp", true);
                if (mot_de_passe == client.Mot_de_passe)
                {
                    valide = true;
                    messageErreur = "";
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

        static void AssocierCompteClient()
        {
            Console.ResetColor();
            Console.Clear();

            Console.SetCursorPosition(1, 0);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(" ▪ " + "Associer un compte existant ");
            Console.ResetColor();
            Console.WriteLine();

            int id_compte = Convert.ToInt32(Demander("Entrez votre identifiant de compte", "int", true));

            string existsCompte = DQL_SQL($"SELECT EXISTS(SELECT * FROM Compte WHERE id_compte = {id_compte})", false)[0][0];
            if (existsCompte != "1")
            {
                Console.WriteLine("Compte introuvable. Opération annulée.");
                Thread.Sleep(1500);
                return;
            }

            string cuisinierExists = DQL_SQL($"SELECT EXISTS(SELECT * FROM Client WHERE id_compte = {id_compte})", false)[0][0];
            if (cuisinierExists == "1")
            {
                Console.WriteLine("Ce compte est déjà associé à un profil client.");
                Thread.Sleep(1500);
                return;
            }

            string isEntreprise = Demander("S'agit-il d'une entreprise ? (oui/non)", "bool", true);
            bool entreprise = isEntreprise == "True";
            string nomEntreprise = "NULL";
            if (entreprise)
            {
                string nomEnt = Demander("Nom de l'entreprise", "string", true);
                nomEntreprise = $"'{nomEnt}'";
            }

            string insertRequete = $"INSERT INTO Client (entreprise, nom_entreprise, id_compte) VALUES ({(entreprise ? 1 : 0)}, {nomEntreprise}, {id_compte});";
            bool ok = DML_SQL(insertRequete);
            if (!ok)
            {
                Console.WriteLine("Erreur lors de l'association. Réessayez.");
                Thread.Sleep(1500);
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Compte associé comme client ✅");
            Console.ResetColor();
            Thread.Sleep(1500);
        }

        static Client CreationCompteClient(Graphe<int> graphe)
        {
            Client client = null;

            Console.ResetColor();
            Console.Clear();

            Console.SetCursorPosition(1, 0);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(" ▪ " + "Creation compte ");
            Console.ResetColor();
            Console.WriteLine();


            string reponse = "";
            string entreprise = "", prenom = "", nom = "", nom_entreprise = "", telephone, rue, numero, code_postal, ville, metro_le_plus_proche, email, mot_de_passe;

            string viaJSON = Demander("Voulez-vous importer votre profil depuis le fichier nouveauClient.json ? [oui/non]", "string", true);
            if (viaJSON.ToLower() == "oui")
            {
                try
                {
                    string jsonString = File.ReadAllText("../../../nouveauClient.json");

                    using JsonDocument doc = JsonDocument.Parse(jsonString);
                    JsonElement root = doc.RootElement;

                    bool statut_entreprise = root.GetProperty("entreprise").GetBoolean();
                    if (statut_entreprise)
                    {
                        entreprise = "TRUE";
                        nom_entreprise = root.GetProperty("nom_entreprise").GetString();
                    }
                    else
                    {
                        entreprise = "FALSE";
                        nom_entreprise = "NULL";
                    }
                    prenom = root.GetProperty("prenom").GetString().ToLower();
                    nom = root.GetProperty("nom").GetString().ToLower();
                    telephone = root.GetProperty("numero_telephone").GetString().ToLower();
                    numero = root.GetProperty("numero_residence").GetString().ToLower();
                    rue = root.GetProperty("rue").GetString().ToLower();
                    ville = root.GetProperty("ville").GetString().ToLower();
                    code_postal = root.GetProperty("code_postal").GetString().ToLower();
                    metro_le_plus_proche = root.GetProperty("metro_le_plus_proche").GetString().ToLower();
                    email = root.GetProperty("email").GetString().ToLower();
                    mot_de_passe = root.GetProperty("mot_de_passe").GetString().ToLower();
                }
                catch (Exception ex) 
                { 
                    Console.WriteLine("Une erreur est survenue, nous allons créer le compte autrement");
                    Console.WriteLine("Champs obligatoires marqués par *");


                    reponse = Demander("Representez-vous une entreprise ? [Oui/Non]", "string", true);
                    if (reponse.ToLower() == "oui")
                    {
                        entreprise = "TRUE";

                        prenom = Demander("Prénom référent", "string", true);

                        nom = Demander("Nom référent", "string", true);

                        nom_entreprise = Demander("Nom de l'entreprise", "string", true);

                    }
                    else if (reponse.ToLower() == "non")
                    {
                        entreprise = "FALSE";

                        prenom = Demander("Prénom", "string", true);

                        nom = Demander("Nom", "string", true);

                        nom_entreprise = "NULL";
                    }

                    telephone = Demander("Numéro de téléphone", "numTel", true);

                    numero = Demander("Numéro de rue de résidence", "int", true);

                    rue = Demander("Nom de rue de résidence", "string", true);

                    ville = Demander("Ville de résidence", "string", true);

                    code_postal = Demander("Code postal de ville de résidence", "int", true);

                    metro_le_plus_proche = Demander("Station de metro la plus proche", "station", true, graphe);

                    email = Demander("Adresse e-mail", "email", true);

                    mot_de_passe = Demander("Mot de passe", "mdp", true);
                }
            }

            else
            {
                Console.WriteLine("Champs obligatoires marqués par *");


                reponse = Demander("Representez-vous une entreprise ? [Oui/Non]", "string", true);
                if (reponse.ToLower() == "oui")
                {
                    entreprise = "TRUE";

                    prenom = Demander("Prénom référent", "string", true);

                    nom = Demander("Nom référent", "string", true);

                    nom_entreprise = Demander("Nom de l'entreprise", "string", true);

                }
                else if (reponse.ToLower() == "non")
                {
                    entreprise = "FALSE";

                    prenom = Demander("Prénom", "string", true);

                    nom = Demander("Nom", "string", true);

                    nom_entreprise = "NULL";
                }

                telephone = Demander("Numéro de téléphone", "numTel", true);

                numero = Demander("Numéro de rue de résidence", "int", true);

                rue = Demander("Nom de rue de résidence", "string", true);

                ville = Demander("Ville de résidence", "string", true);

                code_postal = Demander("Code postal de ville de résidence", "int", true);

                metro_le_plus_proche = Demander("Station de metro la plus proche", "station", true, graphe);


                email = Demander("Adresse e-mail", "email", true);

                mot_de_passe = Demander("Mot de passe", "mdp", true);
            }



            string requeteInsertCompte = $"INSERT INTO Compte (prenom, nom, telephone, rue, numero, code_postal, ville, metro_le_plus_proche, email, mot_de_passe) VALUES ('{prenom}', '{nom}', '{telephone}', '{rue}', {Convert.ToInt32(numero)}, {Convert.ToInt32(code_postal)}, '{ville}', '{metro_le_plus_proche}', '{email}', '{mot_de_passe}');";
            
            bool InsertCompte = DML_SQL(requeteInsertCompte);

            string requete = "SELECT LAST_INSERT_ID();";
            int id_compte = Convert.ToInt32(DQL_SQL(requete, false)[0][0]);

            string requeteInsertClient = $"INSERT INTO Client (entreprise, nom_entreprise, id_compte) VALUES ({entreprise}, '{nom_entreprise}', {id_compte});";
            InsertCompte = DML_SQL(requeteInsertClient);

            if (InsertCompte)
            {
                client = new Client(id_compte);

                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine();
                Console.WriteLine(" Compte Client Créé ✅ ");

                Console.ResetColor();
                Console.WriteLine();

                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.Write(" Votre n° d'identifiant de compte : ");
                Console.BackgroundColor = ConsoleColor.Magenta;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(id_compte + " ");

                Console.ResetColor();
                Console.WriteLine("\n");

                Thread.Sleep(3000);
            }

            return client;
        }

        static void TrouverIdentifiantClient()
        {
            Console.ResetColor();
            Console.Clear();

            Console.SetCursorPosition(1, 0);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(" ▪ " + "Trouver mon identifiant ");
            Console.ResetColor();
            Console.WriteLine();

            Console.WriteLine("Les moyens d'authentification disponible sont : ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" 1. ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("email");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" 2. ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("numero de telephone");
            Console.WriteLine();
            int modeAuth = Convert.ToInt32(Demander("Quel moyen d'authentification souhaitez vous utiliser ? [1/2]", "int", true));
            Console.WriteLine();

            if (modeAuth == 1)
            {
                string email = Demander("Entrez votre adresse e-mail", "string", true);
                Console.WriteLine();

                if (Convert.ToInt32(DQL_SQL($"SELECT EXISTS (SELECT * FROM compte WHERE email='{email}')", false)[0][0])==0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine("L'adresse email entrée est incorrecte, veuillez réessayer");
                    Thread.Sleep(3000);
                    return;
                }

                string[] retourRequete = DQL_SQL($"SELECT prenom, nom, id_compte FROM compte WHERE email='{email}'", false)[0];
                string prenom = retourRequete[0];
                string nom = retourRequete[1];
                string id_compte = retourRequete[2];

                Console.Write("Ce compte appartient à ");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(prenom + " " + nom);
                Console.ResetColor();

                Console.Write("Votre numéro d'identifiant de compte : ");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write(id_compte);
            }
            else if (modeAuth == 2)
            {
                string telephone = Demander("Entrez votre numero de telephone", "string", true);
                Console.WriteLine();

                if (Convert.ToInt32(DQL_SQL($"SELECT EXISTS (SELECT * FROM compte WHERE telephone='{telephone}')", false)[0][0])==0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine("Le numéro de téléphone entré est incorrecte, veuillez réessayer");
                    Thread.Sleep(3000);
                    return;
                }

                string[] retourRequete = DQL_SQL($"SELECT prenom, nom, id_compte FROM compte WHERE telephone='{telephone}'", false)[0];
                string prenom = retourRequete[0];
                string nom = retourRequete[1];
                string id_compte = retourRequete[2];

                Console.Write("Ce compte appartient à ");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(prenom + " " + nom);
                Console.ResetColor();

                Console.Write("Votre numéro d'identifiant de compte : ");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(id_compte);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nUne erreur s'est produite, veuillez reesayer");
            }
            Console.ResetColor();
            Console.WriteLine("\n\nPressez ENTREE pour sortir");
            Console.ReadLine();
        }

        #endregion

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
                string metro_le_plus_proche_cusinier = DQL_SQL($"SELECT metro_le_plus_proche FROM compte WHERE id_compte ={id_compte_cuisinier};", false)[0][0];

                string noeuds = "../../../../../noeuds.csv";
                string arcs = "../../../../../arcs.csv";

                Console.WriteLine("station depart : " + metro_le_plus_proche_cusinier);
                Console.WriteLine("station arrivée : " + client.Metro_le_plus_proche);

                Graphe<int> graphe = new Graphe<int>(noeuds, arcs);
                string depart = metro_le_plus_proche_cusinier;
                string arrivee = client.Metro_le_plus_proche;
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
