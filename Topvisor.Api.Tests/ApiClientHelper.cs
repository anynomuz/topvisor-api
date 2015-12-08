using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topvisor.Api.Tests
{
    public static class ApiClientHelper
    {
        private const string _debugApiKey = "768a9f24525eca4b84fe";

        private static ApiClient _realApiClient;

        public static ApiClient GetRealApiClient()
        {
            if (_realApiClient == null)
            {
                var config = new ClientConfig(_debugApiKey);
                _realApiClient = new ApiClient(config);
            }

            return _realApiClient;
        }

        public static MockApiClient GetMockApiClient()
        {
            return new MockApiClient();
        }
    }
}
