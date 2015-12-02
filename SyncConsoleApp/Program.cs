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

            var project = new XmlProject();

            project.Name = "Name1";
            project.Comment = "Comment1";

            var group = new XmlKeywordGroup();
            group.Name = "Group1";

            project.KeywordGroups.Add(group);

            group.Keywords.Add(new XmlKeyword() { Target = "url1", Phrase = "Phrase1" });
            group.Keywords.Add(new XmlKeyword() { Target = "url2", Phrase = "Phrase2" });

            var ser = new XmlSerializer(typeof(XmlProject));

            using(var stream = new FileStream("project1.xml", FileMode.CreateNew))
            {
                ser.Serialize(stream, project);
            }
        }
    }
}
