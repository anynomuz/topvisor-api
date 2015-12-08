using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Topvisor.Api.Tests
{
    [TestClass]
    public class ApiRequestGroupsTests
    {
        private readonly ApiRequestBuilder _builder = new ApiRequestBuilder();
        private readonly IApiClient _client = ApiClientHelper.GetRealApiClient();

        [TestMethod]
        public void AddGroup()
        {
            var groupName = "test-group";

            var project = ApiClientHelper.GetFirstProject();
            var request = _builder.GetAddKeywordGroupRequest(project.Id, groupName);
            var id = _client.GetMessageResult(request);

            Assert.IsTrue(id > 0, "id > 0");
        }

        //[TestMethod]
        //public void DeleteGroup()
        //{
        //    // TODO: Написать тесты на удаление группы
        //    var project = ApiClientHelper.GetFirstProject();
        //}

        //[TestMethod]
        //public void EnableDisableGroup()
        //{
        //    // TODO: Написать тесты на выключение группы
        //    var project = ApiClientHelper.GetFirstProject();
        //}
    }
}
