using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;

namespace Topvisor.Api.Tests
{
    [TestClass]
    public class ApiClientTests
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ThrowOnInvalidRequest1()
        {
            var client = ApiClientHelper.GetRealApiClient();
            var request = new RestRequest(Method.GET);
            client.GetResponseResult<string>(request);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ThrowOnInvalidRequest2()
        {
            var client = ApiClientHelper.GetRealApiClient();

            var builder = new ApiRequestBuilder();
            var request = builder.GetProjectsRequest();
            request.Parameters[1].Name = "invalid-fake-param";
            
            var res = client.GetResponseObjects<ApiProject>(request);
        }
    }
}
