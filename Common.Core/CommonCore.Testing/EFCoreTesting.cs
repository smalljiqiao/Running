using EFDataCoreDomain.Contexts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonCore.Testing
{
    [TestClass]
    public class EFCoreTesting
    {
        [TestMethod]
        public void TestMethod1()
        {

            using (var context = new QGDDbContexts()) {

            }

        }
    }
}
