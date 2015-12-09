using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Topvisor.Api;
using SyncAppConsole;

namespace SyncAppTests
{
    [TestClass]
    public class SyncClientMockTests
    {
        [TestMethod]
        public void SyncConsistency()
        {
            var xmlRegistry = SyncTestHelper.GetRegistry();

            var apiProjects = SyncTestHelper.GetProjects();
            var apiKeywords = SyncTestHelper.GetKeywords();

            var client = new MockApiClient();
            client.SetProjects(apiProjects);
            client.SetKeywords(apiKeywords);

            var syncClient = new SyncClient(client);
            syncClient.LoadSyncObjects();

            var syncProjectsCount = syncClient.SyncProjects(xmlRegistry.Projects);

            Assert.IsTrue(syncProjectsCount != 0);

            syncProjectsCount = syncClient.SyncProjects(xmlRegistry.Projects);

            Assert.IsTrue(syncProjectsCount == 0);

            var syncGroupsCount = syncClient.SyncGroups(xmlRegistry.Projects);

            Assert.IsTrue(syncGroupsCount != 0);

            syncGroupsCount = syncClient.SyncGroups(xmlRegistry.Projects);

            Assert.IsTrue(syncGroupsCount == 0);

            var syncKeywordsCount = syncClient.SyncKeywords(xmlRegistry.Projects);

            Assert.IsTrue(syncKeywordsCount != 0);

            syncKeywordsCount = syncClient.SyncKeywords(xmlRegistry.Projects);

            Assert.IsTrue(syncKeywordsCount == 0);
        }
    }
}
