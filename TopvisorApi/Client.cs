using RestSharp;
using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Topvisor.Api
{
    /// <summary>
    /// Клиент для доступа к Api Топвизора.
    /// </summary>
    public class Client
    {
        private readonly ClientConfig _config;
        private readonly IRestClient _client;
        private readonly IDeserializer _deserailizer;

        private readonly ApiRequestBuilder _requestBuilder;

        public Client(ClientConfig cfg)
        {
            if (cfg == null)
            {
                throw new ArgumentNullException("cfg");
            }

            _config = cfg;
            _client = new RestClient();
            _client.BaseUrl = new Uri(_config.GetBaseUrl());

            _deserailizer = new JsonDeserializer();
            _requestBuilder = new ApiRequestBuilder(_config.ApiKey);
        }

        public IEnumerable<ApiProject> GetProjects()
        {
            var request = _requestBuilder.GetProjectsRequest();

            var res = _client.Execute<List<ApiProject>>(request);
            return GetValidatedData(res);
        }

        public IEnumerable<ApiKeyword> GetKeywords(
            int projectId, bool onlyEnabled, int groupId = -1)
        {
            var request = _requestBuilder.GetKeywordsRequest(
                projectId, onlyEnabled, groupId);

            var res = _client.Execute<List<ApiKeyword>>(request);
            return GetValidatedData(res);
        }

        public int AddProject(string site)
        {
            var request = _requestBuilder.GetAddProjectRequest(site);
            var res = _client.Execute<ApiResponse>(request);

            ThrowIfError(res);
            return res.Data.Result;
        }

        public bool DeleteProject(int id)
        {
            var request = _requestBuilder.GetDeleteProjectRequest(id);
            var res = _client.Execute<ApiResponse>(request);

            ThrowIfError(res);
            return res.Data.Result > 0;
        }

        public bool DisableProject(int id)
        {
            var request = _requestBuilder.GetDisableProjectRequest(id);
            var res = _client.Execute<ApiResponse>(request);

            ThrowIfError(res);
            return res.Data.Result > 0;
        }

        public int AddKeywordGroup(int projectId, string name)
        {
            var request = _requestBuilder.GetAddKeywordGroupRequest(projectId, name);
            var res = _client.Execute<ApiResponse>(request);

            ThrowIfError(res);
            return res.Data.Result;
        }

        public int AddKeywords(int projectId, int groupId, string[] keywords)
        {
            var request = _requestBuilder.GetAddKeywordsRequest(
                projectId, groupId, keywords);

            var res = _client.Execute<ApiResponse>(request);

            ThrowIfError(res);
            return res.Data.Result;
        }


        private T GetValidatedData<T>(IRestResponse<T> response)
        {
            ThrowIfError(response);

            var apiResponse = TryGetResponse(response);
            ThrowIfError(apiResponse);

            var obj = response.Data as IValidable;

            if (obj != null)
            {
                obj.Validate();
            }

            var list = response.Data as IEnumerable<IValidable>;

            if (list != null)
            {
                foreach (var item in list)
                {
                    item.Validate();
                }
            }

            return response.Data;
        }

        private ApiResponse TryGetResponse<T>(IRestResponse<T> response)
        {
            if ((response.Data == null) && (response.StatusCode == HttpStatusCode.OK))
            {
                return _deserailizer.Deserialize<ApiResponse>(response);
            }

            return response.Data as ApiResponse;
        }

        private static void ThrowIfError(ApiResponse apiResponse)
        {
            if ((apiResponse != null) && apiResponse.Error)
            {
                var error = string.IsNullOrEmpty(apiResponse.Message)
                    ? "Unknown error response."
                    : apiResponse.Message;

                throw new ApplicationException(error);
            }
        }

        private static void ThrowIfError<T>(IRestResponse<T> response)
        {
            if (response.ErrorException != null)
            {
                throw new ApplicationException(
                    "Error retrieving response. Check inner details for more info.",
                    response.ErrorException);
            }

            var apiResponse = response.Data as ApiResponse;
            ThrowIfError(apiResponse);
        }
    }
}
