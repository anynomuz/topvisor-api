using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topvisor.Api.Tests
{
    [TestClass]
    public class ApiRequestProjectsTests
    {
        private readonly ApiRequestBuilder _builder = new ApiRequestBuilder();
        private readonly IApiClient _client = ApiClientHelper.GetRealApiClient();

        [TestMethod]
        public void LoadProjects()
        {
            var projects = ApiClientHelper.GetProjects();
        }

        [TestMethod]
        public void AddProject()
        {
            var site = "http://ya.ru";

            var request = _builder.GetAddProjectRequest(site);
            var projectId = _client.GetMessageResult(request);

            Assert.IsTrue(projectId >= 0, "id >= 0");

            var newProject = ApiClientHelper.GetFirstProject(projectId);

            Assert.AreEqual(site.Replace("http://", ""), newProject.Site);
        }

        [TestMethod]
        public void DropProject()
        {
            var dropProject = ApiClientHelper.GetFirstProject();

            var request = _builder.GetDeleteProjectRequest(dropProject.Id);
            var res = _client.GetBoolResult(request);

            Assert.IsTrue(res);

            var newProjects = ApiClientHelper.GetProjects();

            Assert.IsNull(newProjects.FirstOrDefault(p => p.Id == dropProject.Id));
        }

        [TestMethod]
        public void EnableDisableProject()
        {
            var project = ApiClientHelper.GetFirstProject();
            var newState = (project.On == 0) ? -1 : 0;

            var request = _builder.GetUpdateProjectRequest(project.Id, newState);
            var res = _client.GetBoolResult(request);

            Assert.IsTrue(res);

            var projects = ApiClientHelper.GetProjects(false);
            var newProject = projects.First(p => project.Id == p.Id);

            Assert.AreEqual(newProject.On, newState);
        }
    }
}
