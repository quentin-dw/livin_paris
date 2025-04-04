using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace TestProject1
{
    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        public void TestMethod_DML_SQL()
        {
            string input = "INSERT INTO Ingrédient (nom_ingredient, type) VALUES ('chocolat', 'dessert');";
            bool result = Program.DML_SQL(input);
        }
    }
}
