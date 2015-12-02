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
        private const string _apiUrl = "https://api.topvisor.ru";
        private const int _maxRequestPerSecond = 10;

        private readonly string _baseUrl;

        public ClientConfig(string id)
        {
            Id = id;
            MaxRequestPerSecond = _maxRequestPerSecond; 
        }

        /// <summary>
        /// Ключ клиента.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Лимит запросов в секунду.
        /// </summary>
        public int MaxRequestPerSecond { get; private set; }

        public string GetBaseUrl()
        {
            return _apiUrl;
        }
    }
}
