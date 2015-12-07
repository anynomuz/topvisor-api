using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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

            var apiKey = ConfigurationManager.AppSettings["apikey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException(
                    "Invalid 'apikey' setting in app.config.");
            }

            //------
            var config = new ClientConfig(apiKey);
            var client = new ApiClient(config);

            ////var rb = new ApiRequestBuilder();
            

            ////var request = rb.GetGroupsRequest(399214, false);
            ////var res = client.GetObjects<ApiKeywordGroup>(request);

            ////return;

            ////var syncClient = new SyncClient(config);

            ////syncClient.LoadApiObjects();
            ////syncClient.AddProjects(registry.Projects);
            ////syncClient.UpdateProjects(registry.Projects);
            ////syncClient.DeleteProjects(registry.Projects);

            var syncClient = new SyncClient2(config);
            syncClient.LoadApiObjects(client);

            var list = new List<SyncRequest>();

            list.AddRange(syncClient.AddProjects(registry.Projects));
            list.AddRange(syncClient.DeleteProjects(registry.Projects));
            list.AddRange(syncClient.UpdateProjectsProperties(registry.Projects));

            list.AddRange(syncClient.AddGroups(registry.Projects));
            list.AddRange(syncClient.DeleteGroups(registry.Projects));
            list.AddRange(syncClient.UpdateGroupsProperties(registry.Projects));
        }

        private static XmlRegistry GenTestRegistry(int projectsCount)
        {
            var gen = new ProjectGenerator();
            var projects = gen.CreateProjects(projectsCount, 10);
            return new XmlRegistry(projects);
        }
    }
}
