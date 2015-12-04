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
    public class ApiClient
    {
        private readonly IRestClient _client;
        private readonly IDeserializer _deserailizer;

        public ApiClient(ClientConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            _client = new RestClient();
            _client.BaseUrl = config.BaseUrl;

            _deserailizer = new JsonDeserializer();
        }

        public IEnumerable<T> GetObjects<T>(IRestRequest request)
        {
            var response = _client.Execute<List<T>>(request);

            ThrowIfError(response);

            var apiResponse = TryGetResponse(response);
            ThrowIfError(apiResponse);

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

        public int ExecIdQuery(IRestRequest request)
        {
            var response = _client.Execute<ApiResponse>(request);
            ThrowIfError(response);
            return response.Data.Result;
        }

        public bool ExecBoolQuery(IRestRequest request)
        {
            var response = _client.Execute<ApiResponse>(request);
            ThrowIfError(response);
            return response.Data.Result > 0;
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
