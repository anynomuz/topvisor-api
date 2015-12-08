using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topvisor.Api.Tests
{
    [TestClass]
    public class ApiRequestBuilderTests
    {
        private readonly ApiRequestBuilder _builder = new ApiRequestBuilder();
        private readonly IApiClient _client = ApiClientHelper.GetRealApiClient();


        [TestMethod]
        public IEnumerable<ApiProject> GetProjects()
        {
            var client = ApiClientHelper.GetRealApiClient();
            var request = _builder.GetProjectsRequest();
            return client.GetResponseObjects<ApiProject>(request);
        }

        public ApiProject GetFirstProject(int? id = null)
        {
            return (id == null)
                ? GetProjects().First()
                : GetProjects().First(p => p.Id == (int)id);
        }

        [TestMethod]
        public void AddProject()
        {
            var site = "http://ya.ru";

            var request = _builder.GetAddProjectRequest(site);
            var projectId = _client.GetIntResponse(request);

            Assert.IsTrue(projectId >= 0, "id >= 0");

            var newProject = GetFirstProject(projectId);

            Assert.AreEqual(site.Replace("http://", ""), newProject.Site);
        }

        [TestMethod]
        public void DropProject()
        {
            var dropProject = GetFirstProject();

            var request = _builder.GetDeleteProjectRequest(dropProject.Id);
            var res = _client.GetBoolResponse(request);

            Assert.IsTrue(res);

            var newProjects = GetProjects();

            Assert.IsNull(newProjects.FirstOrDefault(p => p.Id == dropProject.Id));
        }

        [TestMethod]
        public void EnableDisableProject()
        {
            var project = GetFirstProject();
            var newState = (project.On == 0) ? -1 : 0;

            var request = _builder.GetUpdateProjectRequest(project.Id, newState);
            var res = _client.GetBoolResponse(request);

            Assert.IsTrue(res);

            var newProject = GetFirstProject(project.Id);

            Assert.AreEqual(newProject.On, newState);
        }

        [TestMethod]
        public IEnumerable<ApiKeyword> GetKeywords(int? projectId = null)
        {
            if (projectId == null)
            {
                projectId = GetFirstProject(projectId).Id;
            }

            var request = _builder.GetKeywordsRequest((int)projectId, false);
            return _client.GetResponseObjects<ApiKeyword>(request);
        }

        [TestMethod]
        public void AddKeyword()
        {
            //var proj = get
            //var keywords = GetKeywords();
            //_builder.GetAddKeywordsRequest()
        }

        [TestMethod]
        public void DropKeyword()
        {
            // TODO: Написать тесты на кейворды
        }

        [TestMethod]
        public void SetKeywordTarget()
        {
        }
    }
}
