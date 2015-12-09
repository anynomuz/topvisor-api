using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topvisor.Api;

namespace SyncAppTests
{
    public class MockApiClient : IApiClient
    {
        private readonly List<ApiProject> _projects = new List<ApiProject>();
        private readonly List<ApiKeyword> _keywords = new List<ApiKeyword>();

        private int _counter = 1000;

        public void SetProjects(IEnumerable<ApiProject> projects)
        {
            _projects.Clear();
            _projects.AddRange(projects);
        }

        public void SetKeywords(IEnumerable<ApiKeyword> keywords)
        {
            _keywords.Clear();
            _keywords.AddRange(keywords);
        }

        public IEnumerable<T> GetObjects<T>(ApiRequest<IEnumerable<T>> request)
            where T : IApiObject
        {
            if (typeof(T) == typeof(ApiProject))
            {
                return _projects as IEnumerable<T>;
            }

            if (typeof(T) == typeof(ApiKeyword))
            {
                // TODO: Еще нужно учитывать enabled_only
                var projectParam = request.Request.Parameters
                    .Find(p => p.Name == "post[project_id]");

                int projectId = (int)projectParam.Value;

                return _keywords.Where(k => k.ProjectId == projectId) as IEnumerable<T>;
            }

            throw new NotImplementedException();
        }

        public ApiMessageResult<T> GetMessage<T>(
            ApiRequestMessage<T> request, bool throwIfErrorMessage = true)
        {
            throw new NotImplementedException();
        }

        public T GetMessageResult<T>(ApiRequestMessage<T> request)
        {
            if (typeof(T) == typeof(int))
            {
                return (T)(object)(++_counter);
            }

            if (typeof(T) == typeof(List<int>))
            {
                return (T)(object)(new List<int>());
            }

            throw new NotImplementedException();
        }

        public bool GetBoolResult(ApiRequestMessage<int> request)
        {
            return true;
        }
    }
}
