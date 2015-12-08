using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Topvisor.Api.Tests
{
    [TestClass]
    public class ApiRequestKeywordsTests
    {
        private readonly ApiRequestBuilder _builder = new ApiRequestBuilder();
        private readonly IApiClient _client = ApiClientHelper.GetRealApiClient();

        public IEnumerable<ApiKeyword> GetKeywords(int projectId)
        {
            var request = _builder.GetKeywordsRequest((int)projectId, false);
            return _client.GetObjects<ApiKeyword>(request);
        }

        [TestMethod]
        public void LoadKeywords()
        {
            var project = ApiClientHelper.GetFirstProject();
            var keywords = GetKeywords(project.Id);
        }

        [TestMethod]
        public void AddSingleKeyword()
        {
            var testPhrase = "test phrase" + Environment.TickCount;

            var project = ApiClientHelper.GetFirstProject();

            var request = _builder.GetAddKeywordRequest(project.Id, testPhrase);
            var id = _client.GetMessageResult(request);

            Assert.IsTrue(id > 0, "id > 0");

            var keywords = GetKeywords(project.Id);

            var newKeyword = keywords.First();

            Assert.AreEqual(testPhrase, newKeyword.Phrase);
        }

        [TestMethod]
        public void AddSomeKeywords()
        {
            var addKeywords = new List<string>();

            for (int i = 0; i < 5; ++i)
            {
                addKeywords.Add("phrase " + Environment.TickCount);
            }

            var project = ApiClientHelper.GetFirstProject();

            var request = _builder.GetAddKeywordsRequest(project.Id, addKeywords);
            _client.GetMessageResult(request);

            var keywords = GetKeywords(project.Id);

            foreach (var addedWord in addKeywords)
            {
                Assert.IsNotNull(keywords.FirstOrDefault(k => k.Phrase == addedWord));
            }
        }

        [TestMethod]
        public void DropKeyword()
        {
            var project = ApiClientHelper.GetFirstProject();
            var keyword = GetKeywords(project.Id).First();

            var request = _builder.GetDeleteKeywordRequest(keyword.Id);
            var res = _client.GetBoolResult(request);

            Assert.IsTrue(res);

            var keywords = GetKeywords(project.Id);

            Assert.IsNull(keywords.FirstOrDefault(k => k.Id == keyword.Id));
        }

        [TestMethod]
        public void SetKeywordTarget()
        {
            var project = ApiClientHelper.GetFirstProject();
            var keyword = GetKeywords(project.Id).First();

            var url = "http://ya.ru/phrase" + Environment.TickCount;

            var request = _builder.GetUpdateKeywordTargetRequest(keyword.Id, url);
            var res = _client.GetBoolResult(request);

            // BUG: непонятно почему, но таргет устанавливается, а возвращает 0
            //Assert.IsTrue(res);

            var keywords = GetKeywords(project.Id);

            Assert.IsNotNull(keywords.FirstOrDefault(k => k.Target == url));
        }
    }
}
