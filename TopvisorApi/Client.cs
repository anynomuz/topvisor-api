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
        }

        public IEnumerable<ApiProject> GetProjects()
        {
            var request = new RestRequest(Method.GET);

            request.AddParameter("api_key", _config.Id);
            request.AddParameter("oper", "get");
            request.AddParameter("module", "mod_projects");

            var res = _client.Execute<List<ApiProject>>(request);

            return GetValidatedData(res);
        }

        public IEnumerable<ApiKeyword> GetKeywords(int pojectId)
        {
            var request = new RestRequest(Method.GET);

            request.AddParameter("api_key", _config.Id);
            request.AddParameter("oper", "get");
            request.AddParameter("module", "mod_keywords");
            request.AddParameter("post[project_id]", pojectId);

            var res = _client.Execute<List<ApiKeyword>>(request);

            return GetValidatedData(res);
        }

        private T GetValidatedData<T>(IRestResponse<T> response)
        {
            ThrowIfError((IRestResponse)response);

            var message = TryGetMesage(response);

            if ((message != null) && (message.Error))
            {
                throw new ApplicationException(message.Message);
            }

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

        private ApiMessage TryGetMesage<T>(IRestResponse<T> response)
        {
            if ((response.Data == null) && (response.StatusCode == HttpStatusCode.OK))
            {
                return _deserailizer.Deserialize<ApiMessage>(response);
            }

            return null;
        }

        private static void ThrowIfError(IRestResponse response)
        {
            if (response.ErrorException != null)
            {
                throw new ApplicationException(
                    "Error retrieving response.  Check inner details for more info.",
                    response.ErrorException);
            }
        }
    }
}
