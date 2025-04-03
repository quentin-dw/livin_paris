using System;
using System.IO;
using System.Collections.Generic;
using livin_paris;
using MySql.Data.MySqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Org.BouncyCastle.Asn1.Ocsp;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using Org.BouncyCastle.Tls;
using System.Net.Sockets;


namespace livin_paris
{
    internal class Program
    {
        static MySqlConnection connexion;
        static void Main()
        {

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            //string filePath = "../../../../soc-karate.mtx";
            //new Graphe(filePath);

            string connectionString = "Server=localhost;Database=psi;User ID=root;Password=root;SslMode=none;";
            connexion = ConnexionSQL(connectionString);

            AffichageMenu();

            Console.Read();


        }

        static void AffichageMenu()
        {
            bool entreeCorrecte = false;
            string messageErreur = "";
            while (!entreeCorrecte)
            {
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Bienvenue sur l'application Liv'in Paris ! \n\n\n");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(messageErreur);
                Console.ResetColor();

                Console.WriteLine("Interface Administrateur");
                Console.WriteLine("Selectionnez une option ci-dessous : ");
                Console.WriteLine("\t 1) Module client");
                Console.WriteLine("\t 2) Module cuisinier");
                Console.WriteLine("\t 3) Module commande");
                Console.WriteLine("\t 4) Module statistiques");
                Console.WriteLine("\t 5) Module autre");
                Console.Write("=> ");
                Console.ForegroundColor = ConsoleColor.Yellow;

                string reponse = Console.ReadLine();
                Console.ResetColor();
                switch (reponse)
                {
                    case "1":
                        entreeCorrecte = true;
                        ModuleClient();
                        break;
                    case "2":
                        entreeCorrecte = true;
                        ModuleCuisinier();
                        break;
                    case "3":
                        entreeCorrecte = true;
                        //moduleClient();
                        break;
                    case "4":
                        entreeCorrecte = true;
                        moduleStats();
                        break;
                    case "5":
                        entreeCorrecte = true;
                        //moduleClient();
                        break;
                    default:
                        messageErreur = "ERREUR : Entrée incorrecte !";
                        break;

                }
            }
        }

        #region Module CLIENT
        static void ModuleClient()
        {
            bool end = false;
            while (!end)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();

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
                            AjouterClient();
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
            Console.WriteLine("Programme terminé !");


            static void AjouterClient()
            {
                Console.Clear();
                Console.WriteLine("Ajout client\n");

                string reponse = "";
                string entreprise = "", prenom = "", nom = "", nom_entreprise="", telephone, rue, numero, code_postal, ville, metro_le_plus_proche, email, mot_de_passe;
                Console.WriteLine("Champs obligatoires marqués par *");


                while (reponse.ToLower() != "oui" && reponse.ToLower() != "non")
                {
                    {
                        reponse = Demander("Ce client est il une entreprise ? [Oui/Non]", "string", true);
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

                    }
                }

                
                telephone = Demander("Numéro de téléphone", "string", true);

                rue = Demander("Nom de rue de résidence", "string", true);

                numero = Demander("Numéro de rue de résidence", "int", true);

                code_postal = Demander("Code postal de résidence", "int", true);

                ville = Demander("Ville de résidence", "string", true);

                metro_le_plus_proche = Demander("Station de metro la plus proche", "string", true);

                email = Demander("Adresse e-mail", "string", true);

                mot_de_passe = Demander("Mot de passe", "string", true);


                string requeteInsertCompte = $"INSERT INTO Compte (prenom, nom, telephone, rue, numero, code_postal, ville, metro_le_plus_proche, email, mot_de_passe) VALUES ('{prenom}', '{nom}', '{telephone}', '{rue}', {Convert.ToInt32(numero)}, {Convert.ToInt32(code_postal)}, '{ville}', '{metro_le_plus_proche}', '{email}', '{mot_de_passe}');";
                DML_SQL(requeteInsertCompte);

                string requete = "SELECT LAST_INSERT_ID();";
                List<string[]> resultat_id_compte = DQL_SQL(requete, false);

                string requeteInsertClient = $"INSERT INTO Client (entreprise, nom_entreprise, id_compte) VALUES ({entreprise}, '{nom_entreprise}', {Convert.ToInt32(resultat_id_compte[0][0])});";
                DML_SQL(requeteInsertClient);

                string requete2 = "SELECT LAST_INSERT_ID();";
                string[] resultat_id_client = DQL_SQL(requete2, false)[0];
                Console.WriteLine("\nRequête executée, identifiant du nouveau client : " + resultat_id_client);

                Console.BackgroundColor = ConsoleColor.Magenta;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\n Pressez la touche ENTREE pour sortir ");
                Console.ReadLine();
            }

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
                    Console.Write(table_client[i]+" | ");
                }

