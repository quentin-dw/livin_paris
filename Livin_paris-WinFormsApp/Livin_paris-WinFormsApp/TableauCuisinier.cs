using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Livin_paris_WinFormsApp.Outils;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;


namespace Livin_paris_WinFormsApp
{
    public class TableauCuisinier
    {
        private static Cuisinier cuisinierCourant;
        public static void AffichageMenuCuisinier()
        {
            Cuisinier cuisinier = null;
            int choix = MenuCirculaire(4, "connexion", "associer un compte", "creer un compte", "trouver mon identifiant", "Menu Cuisinier");
            if (choix == -2)
            {
                return;
            }
            else if (choix == 0)
            {
                cuisinier = ConnexionCuisinier();
            }
            else if (choix == 1)
            {
                AssocierCompteCuisinier();
                return;
            }
            else if (choix == 2)
            {
                cuisinier = CreationCompteCuisinier();
                if (cuisinier == null)
                {
                    return;
                }
            }
            else if (choix == 3)
            {
                TrouverIdentifiantCuisinier();
                return;
            }

            if(cuisinier == null)
            {
                return;
            }

            cuisinierCourant = cuisinier;

            bool end = false;
            while (!end)
            {
                int choix2 = MenuCirculaire(4, "proposer nouveau plat", "modifier plats", "livraisons à effectuer", "historique livraisons", "Menu Cuisinier");
                if (choix2 == -2)
                {
                    end = true;
                }
                else if (choix2 == 0)
                {
                    NouveauPlat();
                }
                else if (choix2 == 1)
                {
                    ModifierPlats();
                }
                else if (choix2 == 2)
                {
                    Livraisons();
                }
                else if (choix2 == 3)
                {
                    HistoriqueLivraisons();
                }
            }
        }

        #region Connexion, Association, Creation de compte, Trouver identifiant
        static Cuisinier ConnexionCuisinier()
        {
            Cuisinier cuisinier = null;

            Console.ResetColor();
            Console.Clear();

            Console.SetCursorPosition(1, 0);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(" ▪ " + "Connexion ");
            Console.ResetColor();
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

                if(id_compte == -1)
                {
                    return cuisinier;
                }

                string idExists = DQL_SQL($"SELECT EXISTS (SELECT * FROM cuisinier WHERE id_compte = {id_compte})", false)[0][0];


                if (idExists == "1")
                {
                    valide = true;
                }
                else
                {
                    if (DQL_SQL($"SELECT EXISTS (SELECT * FROM compte WHERE id_compte = {id_compte})", false)[0][0] == "1")
                    {
                        messageErreur = " Identifiant non associé à un compte cuisinier (veuillez d'abord l'associer) ";
                    }
                    else
                    {
                        messageErreur = " Identifiant invalide ";
                    }
                }
            }

            valide = false;

            cuisinier = new Cuisinier(id_compte);
            Console.WriteLine("Bienvenue " + cuisinier.Prenom + " " + cuisinier.Nom);

            while (!valide)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(messageErreur);

                Console.ResetColor();

                string mot_de_passe = Demander("Entrez votre mot de passe", "mdp", true);
                if (mot_de_passe == cuisinier.Mot_de_passe)
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

            return cuisinier;
        }

        static void AssocierCompteCuisinier()
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

            string cuisinierExists = DQL_SQL($"SELECT EXISTS(SELECT * FROM Cuisinier WHERE id_compte = {id_compte})", false)[0][0];
            if (cuisinierExists == "1")
            {
                Console.WriteLine("Ce compte est déjà associé à un profil cuisinier.");
                Thread.Sleep(1500);
                return;
            }
            
