using RestSharp;
using System;
using System.Collections.Generic;

namespace Topvisor.Api
{
    public interface IApiClient
    {
        /// <summary>
        /// Возвращает типизованное значение результата
        /// из стандартного сообщения ответа.
        /// </summary>
        /// <typeparam name="T">Тип результата.</typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        T GetResponseResult<T>(IRestRequest request);

        /// <summary>
        /// Возвращает целочисленное значение результата
        /// из стандартного сообщения ответа.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        int GetIntResponse(IRestRequest request);

        /// <summary>
        /// Возвращает булево значение результата.
        /// из стандартного сообщения ответа.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        bool GetBoolResponse(IRestRequest request);

        /// <summary>
        /// Возвращает ответ в виде коллекции объектов.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        IEnumerable<T> GetResponseObjects<T>(IRestRequest request)
            where T : IApiObject;
    }
}
