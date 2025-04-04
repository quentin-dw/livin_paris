using Livin_paris_WinFormsApp;

namespace TestProject1
{
    [TestClass]
    public sealed class Test1
    {
        //[TestMethod]
        //public void TestMethod_DML_SQL_True()
        //{
        //    string input = "INSERT INTO Ingrédient (nom_ingredient, type) VALUES ('chocolat', 'dessert');";
        //    bool result = Program.DML_SQL(input);
        //    Assert.IsTrue(result);
        //}

        //[TestMethod]
        //public void TestMethod_DML_SQL_False()
        //{
        //    string input = "INSERT INTO Ingrédient (a, b) VALUES ('chocolat', 'dessert');";
        //    bool result = Program.DML_SQL(input);
        //    Assert.IsFalse(result);
        //}

        [TestMethod]
        public void TestMethod_isEqual_expecting_True()
        {
            string noeuds = "../../../../../noeuds.csv";
            string arcs = "../../../../../arcs.csv";
            Graphe<int> graphe = new Graphe<int>(noeuds, arcs);
            Noeud<int> noeud1 = graphe.GetNoeuds()[0];
            Noeud<int> noeud2 = graphe.GetNoeuds()[0];
            bool result = Noeud<int>.isEqual(noeud1, noeud2);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestMethod_isEqual_expecting_False()
        {
            string noeuds = "../../../../../noeuds.csv";
            string arcs = "../../../../../arcs.csv";
            Graphe<int> graphe = new Graphe<int>(noeuds, arcs);
            Noeud<int> noeud1 = graphe.GetNoeuds()[0];
            Noeud<int> noeud2 = graphe.GetNoeuds()[1];
            bool result = Noeud<int>.isEqual(noeud1, noeud2);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestMethod_memeStation_expecting_True()
        {
            string noeuds = "../../../../../noeuds.csv";
            string arcs = "../../../../../arcs.csv";
            Graphe<int> graphe = new Graphe<int>(noeuds, arcs);
            Noeud<int> noeud1 = graphe.GetNoeuds()[109];
            Noeud<int> noeud2 = graphe.GetNoeuds()[240];
            bool result = Noeud<int>.memeStation(noeud1, noeud2);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestMethod_memeStation_expecting_False()
        {
            string noeuds = "../../../../../noeuds.csv";
            string arcs = "../../../../../arcs.csv";
            Graphe<int> graphe = new Graphe<int>(noeuds, arcs);
            Noeud<int> noeud1 = graphe.GetNoeuds()[0];
            Noeud<int> noeud2 = graphe.GetNoeuds()[1];
            bool result = Noeud<int>.memeStation(noeud1, noeud2);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestMethod_Noeud_ToString()
        {
            Noeud<int> noeud = new Noeud<int>(0, "5", "gare du nord", 2.05, 48.6, "Paris", "75114");
            string expected = "ID: 0, gare du nord ligne 5";
            string actual = noeud.ToString();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestMethod_Lien_toString()
        {
            Noeud<int> noeud1 = new Noeud<int>(0, "5", "gare du nord", 2.05, 48.6, "Paris", "75114");
            Noeud<int> noeud2 = new Noeud<int>(0, "5", "gare de l'est", 2.05, 48.6, "Paris", "75114");
            Lien<int> lien = new Lien<int>(noeud1, noeud2, 5);
            string expected = "gare du nord -> gare de l'est : 5 min";
            string actual = lien.toString();
            Assert.AreEqual(expected, actual);
        }
    }
}
