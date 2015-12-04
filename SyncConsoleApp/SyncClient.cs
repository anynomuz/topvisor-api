using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topvisor.Api;
using Topvisor.Xml;

namespace SyncConsoleApp
{
    public class SyncClient
    {
        private readonly ApiClient _client;
        private readonly ApiRequestBuilder _requestBuilder;

        private readonly List<ApiProject> _apiProjects =
            new List<ApiProject>();

        private readonly List<ApiKeyword> _apiKeywords =
            new List<ApiKeyword>();

        public SyncClient(ClientConfig config)
        {
            _client = new ApiClient(config);
            _requestBuilder = new ApiRequestBuilder();
        }

        public void LoadState()
        {
            var getProjects = _requestBuilder.GetProjectsRequest();
            var projects = _client.GetObjects<ApiProject>(getProjects);

#warning Брать все фразы или только включеные?

            var keywords = projects
                .Select(p => _requestBuilder.GetKeywordsRequest(p.Id, false))
                .SelectMany(p => _client.GetObjects<ApiKeyword>(p));

            _apiProjects.AddRange(projects);
            _apiKeywords.AddRange(keywords);
        }

        public void AddProjects(IEnumerable<XmlProject> projects)
        {
            var createProjects = GetItemsForCreate(
                projects, _apiProjects, p => GetSiteKey(p.Site), p => GetSiteKey(p.Site))
                .ToList();

            var newProjects = createProjects.Select(
                p => new ApiProject() { Id = -1, Site = p.Site })
                .ToList();

            // создаем новые проекты
            foreach (var proj in newProjects)
            {
                var request = _requestBuilder.GetAddProjectRequest(proj.Site);
                proj.Id = _client.ExecIdQuery(request);

                _apiProjects.Add(proj);
            }

            // синхронизируем содержимое проектов
            for (int i = 0; i < createProjects.Count; ++i)
            {
                UpdateProject(createProjects[i], newProjects[i]);
            }
        }

        public void DeleteProjects(IEnumerable<XmlProject> projects)
        {
            var dropProjects = GetItemsForDelete(
                projects, _apiProjects, p => GetSiteKey(p.Site), p => GetSiteKey(p.Site))
                .ToList();

#warning Удалять или отключать проекты?

            foreach (var proj in dropProjects)
            {
                var request = _requestBuilder.GetDeleteProjectRequest(proj.Id);
                proj.Id = _client.ExecIdQuery(request);

                _apiProjects.Remove(proj);
            }
        }

        public void UpdateProjects(IEnumerable<XmlProject> projects)
        {
            ////projects.Where(p => GetSiteKey(p.Site)
        }

        private void UpdateProject(XmlProject xmlProject, ApiProject apiProject)
        {
            // синхронизировать группы

            // синхронизировать фразы
        }

        private static string GetSiteKey(string site)
        {
            return site.Trim().ToUpper();
        }

        private static IEnumerable<T1> GetItemsForCreate<T1, T2, K>(
            IEnumerable<T1> newItems,
            IEnumerable<T2> oldItems,
            Func<T1, K> newKeySelector,
            Func<T2, K> oldKeySelector)
        {
            return GetLeftItemsSubRigthItems(
                newItems, oldItems, newKeySelector, oldKeySelector);
        }

        private static IEnumerable<T2> GetItemsForDelete<T1, T2, K>(
            IEnumerable<T1> newItems,
            IEnumerable<T2> oldItems,
            Func<T1, K> newKeySelector,
            Func<T2, K> oldKeySelector)
        {
            return GetLeftItemsSubRigthItems(
                oldItems, newItems, oldKeySelector, newKeySelector);
        }

        private static IEnumerable<T1> GetLeftItemsSubRigthItems<T1, T2, K>(
            IEnumerable<T1> leftItems,
            IEnumerable<T2> rigthItems,
            Func<T1, K> leftKeySelector,
            Func<T2, K> rigthKeySelector)
        {
            var dic = new Dictionary<K, T2>();

            foreach (var val in rigthItems)
            {
                dic[rigthKeySelector(val)] = val;
            }

            return leftItems.Where(i => !dic.ContainsKey(leftKeySelector(i)));
        }
    }
}
