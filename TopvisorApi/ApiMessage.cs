using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopvisorApi
{
    /// <summary>
    /// Сообщение получаемое по Api.
    /// </summary>
    public class ApiMessage
    {
        [DeserializeAs(Name = "message")]
        public string Message { get; set; }

        [DeserializeAs(Name = "error")]
        public bool Error { get; set; }
    }
}
