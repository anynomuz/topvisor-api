using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topvisor.Api
{
    public class ApiResponseResult<T> : ApiResponse
    {
        [DeserializeAs(Name = "result")]
        public T Result { get; set; }
    }
}
