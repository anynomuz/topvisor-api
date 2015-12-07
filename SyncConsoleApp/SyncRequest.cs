using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncConsoleApp
{
    public class SyncRequest
    {
        public SyncRequest(SyncRequestType type, IRestRequest request, Action<int> result)
	    {
	    }

        public SyncRequestType Type { get; private set; }
    }
}
