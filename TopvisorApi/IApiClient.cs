using RestSharp;
using System;
using System.Collections.Generic;

namespace Topvisor.Api
{
    public interface IApiClient
    {
        /// <summary>
        /// Возвращает ответ в виде коллекции объектов.
        /// </summary>
        /// <typeparam name="T">Тип возвращаемых объектов.</typeparam>
        /// <param name="request">Запрос на получение объектов.</param>
        /// <returns></returns>
        IEnumerable<T> GetObjects<T>(ApiRequest<IEnumerable<T>> request)
            where T : IApiObject;

        /// <summary>
        /// Возвращает типизированное стандартное сообщение ответа.
        /// </summary>
        /// <typeparam name="T">Тип результата.</typeparam>
        /// <param name="request">Запрос на получение сообщения.</param>
        /// <param name="throwIfErrorMessage">
        /// Генерировать ли исключение при ошибке в сообщении.
        /// </param>
        /// <returns></returns>
        ApiMessageResult<T> GetMessage<T>(
            ApiRequestMessage<T> request, bool throwIfErrorMessage = true);

        /// <summary>
        /// Возвращает типизованное значение результата
        /// из стандартного сообщения ответа.
        /// </summary>
        /// <typeparam name="T">Тип результата.</typeparam>
        /// <param name="request">Запрос на получение сообщения.</param>
        /// <returns></returns>
        T GetMessageResult<T>(ApiRequestMessage<T> request);

        /// <summary>
        /// Возвращает булево значение результата.
        /// из стандартного сообщения ответа.
        /// </summary>
        /// <param name="request">Запрос на получение сообщения.</param>
        /// <returns></returns>
        bool GetBoolResult(ApiRequestMessage<int> request);
    }
}
