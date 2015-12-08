using RestSharp;
using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Topvisor.Api
{
    /// <summary>
    /// Клиент для доступа к Api Топвизора.
    /// </summary>
    public class ApiClient : Topvisor.Api.IApiClient
    {
        private readonly IRestClient _client;
        private readonly IDeserializer _deserailizer;
        private readonly int _maxRequestPerSecond;

        private int _totalRequestCounter = 0;

        private DateTime _lastLimitBoundTime = DateTime.Now;

        public ApiClient(ClientConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            _client = new RestClient(config.GetBaseUrl());
            _maxRequestPerSecond = config.MaxRequestPerSecond;

            _deserailizer = new JsonDeserializer();
        }

        /// <summary>
        /// Возвращает ответ в виде коллекции объектов.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public IEnumerable<T> GetResponseObjects<T>(IRestRequest request)
            where T : IApiObject
        {
            WaitBeforeRequestIfNeeded();
            var response = _client.Execute<List<T>>(request);

            ThrowIfError(response);

            var apiResponse = TryGetResponseMessage(response);
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

        /// <summary>
        /// Возвращает типизованное значение результата
        /// из стандартного сообщения ответа.
        /// </summary>
        /// <typeparam name="T">Тип результата.</typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public T GetResponseResult<T>(IRestRequest request)
        {
            WaitBeforeRequestIfNeeded();
            var response = _client.Execute<ApiResponseResult<T>>(request);

            ThrowIfError(response);
            return response.Data.Result;
        }

        /// <summary>
        /// Возвращает целочисленное значение результата
        /// из стандартного сообщения ответа.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public int GetIntResponse(IRestRequest request)
        {
            return GetResponseResult<int>(request);
        }

        /// <summary>
        /// Возвращает булево значение результата.
        /// из стандартного сообщения ответа.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool GetBoolResponse(IRestRequest request)
        {
            return GetResponseResult<int>(request) > 0;
        }

        /// <summary>
        /// Вызывается перед выполнением запроса.
        /// При необходимости ждет в соответствии с лимитами запросов.
        /// </summary>
        private void WaitBeforeRequestIfNeeded()
        {
            ++_totalRequestCounter;

            if ((_totalRequestCounter % _maxRequestPerSecond) == 0)
            {
                var diff = DateTime.Now - _lastLimitBoundTime;

                if (diff.TotalMilliseconds < 1000)
                {
                    Thread.Sleep((int)diff.TotalMilliseconds);
                }

                _lastLimitBoundTime = DateTime.Now;
            }
        }

        private static void ThrowIfError<T>(IRestResponse<T> response)
        {
            if (response.ErrorException != null)
            {
                throw new InvalidOperationException(
                    "Error retrieving response. Check inner details for more info.",
                    response.ErrorException);
            }

            var apiResponse = response.Data as ApiResponse;
            ThrowIfError(apiResponse);
        }

        private ApiResponse TryGetResponseMessage<T>(IRestResponse<T> response)
        {
            try
            {
                return _deserailizer.Deserialize<ApiResponse>(response);
            }
            catch (Exception)
            {
                // некрасиво, но куда деваться..
                return response.Data as ApiResponse;
            }
        }

        private static void ThrowIfError(ApiResponse apiResponse)
        {
            if ((apiResponse != null) && apiResponse.Error)
            {
                var error = string.IsNullOrEmpty(apiResponse.Message)
                    ? "Unknown error response."
                    : apiResponse.Message;

                throw new InvalidOperationException(error);
            }
        }
    }
}
