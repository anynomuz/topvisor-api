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
        public IEnumerable<T> GetObjects<T>(ApiRequest<IEnumerable<T>> request)
            where T : IApiObject
        {
            throw new NotImplementedException();
        }

        public ApiMessageResult<T> GetMessage<T>(ApiRequestMessage<T> request)
            where T : new()
        {
            throw new NotImplementedException();
        }

        public T GetMessageResult<T>(ApiRequestMessage<T> request)
            where T : new()
        {
            throw new NotImplementedException();
        }

        public bool GetBoolResult(ApiRequestMessage<int> request)
        {
            throw new NotImplementedException();
        }
    }
}
