using RestSharp;
using RestSharp.Deserializers;
using SyncAppTests.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topvisor.Api;
using Topvisor.Xml;

namespace SyncAppTests
{
    public static class SyncTestHelper
    {
        private static JsonDeserializer _deserailizer = new JsonDeserializer();

        public static IEnumerable<ApiProject> GetProjects()
        {
            return GetJsonObjects<ApiProject>(Resources.test1_projects1);
        }

        public static IEnumerable<ApiKeyword> GetKeywords()
        {
            return GetJsonObjects<ApiKeyword>(Resources.test1_keywords);
        }

        public static XmlRegistry GetRegistry()
        {
            var bytes = Encoding.UTF8.GetBytes(Resources.test1_projects);
            return XmlRegistry.Load(new MemoryStream(bytes));
        }

        private static IEnumerable<T> GetJsonObjects<T>(byte[] data)
        {
            var response = new RestResponse();
            response.Content = Encoding.UTF8.GetString(data);
            return _deserailizer.Deserialize<List<T>>(response);
        }
    }
}
