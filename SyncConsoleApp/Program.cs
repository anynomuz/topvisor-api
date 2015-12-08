using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Topvisor.Api;
using Topvisor.Xml;

namespace SyncAppConsole
{
    public class Program
    {
        private const string FileName = "topvisor-proj.xml";

        static void Main(string[] args)
        {
            try
            {
                if (!File.Exists(FileName))
                {
                    var newRegistry = ProjectGenerator.GenRegistry(3, 10);
                    newRegistry.Save(FileName);
                }

                var registry = XmlRegistry.Load(FileName);

                // Валидация реестра, в первую очередь урлы в проектах и словах
                XmlRegistry.ValidateRegistry(registry);

                //------

                var apiKey = ConfigurationManager.AppSettings["apikey"];
                if (string.IsNullOrEmpty(apiKey))
                {
                    throw new InvalidOperationException(
                        "Invalid 'apikey' setting in application config.");
                }

                var config = new ClientConfig(apiKey);
                var client = new ApiClient(config);

                var syncClient = new SyncClient(client);

                Console.Write("Project's data loading...");
                syncClient.LoadSyncObjects();

                Console.WriteLine();
                Console.Write("Project's synchronization...");
                syncClient.SyncProjects(registry.Projects);

                Console.WriteLine();
                Console.Write("Groups's synchronization...");
                syncClient.SyncGroups(registry.Projects);

                Console.WriteLine();
                Console.Write("Keywords's synchronization...");
                syncClient.SyncKeywords(registry.Projects);

                Console.WriteLine();
                Console.Write("Synchronization complited, press any key...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine("Error:");
                Console.WriteLine(ex.ToString());
                Console.ResetColor();

                Console.Write("Press any key...");
                Console.ReadKey();
            }
        }
    }
}