                string id_compte_requete = $"SELECT id_compte FROM Client WHERE id_client = {Convert.ToInt32(id_client)};";
                string id_compte = DQL_SQL(id_compte_requete, false)[0][0];

                string table_compte_requete = $"SELECT * FROM Compte WHERE id_compte = {Convert.ToInt32(id_compte)};";
                string[] table_compte = DQL_SQL(table_compte_requete, false)[0];

                string[] colonnesCompte = new string[] { "id_compte", "prenom","nom","telephone","rue","numero","code_postal","ville","metro_le_plus_proche","email","mot_de_passe" };

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
                    while (!existe) { 
                        string colonne = Demander("Entrez le nom de la " + i + 1 + "e colonne a modifier dans Client", "string", true).ToLower();

                        for (int j = 0; j < colonnesClient.Length; j++) {
                            if (colonne == colonnesClient[j])
                            {
                                existe = true;
                                colonnesAModifClient[i] = colonne;
                            }
                        }                        
                    }
                    string modif = Demander("Entrez la nouvelle valeur de la " + i + 1 + "e colonne", "string", true);

                    if(i == nbrColonnesUpdateClient -1)
                    {
                        ColValClient += (colonnesAModifClient[i] + " = " + $"'{modif}'");
                    } else
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

                    if (i == nbrColonnesUpdateCompte -1)
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
                    DML_SQL(modif_client_requete);
                }

                if (ColValCompte.Length > 0)
                {
                    string modif_compte_requete = $"UPDATE Compte SET {ColValCompte} WHERE id_compte = {Convert.ToInt32(id_compte)};";
                    DML_SQL(modif_compte_requete);
                }

