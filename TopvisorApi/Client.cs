using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopvisorApi
{
    public class Client
    {
        private readonly ClientConfig _cfg;

        public Client(ClientConfig cfg)
        {
            if (cfg == null)
            {
                throw new ArgumentNullException("cfg");
            }

            _cfg = cfg;
        }

        public IEnumerable<object> GetProjects()
        {
            
        }

        private string GetProjectsRequest()
        {

        }
    }
}
