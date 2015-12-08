using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Topvisor.Xml.Tests
{
    [TestClass]
    public class ValidationTests
    {
        [TestMethod]
        public void GeneratedRegistryIsValid()
        {
            var reg = ProjectGenerator.GenRegistry(3, 10);
            XmlRegistry.ValidateRegistry(reg);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void EmptyProjectSiteIsInvalid()
        {
            var reg = ProjectGenerator.GenRegistry(1, 0);
            reg.Projects[0].Site = "";

            XmlRegistry.ValidateRegistry(reg);
        }

        [TestMethod]
        public void EmptyKeywordTargetUrlIsValid()
        {
            var reg = ProjectGenerator.GenRegistry(1, 0);
            var group = reg.Projects[0].KeywordGroups.First();

            group.Keywords.Clear();
            group.Keywords.Add(new XmlKeyword("test", null));

            XmlRegistry.ValidateRegistry(reg);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void IncorrectKeywordTargetUrlIsInvalid()
        {
            var reg = ProjectGenerator.GenRegistry(1, 0);
            var group = reg.Projects[0].KeywordGroups.First();

            group.Keywords.Clear();
            group.Keywords.Add(new XmlKeyword("test", "nourl"));

            XmlRegistry.ValidateRegistry(reg);
        }
    }
}
