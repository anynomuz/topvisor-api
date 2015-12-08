using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topvisor.Api.Tests
{
    public class MockApiClient : IApiClient
    {
        public T GetResponseResult<T>(IRestRequest request)
        {
            throw new NotImplementedException();
        }

        public int GetIntResponse(IRestRequest request)
        {
            throw new NotImplementedException();
        }

        public bool GetBoolResponse(IRestRequest request)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetResponseObjects<T>(IRestRequest request)
            where T : IApiObject
        {
            throw new NotImplementedException();
        }
    }
}
