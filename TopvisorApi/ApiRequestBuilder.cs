using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topvisor.Api
{
    public class ApiRequestBuilder
    {
        public ApiRequestBuilder()
        {
        }

        #region Проекты

        public IRestRequest GetProjectsRequest()
        {
            var request = new RestRequest(Method.GET);
            request.AddParameter("module", "mod_projects");
            request.AddParameter("oper", "get");

            return request;
        }

        public IRestRequest GetAddProjectRequest(string site, int on = 0)
        {
            var request = new RestRequest(Method.GET);
            request.AddParameter("module", "mod_projects");
            request.AddParameter("oper", "add");

            request.AddParameter("post[site]", site);
            request.AddParameter("post[on]", on);

            return request;
        }

        public IRestRequest GetDeleteProjectRequest(int id)
        {
            var request = new RestRequest(Method.GET);
            request.AddParameter("module", "mod_projects");
            request.AddParameter("oper", "del");

            request.AddParameter("post[id]", id);

            return request;
        }

        public IRestRequest GetUpdateProjectRequest(int id, int on)
        {
            var request = new RestRequest(Method.GET);
            request.AddParameter("module", "mod_projects");
            request.AddParameter("oper", "edit");

            request.AddParameter("post[id]", id);
            request.AddParameter("post[on]", on);

            return request;
        }

        #endregion

        #region Фразы

        public IRestRequest GetKeywordsRequest(
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

            return request;
        }

        public IRestRequest GetAddKeywordsRequest(
            int projectId, int groupId, IEnumerable<string> phrases)
        {
            var request = new RestRequest(Method.POST);
            request.AddParameter("module", "mod_keywords", ParameterType.QueryString);

            request.AddParameter("oper", "add", ParameterType.QueryString);
            request.AddParameter("method", "import", ParameterType.QueryString);

            request.AddParameter("project_id", projectId, ParameterType.GetOrPost);
            request.AddParameter("group_id", groupId, ParameterType.GetOrPost);

            request.AddParameter(
                "phrases", string.Join("|||", phrases), ParameterType.GetOrPost);

            return request;
        }

        public IRestRequest GetUpdateKeywordTargetRequest(int id, string url)
        {
            var request = new RestRequest(Method.GET);
            request.AddParameter("module", "mod_keywords");

            request.AddParameter("oper", "edit");
            request.AddParameter("post[id]", id);
            request.AddParameter("post[target]", url);

            return request;
        }

        public IRestRequest GetDeleteKeywordRequest(int id)
        {
            var request = new RestRequest(Method.GET);
            request.AddParameter("module", "mod_keywords");

            request.AddParameter("oper", "del");
            request.AddParameter("post[id]", id);

            return request;
        }

        #endregion

        #region Группы

        public IRestRequest GetAddKeywordGroupRequest(
            int projectId, string name, bool enabled = true)
        {
            var request = new RestRequest(Method.GET);
            request.AddParameter("module", "mod_keywords");
            request.AddParameter("oper", "add");

            request.AddParameter("method", "group");
            request.AddParameter("post[project_id]", projectId);
            request.AddParameter("post[name]", name);
            request.AddParameter("post[on]", enabled ? 1 : 0);

            return request;
        }

        public IRestRequest GetDeleteKeywordGroupRequest(
            int projectId, int groupId)
        {
            var request = new RestRequest(Method.GET);
            request.AddParameter("module", "mod_keywords");
            request.AddParameter("oper", "del");

            request.AddParameter("method", "group");
            request.AddParameter("post[project_id]", projectId);
            request.AddParameter("post[id]", groupId);

            return request;
        }

        public IRestRequest GetUpdateKeywordGroupRequest(
            int projectId, int groupId, bool enabled)
        {
            var request = new RestRequest(Method.GET);
            request.AddParameter("module", "mod_keywords");
            request.AddParameter("oper", "edit");

            request.AddParameter("method", "group");
            request.AddParameter("post[project_id]", projectId);
            request.AddParameter("post[id]", groupId);
            request.AddParameter("post[on]", enabled ? 1 : 0);

            return request;
        }

        #endregion
    }
}
