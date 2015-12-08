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
    public class ApiClient : IApiClient
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
        /// <typeparam name="T">Тип возвращаемых объектов.</typeparam>
        /// <param name="request">Запрос на получение объектов.</param>
        /// <returns></returns>
        public IEnumerable<T> GetObjects<T>(ApiRequest<IEnumerable<T>> request)
            where T : IApiObject
        {
            WaitBeforeRequestIfNeeded();
            var response = _client.Execute<List<T>>(request.Request);

            ThrowIfError(response);

            var apiResponse = TryGetMessage(response);
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
        /// Возвращает типизированное стандартное сообщение ответа.
        /// </summary>
        /// <typeparam name="T">Тип результата.</typeparam>
        /// <param name="request">Запрос на получение сообщения.</param>
        /// <returns></returns>
        public ApiMessageResult<T> GetMessage<T>(ApiRequestMessage<T> request)
            where T : new()
        {
            WaitBeforeRequestIfNeeded();
            var response = _client.Execute<ApiMessageResult<T>>(request.Request);

            ThrowIfError(response);
            return response.Data;
        }

        /// <summary>
        /// Возвращает типизованное значение результата
        /// из стандартного сообщения ответа.
        /// </summary>
        /// <typeparam name="T">Тип результата.</typeparam>
        /// <param name="request">Запрос на получение сообщения.</param>
        /// <returns></returns>
        public T GetMessageResult<T>(ApiRequestMessage<T> request)
            where T : new()
        {
            var message = GetMessage(request);
            return message.Result;
        }

        /// <summary>
        /// Возвращает булево значение результата.
        /// из стандартного сообщения ответа.
        /// </summary>
        /// <param name="request">Запрос на получение сообщения.</param>
        /// <returns></returns>
        public bool GetBoolResult(ApiRequestMessage<int> request)
        {
            return GetMessageResult(request) > 0;
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

            var apiResponse = response.Data as ApiMessage;
            ThrowIfError(apiResponse);
        }

        private ApiMessage TryGetMessage<T>(IRestResponse<T> response)
        {
            try
            {
                return _deserailizer.Deserialize<ApiMessage>(response);
            }
            catch (Exception)
            {
                // некрасиво, но куда деваться..
                return response.Data as ApiMessage;
            }
        }

        private static void ThrowIfError(ApiMessage apiResponse)
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
