using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using static Livin_paris_WinFormsApp.Outils;

namespace Livin_paris_WinFormsApp
{

    public class TableauAdmin
    {
        public static void AffichageMenuAdmin(Graphe<int> graphe)
        {
            Console.ResetColor();
            Console.Clear();

            bool finProgramme = false;

            while (!finProgramme)
            {
                bool entreeCorrecte = false;
                string messageErreur = "";
                while (!entreeCorrecte)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(messageErreur);
                    Console.ResetColor();

                    Console.WriteLine("Interface Administrateur");

                    int reponse = MenuCirculaire(4, "Module client", "Module cuisinier", "Module commande", "module statistiques", "Menu Administrateur");
                    Console.ResetColor();
                    switch (reponse)
                    {
                        case 0:
                            entreeCorrecte = true;
                            ModuleClient(graphe);
                            break;
                        case 1:
                            entreeCorrecte = true;
                            ModuleCuisinier(graphe);
                            break;
                        case 2:
                            entreeCorrecte = true;
                            ModuleCommande(graphe);
                            break;
                        case 3:
                            entreeCorrecte = true;
                            moduleStats();
                            break;
                        case -2:
                            entreeCorrecte = true;
                            finProgramme = true;
                            break;
                        default:
                            messageErreur = "ERREUR : Entrée incorrecte !";
                            break;

                    }
                }
            }
        }



