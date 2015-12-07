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

        private readonly List<int> _enabledGroupsId =
            new List<int>();

        public SyncClient(ClientConfig config)
        {
            _client = new ApiClient(config);
            _requestBuilder = new ApiRequestBuilder();
        }

        public void LoadApiObjects()
        {
#warning Как обрабатывать дубли проектов?

            var getProjects = _requestBuilder.GetProjectsRequest();
            var projects = _client.GetObjects<ApiProject>(getProjects);

            _apiProjects.AddRange(projects);

            var keywords = projects
                .Select(p => _requestBuilder.GetKeywordsRequest(p.Id, false))
                .SelectMany(p => _client.GetObjects<ApiKeyword>(p));

            _apiKeywords.AddRange(keywords);

            // не удалось найти признак вкл/выкл у группы, поэтому
            // таким образом собираем id'шники включеных групп,
            var enabledKeywords = projects
                .Select(p => _requestBuilder.GetKeywordsRequest(p.Id, true))
                .SelectMany(p => _client.GetObjects<ApiKeyword>(p).Select(k => k.GroupId))
                .Distinct();

            _enabledGroupsId.AddRange(enabledKeywords);
        }

        public void AddProjects(IEnumerable<XmlProject> projects)
        {
            var createProjects = GetItemsForCreate(
                projects, _apiProjects, p => GetSiteKey(p.Site), p => GetSiteKey(p.Site))
                .ToList();

            var newProjects = createProjects.Select(
                p => new ApiProject() { Id = -1, Site = p.Site })
                .ToList();

            // создаем новые проекты, нужны их Id чтобы дальше добавлять фразы
            foreach (var proj in newProjects)
            {
                var request = _requestBuilder.GetAddProjectRequest(proj.Site);
                proj.Id = _client.GetIdResponse(request);

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

            foreach (var proj in dropProjects)
            {
                var request = _requestBuilder.GetDeleteProjectRequest(proj.Id);
                proj.Id = _client.GetIdResponse(request);

                _apiProjects.Remove(proj);
            }
        }

        public void UpdateProjects(IEnumerable<XmlProject> projects)
        {
            var updateProjects = projects.Join(
                _apiProjects,
                p => GetSiteKey(p.Site),
                p => GetSiteKey(p.Site),
                (x, a) => new Tuple<XmlProject, ApiProject>(x, a));

            foreach (var pair in updateProjects)
            {
                UpdateProject(pair.Item1, pair.Item2);
            }
        }

        private void UpdateProject(XmlProject xmlProject, ApiProject apiProject)
        {
            // включить или выключить (перенести в архив) проект
            var stateOn = (xmlProject.Enabled) ? 0 : -1;

            if (stateOn != apiProject.On)
            {
                var request = _requestBuilder.GetUpdateProjectRequest(
                    apiProject.Id, stateOn);

                _client.GetBoolResponse(request);
                apiProject.On = stateOn;
            }

            // синхронизировать группы
            var apiGroups = _apiKeywords.Where(w => w.ProjectId == apiProject.Id)
                .GroupBy(g => g.GroupName)
                .Select(group => (ApiKeywordGroup)group.First())
                .ToList();

            UpdateGroups(apiProject, xmlProject.KeywordGroups, apiGroups);
        }

        private void UpdateGroups(
            ApiProject project,
            IEnumerable<XmlKeywordGroup> xmlGroups,
            IEnumerable<ApiKeywordGroup> apiGroups)
        {
            // создать группы
            var createGroups = GetItemsForCreate(
                xmlGroups,
                apiGroups,
                g => GetGroupNameKey(g.Name),
                g => GetGroupNameKey(g.GroupName));

            foreach (var xmlGroup in createGroups)
            {
                var request = _requestBuilder.GetAddKeywordGroupRequest(
                    project.Id, xmlGroup.Name);

                var groupId = _client.GetIdResponse(request);

                var apiGroup = new ApiKeywordGroup()
                    {
                        ProjectId = project.Id,
                        GroupId = groupId,
                        GroupName = xmlGroup.Name,
                    };

                _enabledGroupsId.Add(groupId);

                UpdateGroup(xmlGroup, apiGroup);
            }

            // удалить группы
            var dropGroups = GetItemsForDelete(
                xmlGroups,
                apiGroups,
                g => GetGroupNameKey(g.Name),
                g => GetGroupNameKey(g.GroupName));

            foreach (var group in dropGroups)
            {
                var request = _requestBuilder.GetDeleteKeywordGroupRequest(
                    project.Id, group.GroupId);

                _client.GetBoolResponse(request);
                _enabledGroupsId.Remove(group.GroupId);
            }

            // обновить группы
            var updateGroups = xmlGroups.Join(
                apiGroups,
                g => GetGroupNameKey(g.Name),
                g => GetGroupNameKey(g.GroupName),
                (x, a) => new Tuple<XmlKeywordGroup, ApiKeywordGroup>(x, a));

            foreach (var pair in updateGroups)
            {
                UpdateGroup(pair.Item1, pair.Item2);
            }
        }

        private void UpdateGroup(XmlKeywordGroup xmlGroup, ApiKeywordGroup apiGroup)
        {
            var apiGroupEnabled = _enabledGroupsId.Contains(apiGroup.GroupId);

            // включить / выключить группы
            if (apiGroupEnabled != xmlGroup.Enabled)
            {
                var request = _requestBuilder.GetUpdateKeywordGroupRequest(
                    apiGroup.ProjectId, apiGroup.GroupId, xmlGroup.Enabled);

                _client.GetBoolResponse(request);

                if (xmlGroup.Enabled)
                {
                    _enabledGroupsId.Add(apiGroup.GroupId);
                }
                else
                {
                    _enabledGroupsId.Remove(apiGroup.GroupId);
                }
            }

            var apiKeywords = _apiKeywords.Where(
                w => (w.ProjectId == apiGroup.ProjectId) && (w.GroupId == apiGroup.GroupId))
                .ToList();

            // добавить фразы
            var createWords = GetItemsForCreate(
                xmlGroup.Keywords, apiKeywords, w => w.Phrase, w => w.Phrase);

            var addKeywords = createWords
                .Where(p => !string.IsNullOrEmpty(p.Phrase)).Select(p => p.Phrase);

            var addRequest = _requestBuilder.GetAddKeywordsRequest(
                apiGroup.ProjectId, apiGroup.GroupId, addKeywords);

            _client.GetResponseResult<List<int>>(addRequest);

            // TODO: Установить target-страницу

            // удалить фразы
            var dropWords = GetItemsForDelete(
                xmlGroup.Keywords, apiKeywords, w => w.Phrase, w => w.Phrase);

            foreach (var word in dropWords)
            {
                var dropRequest = _requestBuilder.GetDeleteKeywordRequest(word.Id);
                _client.GetBoolResponse(dropRequest);
            }
        }

        private static string GetSiteKey(string site)
        {
            return site.Trim().TrimEnd('/', '.').ToUpper()
                .Replace("HTTP://", "").Replace("HTTPS://", "");
        }

        private static string GetGroupNameKey(string groupName)
        {
            return groupName.Trim().ToUpper();
        }

        private static IEnumerable<T1> GetItemsForCreate<T1, T2, K>(
            IEnumerable<T1> newItems,
            IEnumerable<T2> oldItems,
            Func<T1, K> newKeySelector,
            Func<T2, K> oldKeySelector)
        {
            return SubtractSets(
                newItems, oldItems, newKeySelector, oldKeySelector);
        }

        private static IEnumerable<T2> GetItemsForDelete<T1, T2, K>(
            IEnumerable<T1> newItems,
            IEnumerable<T2> oldItems,
            Func<T1, K> newKeySelector,
            Func<T2, K> oldKeySelector)
        {
            return SubtractSets(
                oldItems, newItems, oldKeySelector, newKeySelector);
        }

        /// <summary>
        /// Вычитает из левого множества правое множество.
        /// </summary>
        /// <typeparam name="T1">Тип левого множества.</typeparam>
        /// <typeparam name="T2">Тип правого множества.</typeparam>
        /// <typeparam name="K">Тип ключа множеств.</typeparam>
        /// <param name="leftItems">Левое множество.</param>
        /// <param name="rigthItems">Правое множество.</param>
        /// <param name="leftKeySelector">Селектор ключа левого множества.</param>
        /// <param name="rigthKeySelector">Селектор ключа правого множества.</param>
        /// <returns>Разность множеств.</returns>
        private static IEnumerable<T1> SubtractSets<T1, T2, K>(
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
