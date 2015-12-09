using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;

namespace Topvisor.Api.Tests
{
    [TestClass]
    public class ApiClientTests
    {
        [TestMethod]
        public void GetMessageOnInvalidRequest()
        {
            var client = ApiClientHelper.GetRealApiClient();

            var request = new ApiRequestMessage<object>(
                new RestRequest(Method.GET));

            var message = client.GetMessage(request, false);

            Assert.IsNotNull(message);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRequestException))]
        public void ThrowOnInvalidRequest1()
        {
            var client = ApiClientHelper.GetRealApiClient();

            var request = new ApiRequestMessage<object>(
                new RestRequest(Method.GET));

            client.GetMessage(request, true);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRequestException))]
        public void ThrowOnInvalidRequest2()
        {
            var client = ApiClientHelper.GetRealApiClient();

            var builder = new ApiRequestBuilder();
            var request = builder.GetProjectsRequest();
            request.Request.Parameters[1].Name = "invalid-fake-param";
            
            var res = client.GetObjects<ApiProject>(request);
        }
    }
}
