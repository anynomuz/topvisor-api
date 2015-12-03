using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topvisor.Api
{
    /// <summary>
    /// Ответ получаемый по Api.
    /// </summary>
    public class ApiResponse
    {
        [DeserializeAs(Name = "result")]
        public int Result { get; set; }

        [DeserializeAs(Name = "error")]
        public bool Error { get; set; }

        [DeserializeAs(Name = "message")]
        public string Message { get; set; }
    }
}
