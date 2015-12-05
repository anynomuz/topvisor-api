using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using RestSharp;
using Topvisor.Api;
using Topvisor.Xml;

namespace SyncConsoleApp
{
    public class Program
    {
        private const string FileName = "registry.xml";

        static void Main(string[] args)
        {
            if (!File.Exists(FileName))
            {
                var newRegistry = GenTestRegistry(3);
                newRegistry.Save(FileName);
            }

            var registry = XmlRegistry.Load(FileName);

#warning Отвалидировать урлы в проектах и словах

            //------
            var config = new ClientConfig("768a9f24525eca4b84fe");

            ////var rb = new ApiRequestBuilder();
            ////var client = new ApiClient(config);

            ////var request = rb.GetUpdateProjectRequest(399070, true);
            ////var res = client.ExecQueryBool(request);

            ////return;

            var syncClient = new SyncClient(config);

            syncClient.LoadApiObjects();
            syncClient.AddProjects(registry.Projects);
            syncClient.UpdateProjects(registry.Projects);
            syncClient.DeleteProjects(registry.Projects);
        }

        private static XmlRegistry GenTestRegistry(int projectsCount)
        {
            var gen = new ProjectGenerator();
            var projects = gen.CreateProjects(projectsCount, 2 * projectsCount);
            return new XmlRegistry(projects);
        }
    }
}
