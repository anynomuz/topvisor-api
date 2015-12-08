using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topvisor.Api
{
    /// <summary>
    /// Запрос на получение типизированного сообщения Api.
    /// </summary>
    /// <typeparam name="T">Тип поля результата в сообщении.</typeparam>
    public class ApiRequestMessage<T> : ApiRequest<ApiMessageResult<T>>
    {
        public ApiRequestMessage(IRestRequest request)
            : base(request)
        {
        }
    }
}