            bool ok = DML_SQL($"INSERT INTO Cuisinier (id_compte) VALUES ({id_compte});");
            if (!ok)
            {
                Console.WriteLine("Erreur lors de l'association. Réessayez.");
                Thread.Sleep(1500);
                return;
            }
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Compte associé comme cuisinier ✅");
            Console.ResetColor();
            Thread.Sleep(1500);
        }

        static Cuisinier CreationCompteCuisinier()
        {
            Cuisinier cuisinier = null;

            Console.ResetColor();
            Console.Clear();

            Console.SetCursorPosition(1, 0);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(" ▪ " + "Creation compte ");
            Console.ResetColor();
            Console.WriteLine();


            string reponse = "";
            string  prenom = "", nom = "", telephone, rue, numero, code_postal, ville, metro_le_plus_proche, email, mot_de_passe;
            Console.WriteLine("Champs obligatoires marqués par *");

            prenom = Demander("Prénom", "string", true);

            nom = Demander("Nom", "string", true);

            telephone = Demander("Numéro de téléphone", "string", true);

            numero = Demander("Numéro de rue de résidence", "int", true);

            rue = Demander("Nom de rue de résidence", "string", true);

            ville = Demander("Ville de résidence", "string", true);

            code_postal = Demander("Code postal de ville de résidence", "int", true);

            metro_le_plus_proche = Demander("Station de metro la plus proche", "string", true);

            email = Demander("Adresse e-mail", "string", true);

            mot_de_passe = Demander("Mot de passe", "mdp", true);


            string requeteInsertCompte = $"INSERT INTO Compte (prenom, nom, telephone, rue, numero, code_postal, ville, metro_le_plus_proche, email, mot_de_passe) VALUES ('{prenom}', '{nom}', '{telephone}', '{rue}', {Convert.ToInt32(numero)}, {Convert.ToInt32(code_postal)}, '{ville}', '{metro_le_plus_proche}', '{email}', '{mot_de_passe}');";
            bool InsertCompte = DML_SQL(requeteInsertCompte);

            string requete = "SELECT LAST_INSERT_ID();";
            int id_compte = Convert.ToInt32(DQL_SQL(requete, false)[0][0]);

            string requeteInsertCuisinier = $"INSERT INTO Cuisinier (id_compte) VALUES ({id_compte});";
            InsertCompte = DML_SQL(requeteInsertCuisinier);

            if (InsertCompte)
            {
                cuisinier = new Cuisinier(id_compte);

                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine();
                Console.WriteLine(" Compte Cuisinier Créé ✅ ");

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

            return cuisinier;
        }

        static void TrouverIdentifiantCuisinier()
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

                if (Convert.ToInt32(DQL_SQL($"SELECT EXISTS (SELECT * FROM compte WHERE email='{email}')", false)[0][0]) == 0)
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

                if (Convert.ToInt32(DQL_SQL($"SELECT EXISTS (SELECT * FROM compte WHERE telephone='{telephone}')", false)[0][0]) == 0)
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

        static void NouveauPlat()
        {
            Console.ResetColor();
            Console.Clear();

            Console.SetCursorPosition(1, 0);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(" ▪ " + "Proposer un nouveau plat ");
            Console.ResetColor();
            Console.WriteLine();

            string nomPlat = Demander("Nom du plat", "string", true);
            string typePlat = Demander("Type du plat (entrée/plat principal/dessert)", "string", true);
            string dateFab = Demander("Date de fabrication (AAAA-MM-JJ)", "string", true);
            string datePer = Demander("Date de péremption (AAAA-MM-JJ)", "string", true);
            string prixStr = Demander("Prix par personne (ex: 12.50)", "string", true);
            string nationalite = Demander("Nationalité de la cuisine", "string", true);
            string regime = Demander("Régime alimentaire", "string", true);

            string useRecette = Demander("Utiliser une recette existante ? (oui/non)", "bool", true);
            int idRecette = 0;
            if (useRecette == "True")
            {
                DQL_SQL("SELECT id_recette, nom_recette FROM Recette;", true);
                string idRecetteStr = Demander("Entrez l'id de la recette existante", "int", true);
                idRecette = Convert.ToInt32(idRecetteStr);
            }
            else
            {
                string nomRecette = Demander("Nom de la nouvelle recette", "string", true);
                string declinStr = Demander("S'agit-il d'une déclinaison d'une recette existante ? (oui/non)", "bool", true);
                bool isDecl = declinStr == "True";
                string idRecDeclStr = "NULL";
                if (isDecl)
                {
                    idRecDeclStr = Demander("Entrez l'id de la recette parente", "int", true);
                }
                string insertRec = $"INSERT INTO Recette (nom_recette, declinaison, id_recette_declinee) VALUES ('{nomRecette}', {(isDecl ? 1 : 0)}, {(isDecl ? idRecDeclStr : "NULL")});";
                DML_SQL(insertRec);
                idRecette = Convert.ToInt32(DQL_SQL("SELECT LAST_INSERT_ID();", false)[0][0]);

                
                DQL_SQL("SELECT id_ingredient, nom_ingredient FROM Ingrédient;", true);
                string listIng = Demander("Entrez les ids d'ingrédients séparés par des virgules (optionnel)", "string", false);
                if (!string.IsNullOrWhiteSpace(listIng))
                {
                    foreach (var s in listIng.Split(','))
                    {
                        if (int.TryParse(s.Trim(), out int idIng))
                        {
                            string qteIng = Demander($"Quantité pour l'ingrédient {idIng}", "string", true);
                            string insComp = $"INSERT INTO compose (id_ingredient, id_recette, quantite) VALUES ({idIng}, {idRecette}, '{qteIng}');";
                            DML_SQL(insComp);
                        }
                    }
                }
            }

            string insertPlat = $"INSERT INTO Plat (nom_plat, type, date_fabrication, date_peremption, prix, nationnalite_de_cuisine, regime_alimentaire, id_cuisinier, id_recette) VALUES ('{nomPlat}', '{typePlat}', '{dateFab}', '{datePer}', {prixStr}, '{nationalite}', '{regime}', {cuisinierCourant.Id_cuisinier}, {idRecette});";
            DML_SQL(insertPlat);

            Console.WriteLine("Nouveau plat ajouté ✅");
            Thread.Sleep(1500);
        }

        static void ModifierPlats()
        {
            Console.ResetColor();
            Console.Clear();

            Console.SetCursorPosition(1, 0);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(" ▪ " + "Modifier un plat ");
            Console.ResetColor();
            Console.WriteLine();

            DQL_SQL($"SELECT id_plat, nom_plat FROM Plat WHERE id_cuisinier = {cuisinierCourant.Id_cuisinier};", true);
            Console.WriteLine();
            int id_plat = Convert.ToInt32(Demander("Entrez l'id du plat à modifier", "int", true));

            string[] champs = { "nom_plat", "type", "date_fabrication", "date_peremption", "prix", "nationnalite_de_cuisine", "regime_alimentaire", "photo" };
            string[] libelles = { "Nom du plat", "Type du plat", "Date de fabrication (AAAA-MM-JJ)", "Date de péremption (AAAA-MM-JJ)", "Prix par personne", "Nationalité de la cuisine", "Régime alimentaire", "Chemin de la photo", "× SUPPRIMER PLAT ×" };

            bool finModif = false;
            while (!finModif)
            {

                Console.WriteLine("Quel champ souhaitez-vous modifier ?");
                for (int i = 0; i < libelles.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {libelles[i]}");
                }
                Console.WriteLine();
                string choixChampStr = Demander("Entrez le numéro du champ", "int", true);
                int choixChamp = Convert.ToInt32(choixChampStr) - 1;
                if (choixChamp < 0 || choixChamp >= champs.Length+1)
                {
                    Console.WriteLine("Choix invalide");
                    Thread.Sleep(1000);
                    return;
                }
                Console.WriteLine();

                if (choixChamp == libelles.Length - 1)
                {
                    string delete = $"DELETE FROM Plat WHERE id_plat = {id_plat};";
                    DML_SQL(delete);

                    Console.WriteLine("Plat supprimé ✅");
                }
                else
                {

                    string nouvelleValeur = Demander($"Nouvelle valeur pour {libelles[choixChamp]}", "string", true);
                    string valeurSQL = champs[choixChamp] == "prix" ? nouvelleValeur : $"'{nouvelleValeur}'";
                    string update = $"UPDATE Plat SET {champs[choixChamp]} = {valeurSQL} WHERE id_plat = {id_plat};";
                    DML_SQL(update);

                    Console.WriteLine("Plat mis à jour ✅");
                }
                Console.WriteLine();
                string finModifReq = Demander("Voulez-vous modifier un autre champ ? [Oui/Non]", "bool", true);
                if (finModifReq == "False")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Modifications effectuées avec succès ✅");
                    finModif = true;
                }
                
            }
        }

        static void Livraisons()
        {
            Console.ResetColor();
            Console.Clear();

            Console.SetCursorPosition(1, 0);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(" ▪ " + "Livraisons à effectuer ");
            Console.ResetColor();
            Console.WriteLine();

            string requete = $"SELECT l.id_ligne_de_commande, l.date_livraison, l.lieu_livraison, l.quantite, l.cout FROM Ligne_de_commande l JOIN Plat p ON l.id_plat = p.id_plat WHERE p.id_cuisinier = {cuisinierCourant.Id_cuisinier};";
            DQL_SQL(requete, true);
            Console.WriteLine("\n\nAppuyez sur ENTREE pour sortir");
            Console.ReadLine();
        }

        static void HistoriqueLivraisons()
        {
            Console.ResetColor();
            Console.Clear();

            Console.SetCursorPosition(1, 0);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(" ▪ " + "Historique des livraisons ");
            Console.ResetColor();
            Console.WriteLine();

            string requete = $"SELECT l.id_ligne_de_commande, l.date_livraison, l.lieu_livraison, l.quantite, l.cout FROM Ligne_de_commande l JOIN Plat p ON l.id_plat = p.id_plat WHERE p.id_cuisinier = {cuisinierCourant.Id_cuisinier};";
            DQL_SQL(requete, true);
            Console.WriteLine("\nAppuyez sur ENTREE pour sortir");
            Console.ReadLine();
        }
    }
}
