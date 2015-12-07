using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topvisor.Api;

namespace SyncConsoleApp
{
    /// <summary>
    /// Запрос синхронизации.
    /// </summary>
    public class SyncRequest
    {
        private readonly IRestRequest _request;
        private readonly Action<int> _setResult;

        public SyncRequest(SyncRequestType type, IRestRequest request, Action<int> setResult)
	    {
            Type = type;
            _request = request;
            _setResult = setResult;
	    }

#if DEBUG

        public SyncRequest(
            SyncRequestType type, IRestRequest request, Action<int> setResult, object obj)
            : this(type, request, setResult)
        {
            DebugObject = obj;
        }

        /// <summary>
        /// Отладочный объект.
        /// </summary>
        public object DebugObject { get; set; }

#endif

        /// <summary>
        /// Тип запрос.
        /// </summary>
        public SyncRequestType Type { get; private set; }

        /// <summary>
        /// Выполняет запрос.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public int ExecRequest(ApiClient client)
        {
            var id = client.GetIdResponse(_request);
            _setResult(id);

            return id;
        }
    }
}
