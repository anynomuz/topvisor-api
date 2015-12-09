using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topvisor.Api
{
    /// <summary>
    /// Иключение, генериуется при ошибках запросов.
    /// </summary>
    public class InvalidRequestException : Exception
    {
        public InvalidRequestException(
            string message, Exception innerException, string responseContent)
            : base(message, innerException)
        {
            ResponseContent = responseContent;
        }

        public InvalidRequestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public InvalidRequestException(string message, string responseContent)
            : base(message)
        {
            ResponseContent = responseContent;
        }

        public string ResponseContent { get; private set; }
    }
}
