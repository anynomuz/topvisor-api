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

            var syncClient = new SyncClient(client);
            syncClient.LoadSyncObjects();

            syncClient.SyncProjects(registry.Projects);
            syncClient.SyncGroups(registry.Projects);
            syncClient.SyncKeywords(registry.Projects);

        }

        private static XmlRegistry GenTestRegistry(int projectsCount)
        {
            var gen = new ProjectGenerator();
            var projects = gen.CreateProjects(projectsCount, 10);
            return new XmlRegistry(projects);
        }
    }
}
