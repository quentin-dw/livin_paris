# 🍽️ Liv'in Paris - Application Console C#

## 📌 Description

**Liv'in Paris** est une application console en C# développée dans le cadre du Projet Scientifique Informatique 2025. Elle simule une plateforme de mise en relation entre **cuisiniers** et **clients** pour le partage de repas dans Paris intramuros.  
Ce projet intègre la gestion d'utilisateurs, de commandes, de parcours de livraison via le métro parisien, et propose des statistiques avancées et traitements sur graphes.

---

## 🧩 Fonctionnalités

### 👤 Gestion des Utilisateurs
- Inscription, modification, suppression de **clients** (particuliers ou entreprises)
- Inscription, modification, suppression de **cuisiniers**
- Double rôle possible : un utilisateur peut être à la fois client et cuisinier

### 🍲 Gestion des Plats et Commandes
- Création de plats avec :
  - Type (entrée, plat, dessert)
  - Ingrédients, prix, nationalité, régimes alimentaires, photo
- Commandes composées de plusieurs plats, avec dates et lieux de livraison distincts
- Calcul automatique du **prix total**
- Historique des transactions stocké et accessible

### 🚇 Parcours et Livraisons
- Calcul du **chemin le plus court** entre client et cuisinier via les lignes de métro parisien
- Algorithmes implémentés :
  - Dijkstra
  - Bellman-Ford
  - Floyd-Warshall
- Visualisation textuelle et graphique des trajets

### 📊 Statistiques
- Nombre de livraisons par cuisinier
- Classement des meilleurs clients par montant d’achat
- Statistiques sur les commandes (période, nationalité des plats, moyenne de prix)

### 🧠 Fonctions avancées sur les graphes
- Détection de cycles, connexité, coloration de graphe (Welsh-Powell)
- Détermination de groupes indépendants
- Export de données en **JSON** et **XML**

---

## 💻 Technologies utilisées

- **Langage** : C# (.NET)
- **Base de données** : MySQL
- **Graphes** : Algorithmes personnalisés, visualisation graphique via WinForms
- **Tests** : Projet de tests unitaires avec MSTest

---

## 🚀 Lancement de l'application

1. Cloner le dépôt :
   ```bash
   https://github.com/quentin-dw/livin_paris.git
   ```
2. Exécuter `creation_db_demougeot_dehecohen_dewolf.sql` dans MySQL pour créer la base de donnée
3. Lancer la solution `Livin_paris-WinFormsApp.sln` dans Visual Studio

---

## 👥 Auteurs

- Nils Demougeot
- Tom Dehé-Cohen
- Quentin De Wolf

