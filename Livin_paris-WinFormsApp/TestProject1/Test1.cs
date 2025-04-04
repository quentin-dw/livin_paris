using Livin_paris_WinFormsApp;

namespace TestProject1
{
    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        public void TestMethod_DML_SQL_True()
        {
            string input = "INSERT INTO Ingrédient (nom_ingredient, type) VALUES ('chocolat', 'dessert');";
            bool result = Program.DML_SQL(input);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestMethod_DML_SQL_False()
        {
            string input = "INSERT INTO Ingrédient (a, b) VALUES ('chocolat', 'dessert');";
            bool result = Program.DML_SQL(input);
            Assert.IsFalse(result);
        }
    }
}
