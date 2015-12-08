using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topvisor.Api
{
    /// <summary>
    /// Запрос на получение типизированных данных по Api.
    /// </summary>
    /// <typeparam name="T">Тип ожидаемого ответа.</typeparam>
    public class ApiRequest<T>
    {
        public ApiRequest(IRestRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            Request = request;
        }

        public IRestRequest Request { get; private set; }
    }
}
