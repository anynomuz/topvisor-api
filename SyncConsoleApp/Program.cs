using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Topvisor.Api;
using Topvisor.Xml;

namespace SyncConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ////var config = new ClientConfig("");
            ////var client = new Client(config);
            ////var projects = client.GetProjects();
            ////var keywords = client.GetKeywords(396790);

            var fileName = "project1.xml";

            var registry = GetTestRegistry();
            registry.Save(fileName);
            var reg = XmlRegistry.Load(fileName);
        }

        private static XmlRegistry GetTestRegistry()
        {
            var project = new XmlProject("Name1", "Comment1");

            var group = new XmlKeywordGroup("Group1");
            project.KeywordGroups.Add(group);

            group.Keywords.Add(new XmlKeyword("Phrase1", "url1"));
            group.Keywords.Add(new XmlKeyword("Phrase2", "url2"));

            return new XmlRegistry(new[] { project });
        }
    }
}
