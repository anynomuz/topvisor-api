using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topvisor.Api
{
    /// <summary>
    /// Построитель запросов к Api.
    /// </summary>
    /// <remarks>
    /// Форматы запросов в документации: https://topvisor.ru/api
    /// </remarks>
    public class ApiRequestBuilder
    {
        public ApiRequestBuilder()
        {
        }

        #region Проекты

        public ApiRequest<IEnumerable<ApiProject>> GetProjectsRequest()
        {
            var request = new RestRequest(Method.GET);
            request.AddParameter("module", "mod_projects");
            request.AddParameter("oper", "get");

            return new ApiRequest<IEnumerable<ApiProject>>(request);
        }

        public ApiRequestMessage<int> GetAddProjectRequest(string site, int on = 0)
        {
            var request = new RestRequest(Method.GET);
            request.AddParameter("module", "mod_projects");
            request.AddParameter("oper", "add");

            request.AddParameter("post[site]", site);
            request.AddParameter("post[on]", on);

            return new ApiRequestMessage<int>(request);
        }

        public ApiRequestMessage<int> GetDeleteProjectRequest(int id)
        {
            var request = new RestRequest(Method.GET);
            request.AddParameter("module", "mod_projects");
            request.AddParameter("oper", "del");

            request.AddParameter("post[id]", id);

            return new ApiRequestMessage<int>(request);
        }

        public ApiRequestMessage<int> GetUpdateProjectRequest(int id, int on)
        {
            var request = new RestRequest(Method.GET);
            request.AddParameter("module", "mod_projects");
            request.AddParameter("oper", "edit");

            request.AddParameter("post[id]", id);
            request.AddParameter("post[on]", on);

            return new ApiRequestMessage<int>(request);
        }

        #endregion

        #region Фразы

        public ApiRequest<IEnumerable<ApiKeyword>> GetKeywordsRequest(
            int projectId, bool onlyEnabled, int groupId = -1)
        {
            var request = new RestRequest(Method.GET);
            request.AddParameter("module", "mod_keywords");

            request.AddParameter("oper", "get");
            request.AddParameter("post[project_id]", projectId);

            if (onlyEnabled)
            {
                request.AddParameter("post[only_enabled]", 1);
            }

            if (groupId > 0)
            {
                request.AddParameter("post[group_id]", groupId);
            }

            return new ApiRequest<IEnumerable<ApiKeyword>>(request);
        }

        public ApiRequestMessage<int> GetAddKeywordRequest(
            int projectId, string phrase, int groupId = -1)
        {
            var request = new RestRequest(Method.GET);
            request.AddParameter("module", "mod_keywords");

            request.AddParameter("oper", "add");
            request.AddParameter("post[project_id]", projectId);

            if (groupId > 0)
            {
                request.AddParameter("post[group_id]", groupId);
            }

            request.AddParameter("post[phrase]", phrase);

            return new ApiRequestMessage<int>(request);
        }

        public ApiRequestMessage<List<int>> GetAddKeywordsRequest(
            int projectId, IEnumerable<string> phrases, int groupId = -1)
        {
            var request = new RestRequest(Method.POST);
            request.AddParameter("module", "mod_keywords", ParameterType.QueryString);

            request.AddParameter("oper", "add", ParameterType.QueryString);
            request.AddParameter("method", "import", ParameterType.QueryString);

            request.AddParameter("project_id", projectId, ParameterType.GetOrPost);

            if (groupId > 0)
            {
                request.AddParameter("group_id", groupId, ParameterType.GetOrPost);
            }

            request.AddParameter(
                "phrases", string.Join("|||", phrases), ParameterType.GetOrPost);

            return new ApiRequestMessage<List<int>>(request);
        }

        public ApiRequestMessage<int> GetUpdateKeywordTargetRequest(int id, string url)
        {
            var request = new RestRequest(Method.GET);
            request.AddParameter("module", "mod_keywords");

            request.AddParameter("oper", "edit");
            request.AddParameter("post[id]", id);
            request.AddParameter("post[target]", url);

            return new ApiRequestMessage<int>(request);
        }

        public ApiRequestMessage<int> GetDeleteKeywordRequest(int id)
        {
            var request = new RestRequest(Method.GET);
            request.AddParameter("module", "mod_keywords");

            request.AddParameter("oper", "del");
            request.AddParameter("post[id]", id);

            return new ApiRequestMessage<int>(request);
        }

        #endregion

        #region Группы

        public ApiRequestMessage<int> GetAddKeywordGroupRequest(
            int projectId, string name, bool enabled = true)
        {
            var request = new RestRequest(Method.GET);
            request.AddParameter("module", "mod_keywords");
            request.AddParameter("oper", "add");

            request.AddParameter("method", "group");
            request.AddParameter("post[project_id]", projectId);
            request.AddParameter("post[name]", name);
            request.AddParameter("post[on]", enabled ? 1 : 0);

            return new ApiRequestMessage<int>(request);
        }

        public ApiRequestMessage<int> GetDeleteKeywordGroupRequest(
            int projectId, int groupId)
        {
            var request = new RestRequest(Method.GET);
            request.AddParameter("module", "mod_keywords");
            request.AddParameter("oper", "del");

            request.AddParameter("method", "group");
            request.AddParameter("post[project_id]", projectId);
            request.AddParameter("post[id]", groupId);

            return new ApiRequestMessage<int>(request);
        }

        public ApiRequestMessage<int> GetUpdateKeywordGroupRequest(
            int projectId, int groupId, bool enabled)
        {
            var request = new RestRequest(Method.GET);
            request.AddParameter("module", "mod_keywords");
            request.AddParameter("oper", "edit");

            request.AddParameter("method", "group");
            request.AddParameter("post[project_id]", projectId);
            request.AddParameter("post[id]", groupId);
            request.AddParameter("post[on]", enabled ? 1 : 0);

            return new ApiRequestMessage<int>(request);
        }

        #endregion
    }
}