                Console.BackgroundColor = ConsoleColor.Magenta;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\n Pressez la touche ENTREE pour sortir ");
                Console.ReadLine();
            }

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
                DML_SQL(requeteDeleteClient);

                string requeteDeleteCompte = $"DELETE FROM Compte WHERE id_compte = {Convert.ToInt32(id_compte)};";
                DML_SQL(requeteDeleteCompte);

                Console.WriteLine("\n Pressez la touche ENTREE pour sortir ");
                Console.ReadLine();
            }

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
        }
        #endregion

        #region Module Cuisinier
        static void ModuleCuisinier()
        {
            bool end = false;
            while (!end)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();

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
                            AjouterCuisinier();
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


            static void AjouterCuisinier()
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

                } else
                {
                    prenom = Demander("Prénom", "string", true);

                    nom = Demander("Nom", "string", true);

                    telephone = Demander("Numéro de téléphone", "string", true);

                    rue = Demander("Nom de rue de résidence", "string", true);

                    numero = Demander("Numéro de rue de résidence", "int", true);

                    code_postal = Demander("Code postal de résidence", "int", true);

                    ville = Demander("Ville de résidence", "string", true);

                    metro_le_plus_proche = Demander("Station de metro la plus proche", "string", true);

                    email = Demander("Adresse e-mail", "string", true);

                    mot_de_passe = Demander("Mot de passe", "string", true);


                    string requeteInsertCompte = $"INSERT INTO Compte (prenom, nom, telephone, rue, numero, code_postal, ville, metro_le_plus_proche, email, mot_de_passe) VALUES ('{prenom}', '{nom}', '{telephone}', '{rue}', {Convert.ToInt32(numero)}, {Convert.ToInt32(code_postal)}, '{ville}', '{metro_le_plus_proche}', '{email}', '{mot_de_passe}');";
                    DML_SQL(requeteInsertCompte);

                    string requete = "SELECT LAST_INSERT_ID();";
                    List<string[]> resultat_id_compte = DQL_SQL(requete, false);

                    string requeteInsertCuisinier= $"INSERT INTO Cuisinier (id_compte) VALUES ({resultat_id_compte[0][0]});";
                    DML_SQL(requeteInsertCuisinier);

                    string requete2 = "SELECT LAST_INSERT_ID();";
                    string[] resultat_id_cuisinier = DQL_SQL(requete2, false)[0];
                    Console.WriteLine("\nRequête executée, identifiant du nouveau cuisinier : " + resultat_id_cuisinier);
                }
                    

                Console.BackgroundColor = ConsoleColor.Magenta;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\n Pressez la touche ENTREE pour sortir ");
                Console.ReadLine();
            }

            static void ModifierCuisinier()
            {
                Console.Clear();
                Console.WriteLine("Modification du compte lié au cuisinier\n");

                Console.WriteLine("Champs obligatoires marqués par *\n");

                string id_cuisinier= Demander("Entrez l'id du cuisinier à modifier", "int", true);

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
                    DML_SQL(modif_compte_requete);
                }

                Console.BackgroundColor = ConsoleColor.Magenta;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\n Pressez la touche ENTREE pour sortir ");
                Console.ReadLine();
            }

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
                DML_SQL(requeteDeleteCuisinier);

                string requeteDeleteCompte = $"DELETE FROM Compte WHERE id_compte = {Convert.ToInt32(id_compte)};";
                DML_SQL(requeteDeleteCompte);

                Console.WriteLine("\n Pressez la touche ENTREE pour sortir ");
                Console.ReadLine();
            }

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


        static void moduleStats()
        {
            Console.Clear();

            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Nombre de livraisons par cuisinier");
            Console.ResetColor();
            Console.WriteLine();

            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("id_client | prenom | nom | nbr livraison");
            Console.ResetColor();
            Console.WriteLine();
            string affichier_cuisinier_requete = $"SELECT C.id_cuisinier, Cpt.prenom, Cpt.nom, count(L.id_cuisinier) FROM Cuisinier C JOIN Compte Cpt ON C.id_compte = Cpt.id_compte JOIN Livre L ON C.id_cuisinier = L.id_cuisinier GROUP BY id_cuisinier;";
            DQL_SQL(affichier_cuisinier_requete, true);


            Console.WriteLine("\n");


            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Nombre de livraisons par cuisinier");
            Console.ResetColor();
            Console.WriteLine();

            string min_date = Demander("Quelle est la date minimale pour cherche les commandes ? (Format : AAAA-MM-JJ)", "string", false);
            string max_date = Demander("Quelle est la date maximale pour cherche les commandes ? (Format : AAAA-MM-JJ)", "string", false);

            if(min_date.Length == 10 && max_date.Length == 10 && min_date[4] == '-' && min_date[7] == '-' && max_date[4] == '-' && max_date[7] == '-')
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write("id_commande | nom_plat | quantité | date_livraison");
                Console.ResetColor();
                Console.WriteLine();

                string affichier_plats_requete = $"SELECT LDC.id_commande, P.nom_plat, LDC.quantite, LDC.date_livraison FROM ligne_de_commande LDC JOIN Plat P ON LDC.id_plat = P.id_plat WHERE LDC.date_livraison >= '{min_date}' AND LDC.date_livraison <= '{max_date}' ORDER BY LDC.id_commande;";
                DQL_SQL(affichier_plats_requete, true);
            } else
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
            Console.WriteLine("\n Pressez la touche ENTREE pour sortir ");
            Console.ReadLine();

        }

        #region Outils
        static string Demander(string question, string type, bool required)
        {
            string reponse = "";
            bool correcte = false;
            while (correcte == false)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write(" " + question + " ");


                if (required)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("* ");
                }
                Console.ResetColor();
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("➜  ");
                Console.ForegroundColor = ConsoleColor.Yellow;

                reponse = Console.ReadLine();

                if (required && reponse != null)
                {
                    if (type == "string" && reponse.Trim() != "")
                    {
                        correcte = true;
                        reponse = reponse.ToLower().Trim();
                    }
                    else if (type == "int" && int.TryParse(reponse, out int n)) 
                    {
                        correcte = true;
                        reponse = reponse.Trim();
                    }
                }
                else if (!required)
                {
                    if (reponse == null)
                    {
                        correcte = true;
                    } else if (type == "int" && int.TryParse(reponse, out int n))
                    {
                        correcte = true;
                        reponse = reponse.Trim();
                    }
                }

                if (!correcte) { 
                    Console.Write("\t");
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(" Erreur : Format incorrect ! ");
                    Console.ResetColor();
                }

                Console.WriteLine();
            }

            Console.ResetColor();
            return reponse;
        }

        static void DML_SQL(string req)
        {
            MySqlCommand command = connexion.CreateCommand();
            command.CommandText = req;
            try
            {
                command.ExecuteNonQuery();
            }
            catch (MySqlException e)
            {
                Console.WriteLine(" Erreur : " + e.ToString());
                Console.ReadLine();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Modification réussie ✅");
            Console.ResetColor();
            command.Dispose();
        }

        static List<string[]> DQL_SQL(string req, bool affichage)
        {
            MySqlCommand command = connexion.CreateCommand();
            command.CommandText = req;

            List<string[]> resultat = new List<string[]>();
            MySqlDataReader reader = command.ExecuteReader();

            string[] valueString = new string[reader.FieldCount];
            while (reader.Read())
            {
                Console.Write("◌ ");
                for (int i = 0; i <reader.FieldCount; i++)
                {
                    valueString[i] = reader.GetValue(i).ToString();
                    if (affichage && i < reader.FieldCount-1)
                    {
                        Console.Write(valueString[i] + ", ");
                    } else if (affichage && i == reader.FieldCount - 1)
                    {
                        Console.Write(valueString[i]);
                    }
                }
                resultat.Add(valueString);
                if (affichage)
                {
                    Console.WriteLine();
                }
            }
            reader.Close();
            command.Dispose();

            return resultat;
        }

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
        }
        #endregion
    }
}
