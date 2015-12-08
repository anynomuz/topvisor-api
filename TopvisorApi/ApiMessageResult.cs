using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topvisor.Api
{
    /// <summary>
    /// Сообщение получаемое по Api c типизированным результатом.
    /// </summary>
    /// <typeparam name="T">Тип результата.</typeparam>
    public class ApiMessageResult<T> : ApiMessage
    {
        [DeserializeAs(Name = "result")]
        public T Result { get; set; }
    }
}
