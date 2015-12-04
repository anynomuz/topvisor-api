using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topvisor.Api
{
    /// <summary>
    /// Конфиг клиента api.
    /// </summary>
    public class ClientConfig
    {
        private const int _maxRequestPerSecond = 10;
        private readonly Uri _baseUrl;

        public ClientConfig(string apiKey)
        {
            ApiKey = apiKey;
            _baseUrl = new Uri("https://api.topvisor.ru");

            MaxRequestPerSecond = _maxRequestPerSecond;
        }

        /// <summary>
        /// Базовый адрес.
        /// </summary>
        public Uri BaseUrl
        {
            get { return _baseUrl; }
        }

        /// <summary>
        /// Ключ клиента.
        /// </summary>
        public string ApiKey { get; private set; }

        /// <summary>
        /// Лимит запросов в секунду.
        /// </summary>
        public int MaxRequestPerSecond { get; private set; }
    }
}
