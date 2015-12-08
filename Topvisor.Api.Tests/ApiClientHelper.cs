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
        private static ApiRequestBuilder _builder = new ApiRequestBuilder();

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

        public static IEnumerable<ApiProject> GetProjects(bool onlyEnabled = true)
        {
            var client = ApiClientHelper.GetRealApiClient();
            var request = _builder.GetProjectsRequest();

            var projects =  client.GetObjects<ApiProject>(request);

            return (onlyEnabled)
                ? projects.Where(p => p.On >= 0).ToList()
                : projects.ToList();
        }

        public static ApiProject GetFirstProject(int? id = null)
        {
            return (id == null)
                ? GetProjects().First()
                : GetProjects().First(p => p.Id == (int)id);
        }
    }
}