        #region Module CLIENT
        /// <summary>
        /// Affichage et gestion du module Client
        /// </summary>
        static void ModuleClient(Graphe<int> graphe)
        {
            bool end = false;
            while (!end)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();

                Console.WriteLine("Utilisez les touches flechées de votre clavier");
                Console.WriteLine("Appuyez sur ECHAP pour sortir");

                string libelle = "";
                int width = Console.WindowWidth;
                int height = Console.WindowHeight;

                libelle = " AJOUTER CLIENT ";
                Console.SetCursorPosition((width / 2) - (libelle.Length / 2), height / 4);
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(libelle);
                Console.ResetColor();

                libelle = " MODIFIER CLIENT ";
                Console.SetCursorPosition((width / 4) - (libelle.Length / 2), height / 2);
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine(libelle);
                Console.ResetColor();

                libelle = " AFFICHER CLIENT ";
                Console.SetCursorPosition((width / 2) - (libelle.Length / 2), 3 * (height / 4) + 2);
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(libelle);
                Console.ResetColor();

                libelle = " SUPPRIMER CLIENT ";
                Console.SetCursorPosition(3 * (width / 4) - (libelle.Length / 2), height / 2);
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(libelle);
                Console.ResetColor();


                Console.SetCursorPosition(width / 2, height / 2 - 1);
                Console.Write("▲");

                Console.SetCursorPosition(width / 2 - 2, height / 2);
                Console.Write("◀ ■ ▶");

                Console.SetCursorPosition(width / 2, height / 2 + 1);
                Console.Write("▼");

                Console.SetCursorPosition(width / 2, height / 2 + 2);

                bool entreeCorrecte = false;
                while (!entreeCorrecte)
                {
                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                        if (keyInfo.Key == ConsoleKey.UpArrow)
                        {
                            entreeCorrecte = true;
                            AjouterClient(graphe);
                        }
                        else if (keyInfo.Key == ConsoleKey.LeftArrow)
                        {
                            entreeCorrecte = true;
                            ModifierClient();
                        }
                        else if (keyInfo.Key == ConsoleKey.DownArrow)
                        {
                            entreeCorrecte = true;
                            AfficherClient();
                        }
                        else if (keyInfo.Key == ConsoleKey.RightArrow)
                        {
                            entreeCorrecte = true;
                            SupprimerClient();
                        }
                        else if (keyInfo.Key == ConsoleKey.Escape)
                        {
                            entreeCorrecte = true;
                            end = true;
                        }
                    }

                    Thread.Sleep(50);
                }
            }

        }
        /// <summary>
        /// Permet d'ajouter un client en faisant apparaitre l'interface appropriée
        /// </summary>
        /// <returns>Retourne l'id du client ajouté</returns>
        static int AjouterClient(Graphe<int> graphe)
        {
            Console.Clear();
            Console.WriteLine("Ajout client\n");

            string reponse = "";
            string entreprise = "", prenom = "", nom = "", nom_entreprise = "", telephone, rue, numero, code_postal, ville, metro_le_plus_proche, email, mot_de_passe;
            Console.WriteLine("Champs obligatoires marqués par *");


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
                    metro_le_plus_proche = root.GetProperty("metro_le_plus_proche").GetString();
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


                reponse = Demander("Representez-vous une entreprise ? [Oui/Non]", "bool", true);
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

            if (InsertCompte)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Réussi ✅");
                Console.ResetColor();
            }

            string requete = "SELECT LAST_INSERT_ID();";
            List<string[]> resultat_id_compte = DQL_SQL(requete, false);

            string requeteInsertClient = $"INSERT INTO Client (entreprise, nom_entreprise, id_compte) VALUES ({entreprise}, '{nom_entreprise}', {Convert.ToInt32(resultat_id_compte[0][0])});";
            InsertCompte = DML_SQL(requeteInsertClient);

            if (InsertCompte)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Réussi ✅");
                Console.ResetColor();
            }

            string requete2 = "SELECT LAST_INSERT_ID();";
            string[] resultat_id_client = DQL_SQL(requete2, false)[0];
            Console.WriteLine("\nRequête executée, identifiant du nouveau client : " + resultat_id_client[0]);

            Console.BackgroundColor = ConsoleColor.Magenta;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n Pressez la touche ENTREE pour continuer ");
            Console.ReadLine();

            return Convert.ToInt32(resultat_id_client[0]);
        }

        /// <summary>
        /// Permet de modifier un client en faisant apparaitre l'interface appropriée
        /// </summary>
        static void ModifierClient()
        {
            Console.Clear();
            Console.WriteLine("Modification client\n");

            string reponse = "";
            Console.WriteLine("Champs obligatoires marqués par *\n");

            string id_client = Demander("Entrez l'id du client à modifier", "int", true);

            string table_client_requete = $"SELECT * FROM Client WHERE id_client = {Convert.ToInt32(id_client)};";
            string[] table_client = DQL_SQL(table_client_requete, false)[0];

            string[] colonnesClient = new string[] { "id_client", "entreprise", "nom_entreprise", "id_compte" };

            Console.WriteLine("Table CLIENT :");
            Console.WriteLine("id_client | entreprise | nom_entreprise | id_compte |");
            for (int i = 0; i < table_client.Length; i++)
            {
                Console.Write(table_client[i] + " | ");
            }

            string id_compte_requete = $"SELECT id_compte FROM Client WHERE id_client = {Convert.ToInt32(id_client)};";
            string id_compte = DQL_SQL(id_compte_requete, false)[0][0];

            string table_compte_requete = $"SELECT * FROM Compte WHERE id_compte = {Convert.ToInt32(id_compte)};";
            string[] table_compte = DQL_SQL(table_compte_requete, false)[0];

            string[] colonnesCompte = new string[] { "id_compte", "prenom", "nom", "telephone", "rue", "numero", "code_postal", "ville", "metro_le_plus_proche", "email", "mot_de_passe" };

            Console.WriteLine("\n\nTable COMPTE :");
            Console.WriteLine("id_compte | prenom | nom | telephone | rue | numero | code_postal | ville | metro_le_plus_proche | email | mot_de_passe");
            for (int i = 0; i < table_compte.Length; i++)
            {
                Console.Write(table_compte[i] + " | ");
            }
            Console.WriteLine();

            int nbrColonnesUpdateClient = Convert.ToInt32(Demander("Combien de colonnes souhaitez vous modifier dans la table CLIENT?", "int", true));
            string[] colonnesAModifClient = new string[nbrColonnesUpdateClient];

            int nbrColonnesUpdateCompte = Convert.ToInt32(Demander("Combien de colonnes souhaitez vous modifier dans la table COMPTE?", "int", true));
            string[] colonnesAModifCompte = new string[nbrColonnesUpdateCompte];



            string ColValClient = "";
            Console.WriteLine("Dans la table CLIENT :");
            for (int i = 0; i < nbrColonnesUpdateClient; i++)
            {
                bool existe = false;
                while (!existe)
                {
                    string colonne = Demander("Entrez le nom de la " + i + 1 + "e colonne a modifier dans Client", "string", true).ToLower();

                    for (int j = 0; j < colonnesClient.Length; j++)
                    {
                        if (colonne == colonnesClient[j])
                        {
                            existe = true;
                            colonnesAModifClient[i] = colonne;
                        }
                    }
                }
                string modif = Demander("Entrez la nouvelle valeur de la " + i + 1 + "e colonne", "string", true);

                if (i == nbrColonnesUpdateClient - 1)
                {
                    ColValClient += (colonnesAModifClient[i] + " = " + $"'{modif}'");
                }
                else
                {
                    ColValClient += (colonnesAModifClient[i] + " = " + $"'{modif}'" + ", ");
                }
            }

            string ColValCompte = "";
            Console.WriteLine("Dans la table COMPTE :");
            for (int i = 0; i < nbrColonnesUpdateCompte; i++)
            {
                bool existe = false;
                while (!existe)
                {
                    string colonne = Demander("Entrez le nom de la " + (i + 1) + "e colonne a modifier dans Compte", "string", true);

                    for (int j = 0; j < colonnesCompte.Length && existe == false; j++)
                    {
                        if (colonne == colonnesCompte[j])
                        {
                            existe = true;
                            colonnesAModifCompte[i] = colonne;
                        }
                    }
                }

                string modif = Demander("Entrez la nouvelle valeur de la " + i + 1 + "e colonne", "string", true);

                if (i == nbrColonnesUpdateCompte - 1)
                {
                    ColValCompte += (colonnesAModifCompte[i] + " = " + $"'{modif}'");
                }
                else
                {
                    ColValCompte += (colonnesAModifCompte[i] + " = " + $"'{modif}'" + ", ");
                }
            }

            if (ColValClient.Length > 0)
            {
                string modif_client_requete = $"UPDATE Client SET {ColValClient} WHERE id_compte = {Convert.ToInt32(id_compte)};";
                bool modif_client = DML_SQL(modif_client_requete);

                if (modif_client)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Réussi ✅");
                    Console.ResetColor();
                }
            }

            if (ColValCompte.Length > 0)
            {
                string modif_compte_requete = $"UPDATE Compte SET {ColValCompte} WHERE id_compte = {Convert.ToInt32(id_compte)};";
                bool modif_compte = DML_SQL(modif_compte_requete);

                if (modif_compte)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Réussi ✅");
                    Console.ResetColor();
                }
            }

            Console.BackgroundColor = ConsoleColor.Magenta;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n Pressez la touche ENTREE pour sortir ");
            Console.ReadLine();
        }

        /// <summary>
        /// Permet de supprimer un client en faisant apparaitre l'interface appropriée
        /// </summary>
        static void SupprimerClient()
        {
            Console.Clear();
            Console.WriteLine("Suppression client\n");

            string reponse = "";
            Console.WriteLine("Champs obligatoires marqués par *");

            string id_client = Demander("Entrez l'id du client à supprimer", "int", true);

            string id_compte_requete = $"SELECT id_compte FROM Client WHERE id_client = {Convert.ToInt32(id_client)};";
            string id_compte = DQL_SQL(id_compte_requete, false)[0][0];

            string requeteDeleteClient = $"DELETE FROM Client WHERE id_client = {Convert.ToInt32(id_client)};";
            bool deleteClient = DML_SQL(requeteDeleteClient);

            string requeteDeleteCompte = $"DELETE FROM Compte WHERE id_compte = {Convert.ToInt32(id_compte)};";
            bool deleteCompte = DML_SQL(requeteDeleteCompte);

            if (deleteCompte && deleteClient)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Réussi ✅");
                Console.ResetColor();
            }

            Console.WriteLine("\n Pressez la touche ENTREE pour sortir ");
            Console.ReadLine();
        }

        /// <summary>
        /// Permet d'afficher un client en faisant apparaitre l'interface appropriée
        /// </summary>
        static void AfficherClient()
        {
            Console.Clear();

            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Ordonné par ordre alpahabétique");
            Console.ResetColor();
            Console.WriteLine();

            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("id_compte | prenom | nom | telephone | rue | numero | code_postal | ville | metro_le_plus_proche | email | mot_de_passe | id_client | entreprise | nom_entreprise | id_compte");
            Console.ResetColor();
            Console.WriteLine();
            string table_client_requete = $"SELECT * FROM Compte JOIN Client ON Compte.id_compte = Client.id_compte ORDER BY Compte.nom;";
            DQL_SQL(table_client_requete, true);

            Console.WriteLine("\n");

            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Ordonné par rue");
            Console.ResetColor();
            Console.WriteLine();

            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("id_compte | prenom | nom | telephone | rue | numero | code_postal | ville | metro_le_plus_proche | email | mot_de_passe | id_client | entreprise | nom_entreprise | id_compte");
            Console.ResetColor();
            Console.WriteLine();
            table_client_requete = $"SELECT * FROM Compte JOIN Client ON Compte.id_compte = Client.id_compte ORDER BY Compte.rue;";
            DQL_SQL(table_client_requete, true);


            Console.WriteLine("\n Pressez la touche ENTREE pour sortir ");
            Console.ReadLine();

        }
        #endregion


        #region Module CUISINIER
        /// <summary>
        /// Affiche et gère le module cuisinier
        /// </summary>
        static void ModuleCuisinier(Graphe<int> graphe)
        {
            bool end = false;
            while (!end)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();
                Console.WriteLine("Utilisez les touches flechées de votre clavier");
                Console.WriteLine("Appuyez sur ECHAP pour sortir");
                string libelle = "";
                int width = Console.WindowWidth;
                int height = Console.WindowHeight;

                libelle = " AJOUTER CUISINIER ";
                Console.SetCursorPosition((width / 2) - (libelle.Length / 2), height / 4);
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(libelle);
                Console.ResetColor();

                libelle = " MODIFIER CUISINIER ";
                Console.SetCursorPosition((width / 4) - (libelle.Length / 2), height / 2);
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine(libelle);
                Console.ResetColor();

                libelle = " AFFICHER CUISINIER ";
                Console.SetCursorPosition((width / 2) - (libelle.Length / 2), 3 * (height / 4) + 2);
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(libelle);
                Console.ResetColor();

                libelle = " SUPPRIMER CUISINIER ";
                Console.SetCursorPosition(3 * (width / 4) - (libelle.Length / 2), height / 2);
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(libelle);
                Console.ResetColor();


                Console.SetCursorPosition(width / 2, height / 2 - 1);
                Console.Write("▲");

                Console.SetCursorPosition(width / 2 - 2, height / 2);
                Console.Write("◀ ■ ▶");

                Console.SetCursorPosition(width / 2, height / 2 + 1);
                Console.Write("▼");

                Console.SetCursorPosition(width / 2, height / 2 + 2);

                bool entreeCorrecte = false;
                while (!entreeCorrecte)
                {
                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                        if (keyInfo.Key == ConsoleKey.UpArrow)
                        {
                            entreeCorrecte = true;
                            AjouterCuisinier(graphe);
                        }
                        else if (keyInfo.Key == ConsoleKey.LeftArrow)
                        {
                            entreeCorrecte = true;
                            ModifierCuisinier();
                        }
                        else if (keyInfo.Key == ConsoleKey.DownArrow)
                        {
                            entreeCorrecte = true;
                            AfficherCuisinier();
                        }
                        else if (keyInfo.Key == ConsoleKey.RightArrow)
                        {
                            entreeCorrecte = true;
                            SupprimerCuisinier();
                        }
                        else if (keyInfo.Key == ConsoleKey.Escape)
                        {
                            entreeCorrecte = true;
                            end = true;
                        }
                    }

                    Thread.Sleep(50);
                }
            }
            Console.WriteLine("Programme terminé !");

            /// <summary>
            /// Permet d'ajouter un cuisinier en faisant apparaitre l'interface appropriée
            /// </summary>
            static void AjouterCuisinier(Graphe<int> graphe)
            {
                Console.Clear();
                Console.WriteLine("Ajout cuisinier\n");


                string id_compte, prenom = "", nom = "", telephone, rue, numero, code_postal, ville, metro_le_plus_proche, email, mot_de_passe;
                Console.WriteLine("Champs obligatoires marqués par *");

                string reponse = "";
                bool compte = false;
                while (reponse.ToLower() != "oui" && reponse.ToLower() != "non")
                {
                    {
                        reponse = Demander("Le cuisinier a-t-il déjà un compte (client) ? [Oui/Non]", "string", true);
                        if (reponse.ToLower() == "oui")
                        {
                            compte = true;
                        }
                        else if (reponse.ToLower() == "non")
                        {
                            compte = false;
                        }
                    }
                }

                if (compte)
                {
                    id_compte = Demander("Identifiant du compte", "int", true);
                    string requeteInsertCuisinier = $"INSERT INTO Cuisinier (id_compte) VALUES ({Convert.ToInt32(id_compte)});";
                    DML_SQL(requeteInsertCuisinier);

                }
                else
                {
                    string viaJSON = Demander("Voulez-vous importer votre profil depuis le fichier nouveauCuisinier.json ? [oui/non]", "string", true);
                    if (viaJSON.ToLower() == "oui")
                    {
                        Console.WriteLine("Lecture du fichier et ajout du compte dans notre base de données...");
                        try
                        {
                            string jsonString = File.ReadAllText("../../../nouveauCuisinier.json");
                            using JsonDocument doc = JsonDocument.Parse(jsonString);
                            JsonElement root = doc.RootElement;

                            prenom = root.GetProperty("prenom").GetString().ToLower();
                            nom = root.GetProperty("nom").GetString().ToLower();
                            telephone = root.GetProperty("numero_telephone").GetString().ToLower();
                            numero = root.GetProperty("numero_residence").GetString().ToLower();
                            rue = root.GetProperty("rue").GetString().ToLower();
                            ville = root.GetProperty("ville").GetString().ToLower();
                            code_postal = root.GetProperty("code_postal").GetString().ToLower();
                            metro_le_plus_proche = root.GetProperty("metro_le_plus_proche").GetString();
                            email = root.GetProperty("email").GetString().ToLower();
                            mot_de_passe = root.GetProperty("mot_de_passe").GetString().ToLower();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Une erreur est survenue, nous allons créer le compte autrement");
                            Console.WriteLine("Champs obligatoires marqués par *");

                            prenom = Demander("Prénom", "string", true);

                            nom = Demander("Nom", "string", true);

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

                        prenom = Demander("Prénom", "string", true);


                        nom = Demander("Nom", "string", true);

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

                    if (InsertCompte)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Réussi ✅");
                        Console.ResetColor();
                    }

                    string requete = "SELECT LAST_INSERT_ID();";
                    List<string[]> resultat_id_compte = DQL_SQL(requete, false);

                    string requeteInsertCuisinier = $"INSERT INTO Cuisinier (id_compte) VALUES ({resultat_id_compte[0][0]});";
                    bool InsertCuisinier = DML_SQL(requeteInsertCuisinier);

                    if (InsertCuisinier)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Réussi ✅");
                        Console.ResetColor();
                    }

                    string requete2 = "SELECT LAST_INSERT_ID();";
                    int resultat_id_cuisinier = Convert.ToInt32(DQL_SQL(requete2, false)[0][0]);
                    Console.WriteLine("\nRequête executée, identifiant du nouveau cuisinier : " + resultat_id_cuisinier);
                }


                Console.BackgroundColor = ConsoleColor.Magenta;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\n Pressez la touche ENTREE pour sortir ");
                Console.ReadLine();
            }

            /// <summary>
            /// Permet de modifier un cuisinier en faisant apparaitre l'interface appropriée
            /// </summary>
            static void ModifierCuisinier()
            {
                Console.Clear();
                Console.WriteLine("Modification du compte lié au cuisinier\n");

                Console.WriteLine("Champs obligatoires marqués par *\n");

                string id_cuisinier = Demander("Entrez l'id du cuisinier à modifier", "int", true);

                string id_compte_requete = $"SELECT id_compte FROM Cuisinier WHERE id_cuisinier = {Convert.ToInt32(id_cuisinier)};";
                string id_compte = DQL_SQL(id_compte_requete, false)[0][0];

                string table_compte_requete = $"SELECT * FROM Compte WHERE id_compte = {Convert.ToInt32(id_compte)};";
                string[] table_compte = DQL_SQL(table_compte_requete, false)[0];

                string[] colonnesCompte = new string[] { "id_compte", "prenom", "nom", "telephone", "rue", "numero", "code_postal", "ville", "metro_le_plus_proche", "email", "mot_de_passe" };

                Console.WriteLine("id_compte | prenom | nom | telephone | rue | numero | code_postal | ville | metro_le_plus_proche | email | mot_de_passe");
                for (int i = 0; i < table_compte.Length; i++)
                {
                    Console.Write(table_compte[i] + " | ");
                }
                Console.WriteLine();

                int nbrColonnesUpdateCompte = Convert.ToInt32(Demander("Combien de colonnes souhaitez vous modifier ?", "int", true));
                string[] colonnesAModifCompte = new string[nbrColonnesUpdateCompte];


                string ColValCompte = "";
                for (int i = 0; i < nbrColonnesUpdateCompte; i++)
                {
                    bool existe = false;
                    while (!existe)
                    {
                        string colonne = Demander("Entrez le nom de la " + (i + 1) + "e colonne a modifier", "string", true);

                        for (int j = 0; j < colonnesCompte.Length && existe == false; j++)
                        {
                            if (colonne == colonnesCompte[j])
                            {
                                existe = true;
                                colonnesAModifCompte[i] = colonne;
                            }
                        }
                    }

                    string modif = Demander("Entrez la nouvelle valeur de la " + i + 1 + "e colonne", "string", true);

                    if (i == nbrColonnesUpdateCompte - 1)
                    {
                        ColValCompte += (colonnesAModifCompte[i] + " = " + $"'{modif}'");
                    }
                    else
                    {
                        ColValCompte += (colonnesAModifCompte[i] + " = " + $"'{modif}'" + ", ");
                    }
                }

                if (ColValCompte.Length > 0)
                {
                    string modif_compte_requete = $"UPDATE Compte SET {ColValCompte} WHERE id_compte = {Convert.ToInt32(id_compte)};";
                    bool modif_compte = DML_SQL(modif_compte_requete);
                    if (modif_compte)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Réussi ✅");
                        Console.ResetColor();
                    }
                }

                Console.BackgroundColor = ConsoleColor.Magenta;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\n Pressez la touche ENTREE pour sortir ");
                Console.ReadLine();
            }

            /// <summary>
            /// Permet de supprimer un cuisinier en faisant apparaitre l'interface appropriée
            /// </summary>
            static void SupprimerCuisinier()
            {
                Console.Clear();
                Console.WriteLine("Suppression cuisinier\n");

                string reponse = "";
                Console.WriteLine("Champs obligatoires marqués par *");

                string id_cuisinier = Demander("Entrez l'id du cuisinier à supprimer", "int", true);

                string id_compte_requete = $"SELECT id_compte FROM Cuisinier WHERE id_cuisinier = {Convert.ToInt32(id_cuisinier)};";
                string id_compte = DQL_SQL(id_compte_requete, false)[0][0];

                string requeteDeleteCuisinier = $"DELETE FROM Cuisinier WHERE id_cuisinier = {Convert.ToInt32(id_cuisinier)};";
                bool DeleteCuisinier = DML_SQL(requeteDeleteCuisinier);

                string requeteDeleteCompte = $"DELETE FROM Compte WHERE id_compte = {Convert.ToInt32(id_compte)};";
                bool DeleteCompte = DML_SQL(requeteDeleteCompte);

                if (DeleteCompte && DeleteCuisinier)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Réussi ✅");
                    Console.ResetColor();
                }

                Console.WriteLine("\n Pressez la touche ENTREE pour sortir ");
                Console.ReadLine();
            }

            /// <summary>
            /// Permet d'afficher un cuisinier en faisant apparaitre l'interface appropriée
            /// </summary>
            static void AfficherCuisinier()
            {
                Console.Clear();

                Console.BackgroundColor = ConsoleColor.DarkYellow;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Ordonné par ordre alpahabétique");
                Console.ResetColor();
                Console.WriteLine();

                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write("id_compte | prenom | nom | telephone | rue | numero | code_postal | ville | metro_le_plus_proche | email | mot_de_passe | id_cuisinier");
                Console.ResetColor();
                Console.WriteLine();
                string table_client_requete = $"SELECT * FROM Compte JOIN Cuisinier ON Compte.id_compte = Cuisinier.id_compte ORDER BY Compte.nom;";
                DQL_SQL(table_client_requete, true);

                Console.WriteLine("\n");

                Console.BackgroundColor = ConsoleColor.DarkYellow;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Ordonné par rue");
                Console.ResetColor();
                Console.WriteLine();

                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write("id_compte | prenom | nom | telephone | rue | numero | code_postal | ville | metro_le_plus_proche | email | mot_de_passe | id_client | entreprise | nom_entreprise | id_compte");
                Console.ResetColor();
                Console.WriteLine();
                table_client_requete = $"SELECT * FROM Compte JOIN Client ON Compte.id_compte = Client.id_compte ORDER BY Compte.rue;";
                DQL_SQL(table_client_requete, true);


                Console.WriteLine("\n Pressez la touche ENTREE pour sortir ");
                Console.ReadLine();

            }
        }
        #endregion


        #region Module COMMANDE
        /// <summary>
        /// Gestion et affichage du module de gestion des commandes
        /// </summary>
        static void ModuleCommande(Graphe<int> graphe)
        {
            bool end = false;
            while (!end)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();

                Console.WriteLine("Utilisez les touches flechées de votre clavier");
                Console.WriteLine("Appuyez sur ECHAP pour sortir");

                string libelle = "";
                int width = Console.WindowWidth;
                int height = Console.WindowHeight;

                libelle = " NOUVELLE COMMANDE ";
                Console.SetCursorPosition((width / 2) - (libelle.Length / 2), height / 4);
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(libelle);
                Console.ResetColor();

                libelle = " MODIFIER COMMANDE ";
                Console.SetCursorPosition((width / 4) - (libelle.Length / 2), height / 2);
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine(libelle);
                Console.ResetColor();

                libelle = " PARCOURS CUISINIER ";
                Console.SetCursorPosition((width / 2) - (libelle.Length / 2), 3 * (height / 4) + 2);
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(libelle);
                Console.ResetColor();

                libelle = " PRIX COMMANDE ";
                Console.SetCursorPosition(3 * (width / 4) - (libelle.Length / 2), height / 2);
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(libelle);
                Console.ResetColor();


                Console.SetCursorPosition(width / 2, height / 2 - 1);
                Console.Write("▲");

                Console.SetCursorPosition(width / 2 - 2, height / 2);
                Console.Write("◀ ■ ▶");

                Console.SetCursorPosition(width / 2, height / 2 + 1);
                Console.Write("▼");

                Console.SetCursorPosition(width / 2, height / 2 + 2);

                bool entreeCorrecte = false;
                while (!entreeCorrecte)
                {
                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                        if (keyInfo.Key == ConsoleKey.UpArrow)
                        {
                            entreeCorrecte = true;
                            NouvelleCommande(graphe);
                        }
                        else if (keyInfo.Key == ConsoleKey.LeftArrow)
                        {
                            entreeCorrecte = true;
                            ModifierCommande();
                        }
                        else if (keyInfo.Key == ConsoleKey.DownArrow)
                        {
                            entreeCorrecte = true;
                            ParcoursCuisinier();
                        }
                        else if (keyInfo.Key == ConsoleKey.RightArrow)
                        {
                            entreeCorrecte = true;
                            PrixCommande();
                        }
                        else if (keyInfo.Key == ConsoleKey.Escape)
                        {
                            entreeCorrecte = true;
                            end = true;
                        }
                    }

                    Thread.Sleep(50);
                }
            }

            /// <summary>
            /// Permet de créer de nouvelles commandes en faisant apparaître l'interface appropriée
            /// </summary>
            static void NouvelleCommande(Graphe<int> graphe)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();
                Console.WriteLine("Création d'une nouvelle commande");

                bool compteExists = Convert.ToBoolean(Demander("Avez vous un compte client ? [Oui/Non]", "bool", true));
                int id_client;
                string id_compte_requete, id_compte;
                if (compteExists)
                {
                    id_client = Convert.ToInt32(Demander("Entrez votre identifiant client", "int", true));
                    id_compte_requete = $"SELECT id_compte FROM Client WHERE id_client = {id_client};";
                    id_compte = DQL_SQL(id_compte_requete, false)[0][0];

                    string mot_de_passe = Demander("Entrez votre mot de passe", "mdp", true);
                    string reel_mot_de_passe = DQL_SQL($"SELECT mot_de_passe FROM Compte WHERE id_compte = '{id_compte}'", false)[0][0];

                    if (mot_de_passe != reel_mot_de_passe)
                    {
                        Console.WriteLine("Mot de passe incorrect, appuyez sur la touche ENTREE");
                        return;
                    }
                }
                else
                {
                    id_client = AjouterClient(graphe);
                    id_compte_requete = $"SELECT id_compte FROM Client WHERE id_client = {id_client};";
                    id_compte = DQL_SQL(id_compte_requete, false)[0][0];
                }

                string adresse_client_requete = $"SELECT metro_le_plus_proche FROM Compte WHERE id_compte = {id_compte};";
                string adresse_client = DQL_SQL(adresse_client_requete, false)[0][0];


                DML_SQL($"INSERT INTO commande (avis_client, note_client, cout_total, id_client) VALUES (NULL, NULL, 0, {id_client})");
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

                    string insertion_plat = $"INSERT INTO Ligne_de_commande (lieu_livraison, quantite, date_livraison, cout, id_commande, id_plat) VALUES ('{adresse_client}', '{Convert.ToString(quantite)}', '{date_livraison}', {cout_bon_format}, {id_commande}, {id_plat});";
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

                    string depart = metro_le_plus_proche_cusinier;
                    string arrivee = metro_le_plus_proche_client;
                    var (chemin, coutG) = graphe.TrouverMeilleurChemin(depart, arrivee);

                    if (chemin.Count == 0)
                        Console.WriteLine("Aucun chemin trouvé.");
                    else
                        Console.WriteLine($"Plus court chemin ({coutG} min) :\n {string.Join("\n -> ", chemin)}");
                }
                Console.WriteLine("\n Pressez la touche ENTREE pour sortir ");
                Console.ReadLine();
            }

            /// <summary>
            /// Permet de modifier des commandes en faisant apparaître l'interface appropriée
            /// </summary>
            static void ModifierCommande()
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();
                Console.WriteLine("Modification d'une commande");

                string id_commande = Demander("Entre l'identifiant de la commande à modfier", "int", true);

                DQL_SQL($"SELECT * FROM Commande WHERE id_commande = {id_commande}", true);

                Console.WriteLine();

                string avis_client = Demander("Entre la valeur de l'avis client", "string", true);
                string note_client = Demander("Entre la valeur de la note client", "int", true);
                string cout_total = Demander("Entre la valeur du cout total ", "string", true);
                string id_client = Demander("Entre la valeur de l'identifiant client ", "int", true);

                bool result = DML_SQL($"UPDATE Commande SET avis_client = '{avis_client}', note_client= {note_client}, cout_total={cout_total}, id_client={id_client} WHERE id_commande = {id_commande}");
                if (result)
                {
                    Console.WriteLine("Mise à jour réussie");
                }
                Console.WriteLine("\n Pressez la touche ENTREE pour sortir ");
                Console.ReadLine();
            }

            /// <summary>
            /// Permet de d'afficher le parcours que devra empreinter le cuisinier pour livre sa commande
            /// </summary>
            static void ParcoursCuisinier()
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();
                Console.WriteLine("Parcours cuisinier");

                string id_cuisinier = Demander("Entrez l'identifiant du cuisinier", "string", true);
                string id_commande = Demander("Entrez l'identifiant de la commande", "string", true);

                string[] id_ligne_de_commande = DQL_SQL($"SELECT LDC.id_ligne_de_commande FROM ligne_de_commande LDC WHERE id_commande = {id_commande};", false)[0];

                for (int i = 0; i < id_ligne_de_commande.Length; i++)
                {
                    Console.WriteLine($"Livraison {i + 1}e commande ");
                    string id_compte_cuisinier = DQL_SQL($"SELECT Cpt.id_compte FROM Compte Cpt JOIN cuisinier Cui ON cpt.id_compte = cui.id_compte WHERE cui.id_cuisinier = {id_cuisinier};", false)[0][0];
                    string metro_le_plus_proche_client = DQL_SQL($"SELECT lieu_livraison FROM ligne_de_commande WHERE id_ligne_de_commande = {id_ligne_de_commande[i]};", false)[0][0];
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
                        Console.WriteLine($"Plus court chemin ({coutG} min) :\n {string.Join("\n -> ", chemin)}");
                }
                Console.WriteLine("\n Pressez la touche ENTREE pour sortir ");
                Console.ReadLine();

            }

            /// <summary>
            /// Permet de d'afficher le prix d'une commande donnée
            /// </summary>
            static void PrixCommande()
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();
                Console.WriteLine("Prix commande");

                string id_commande = Demander("Entrez l'identifiant de la commande", "string", true);
                Console.Write("Prix de la commande : ");
                DQL_SQL($"SELECT cout_total FROM commande WHERE id_commande = {id_commande};", true);

                Console.WriteLine("\n Pressez la touche ENTREE pour sortir ");
                Console.ReadLine();
            }


            Console.WriteLine("\n Pressez la touche ENTREE pour sortir ");
            Console.ReadLine();
        }
        #endregion

        #region Module STATISTIQUES
        /// <summary>
        /// Affichage et gestion du module statistiques
        /// </summary>
        static void moduleStats()
        {
            Console.Clear();
            GrapheCommandes grapheCommandes = new GrapheCommandes();
            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Nombre de livraisons par cuisinier");
            Console.ResetColor();
            Console.WriteLine();

            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("id_cuisinier | prenom | nom | nbr livraison");
            Console.ResetColor();
            Console.WriteLine();
            string affichier_cuisinier_requete = $"SELECT C.id_cuisinier, Cpt.prenom, Cpt.nom, count(L.id_cuisinier) FROM Cuisinier C JOIN Compte Cpt ON C.id_compte = Cpt.id_compte JOIN Livre L ON C.id_cuisinier = L.id_cuisinier GROUP BY id_cuisinier;";
            DQL_SQL(affichier_cuisinier_requete, true);


            Console.WriteLine("\n");


            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Commandes selon une période");
            Console.ResetColor();
            Console.WriteLine();

            string min_date = Demander("Quelle est la date minimale pour cherche les commandes ? (Format : AAAA-MM-JJ)", "string", false);
            string max_date = Demander("Quelle est la date maximale pour cherche les commandes ? (Format : AAAA-MM-JJ)", "string", false);

            if (min_date.Length == 10 && max_date.Length == 10 && min_date[4] == '-' && min_date[7] == '-' && max_date[4] == '-' && max_date[7] == '-')
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write("id_commande | nom_plat | quantité | date_livraison");
                Console.ResetColor();
                Console.WriteLine();

                string affichier_plats_requete = $"SELECT LDC.id_commande, P.nom_plat, LDC.quantite, LDC.date_livraison FROM ligne_de_commande LDC JOIN Plat P ON LDC.id_plat = P.id_plat WHERE LDC.date_livraison >= '{min_date}' AND LDC.date_livraison <= '{max_date}' ORDER BY LDC.id_commande;";
                DQL_SQL(affichier_plats_requete, true);
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(" Format d'entrée incorrect, veuillez réessayer ");
                Console.ResetColor();
            }


            Console.WriteLine("\n");


            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Moyenne des prix des commandes");
            Console.ResetColor();
            Console.WriteLine();

            string moyenne_prix_cmd_requete = $"SELECT AVG(C.cout_total) FROM commande C;";
            DQL_SQL(moyenne_prix_cmd_requete, true);


            Console.WriteLine("\n");


            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Moyenne des prix des commandes par Client (en €)");
            Console.ResetColor();
            Console.WriteLine();

            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("id_client | prenom | nom | moyenne coût commande");
            Console.ResetColor();
            Console.WriteLine();

            string moyenne_prix_client_requete = $"SELECT Ct.id_client, Cpt.prenom, Cpt.nom, AVG(Cmd.cout_total) FROM commande Cmd JOIN client Ct ON Cmd.id_client = Ct.id_client JOIN compte Cpt ON Ct.id_compte = Cpt.id_compte GROUP BY Ct.id_client;";
            DQL_SQL(moyenne_prix_client_requete, true);


            Console.WriteLine("\n");


            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Liste commandes client selon nationalité et période");
            Console.ResetColor();
            Console.WriteLine();

            string id_client = Demander("Quel est l'identifiant du client ?", "int", false);
            string min_date2 = Demander("Quelle est la date minimale pour cherche les commandes ? (Format : AAAA-MM-JJ)", "string", false);
            string max_date2 = Demander("Quelle est la date maximale pour cherche les commandes ? (Format : AAAA-MM-JJ)", "string", false);
            string nationalite = Demander("Quelle est la nationalité des plats ?", "string", false);

            if (min_date2.Length == 10 && max_date2.Length == 10 && min_date2[4] == '-' && min_date2[7] == '-' && max_date2[4] == '-' && max_date2[7] == '-')
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write("id_commande | nom_plat | nationalité_de_cuisine | date_livraison | cout_total");
                Console.ResetColor();
                Console.WriteLine();

                string affichier_plats_requete = $"SELECT c.id_commande, p.nom_plat, p.nationnalite_de_cuisine, ldc.date_livraison, c.cout_total FROM Commande c JOIN Ligne_de_commande ldc ON c.id_commande = ldc.id_commande JOIN Plat p ON ldc.id_plat = p.id_plat JOIN Client cl ON c.id_client = cl.id_client WHERE cl.id_client = {Convert.ToInt32(id_client)} AND ldc.date_livraison BETWEEN '{min_date2}' AND '{max_date2}'  AND p.nationnalite_de_cuisine = '{nationalite}' ORDER BY ldc.date_livraison;\r\n";
                DQL_SQL(affichier_plats_requete, true);
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(" Format d'entrée des dates incorrect, veuillez réessayer ");
                Console.ResetColor();
            }



            Console.WriteLine("\n");
            Console.WriteLine("\n Pressez la touche ENTREE pour sortir ");
            Console.ReadLine();

        }
        #endregion


    }
}