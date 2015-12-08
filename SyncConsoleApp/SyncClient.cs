using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topvisor.Api;
using Topvisor.Xml;

namespace SyncAppConsole
{
    /// <summary>
    /// Клиент для выполнения синхронизации.
    /// </summary>
    public class SyncClient
    {
        private readonly List<ApiProject> _apiProjects =
            new List<ApiProject>();

        private readonly List<SyncKeywordGroup> _apiGroups =
            new List<SyncKeywordGroup>();

        private readonly IApiClient _client;
        private readonly ApiRequestBuilder _requestBuilder;

        public SyncClient(IApiClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            _client = client;
            _requestBuilder = new ApiRequestBuilder();
        }

        /// <summary>
        /// Загружает синхронизируемые объекты.
        /// </summary>
        public void LoadSyncObjects()
        {
            _apiProjects.Clear();
            _apiGroups.Clear();

#warning Как обрабатывать дубли проектов?

            var getProjects = _requestBuilder.GetProjectsRequest();
            var projects = _client.GetResponseObjects<ApiProject>(getProjects);

            _apiProjects.AddRange(projects);

            var keywords = projects
                .Select(p => _requestBuilder.GetKeywordsRequest(p.Id, false))
                .SelectMany(p => _client.GetResponseObjects<ApiKeyword>(p))
                .ToList();

            // не удалось найти признак вкл/выкл у группы, поэтому
            // дополнительным запросом собираем id'шники включеных групп
            var enabledKeywords = projects
                .Select(p => _requestBuilder.GetKeywordsRequest(p.Id, true))
                .SelectMany(p => _client.GetResponseObjects<ApiKeyword>(p).Select(k => k.GroupId))
                .Distinct()
                .ToList();

            var groups = keywords
                .GroupBy(k => k.GroupId)
                .Select(g => new SyncKeywordGroup(g, enabledKeywords.Contains(g.Key)));

            _apiGroups.AddRange(groups);
        }

        #region Синхронизация проектов

        /// <summary>
        /// Выполняет синхронизацию списка проектов
        /// и свойств относящихся непосредственно к проектам.
        /// </summary>
        /// <param name="projects"></param>
        /// <returns></returns>
        public int SyncProjects(IEnumerable<XmlProject> projects)
        {
            int counter = 0;

            counter += AddProjects(projects);
            counter += UpdateProjectsProperties(projects);
            counter += DeleteProjects(projects);

            return counter;
        }

        /// <summary>
        /// Добавляет недостающие проекты.
        /// </summary>
        /// <param name="projects"></param>
        /// <returns></returns>
        public int AddProjects(IEnumerable<XmlProject> projects)
        {
            var createProjects = SyncHelper.GetItemsForCreate(
                projects, _apiProjects, p => GetSiteKey(p.Site), p => GetSiteKey(p.Site))
                .ToList();

            foreach (var proj in createProjects)
            {
                var request = _requestBuilder.GetAddProjectRequest(proj.Site);
                var projectId = _client.GetIntResponse(request);

                var newProject = new ApiProject() { Id = projectId, Site = proj.Site, On = 0 };
                _apiProjects.Add(newProject);
            }

            return createProjects.Count;
        }

        /// <summary>
        /// Удаляет лишние проекты.
        /// </summary>
        /// <param name="projects"></param>
        /// <returns></returns>
        public int DeleteProjects(IEnumerable<XmlProject> projects)
        {
            var dropProjects = SyncHelper.GetItemsForDelete(
                projects, _apiProjects, p => GetSiteKey(p.Site), p => GetSiteKey(p.Site))
                .ToList();

            int counter = 0;

            foreach (var proj in dropProjects)
            {
                var request = _requestBuilder.GetDeleteProjectRequest(proj.Id);
                var res = _client.GetBoolResponse(request);

                if (res)
                {
                    _apiProjects.Remove(proj);
                    ++counter;
                }
            }

            return counter;
        }

        /// <summary>
        /// Обновляет свойства проектов.
        /// </summary>
        /// <param name="projects"></param>
        /// <returns></returns>
        public int UpdateProjectsProperties(IEnumerable<XmlProject> projects)
        {
            var updateProjects = SyncHelper.GetItemsForUpdate(
                projects, _apiProjects, p => GetSiteKey(p.Site), p => GetSiteKey(p.Site));

            int counter = 0;

            foreach (var pair in updateProjects)
            {
                // включить или выключить (перенести в архив) проект
                var stateOn = (pair.Item1.Enabled) ? 0 : -1;

                var apiProject = pair.Item2;

                if (stateOn != apiProject.On)
                {
                    var request = _requestBuilder.GetUpdateProjectRequest(
                        apiProject.Id, stateOn);

                    var res = _client.GetBoolResponse(request);

                    if (res)
                    {
                        apiProject.On = stateOn;
                        ++counter;
                    }
                }
            }

            return counter;
        }

        #endregion

        #region Синхронизация групп

        /// <summary>
        /// Выполняет синхронизацию групп проектов
        /// и свойств относящихся непосредственно к группам.
        /// </summary>
        /// <param name="projects"></param>
        /// <returns></returns>
        public int SyncGroups(IEnumerable<XmlProject> projects)
        {
            int counter = 0;

            counter += AddGroups(projects);
            counter += UpdateGroupsProperties(projects);
            counter += DeleteGroups(projects);

            return counter;
        }

        /// <summary>
        /// Добавляет недостающие проекты.
        /// </summary>
        /// <param name="projects"></param>
        /// <returns></returns>
        public int AddGroups(IEnumerable<XmlProject> projects)
        {
            var syncProjects = GetSyncProjectGroups(projects);

            int counter = 0;

            foreach (var proj in syncProjects)
            {
                var createGroups = SyncHelper.GetItemsForCreate(
                    proj.XmlObjects,
                    proj.ApiObjects,
                    g => GetGroupNameKey(g.Name),
                    g => GetGroupNameKey(g.GroupName));

                foreach (var group in createGroups)
                {
                    var request = _requestBuilder.GetAddKeywordGroupRequest(
                        proj.ParentId, group.Name, group.Enabled);

                    var groupId = _client.GetIntResponse(request);

                    var newGroup = new SyncKeywordGroup(
                        proj.ParentId, groupId, group.Name, group.Enabled);

                    _apiGroups.Add(newGroup);
                    ++counter;
                }
            }

            return counter;
        }

        /// <summary>
        /// Удаляет лишние проекты.
        /// </summary>
        /// <param name="projects"></param>
        /// <returns></returns>
        public int DeleteGroups(IEnumerable<XmlProject> projects)
        {
            var syncProjects = GetSyncProjectGroups(projects);

            int counter = 0;

            foreach (var sync in syncProjects)
            {
                var dropGroups = SyncHelper.GetItemsForDelete(
                    sync.XmlObjects,
                    sync.ApiObjects,
                    g => GetGroupNameKey(g.Name),
                    g => GetGroupNameKey(g.GroupName));

                foreach (var group in dropGroups)
                {
                    var request = _requestBuilder.GetDeleteKeywordGroupRequest(
                        sync.ParentId, group.Id);

                    var res = _client.GetBoolResponse(request);

                    if (res)
                    {
                        _apiGroups.Remove(group);
                        ++counter;
                    }
                }
            }

            return counter;
        }

        /// <summary>
        /// Обновляет свойства проектов.
        /// </summary>
        /// <param name="projects"></param>
        /// <returns></returns>
        public int UpdateGroupsProperties(IEnumerable<XmlProject> projects)
        {
            var syncGroupPair = GetSyncGroupsPair(projects);

            int counter = 0;

            foreach (var pair in syncGroupPair)
            {
                var xmlGroup = pair.Item1;
                var apiGroup = pair.Item2;

                // включить / выключить группы
                if (apiGroup.Enabled != xmlGroup.Enabled)
                {
                    var request = _requestBuilder.GetUpdateKeywordGroupRequest(
                        apiGroup.ProjectId, apiGroup.Id, xmlGroup.Enabled);

                    var res = _client.GetBoolResponse(request);

                    if (res)
                    {
                        apiGroup.Enabled = xmlGroup.Enabled;
                        ++counter;
                    }
                }
            }

            return counter;
        }

        #endregion

        #region Синхронизация фраз

        /// <summary>
        /// Выполняет синхронизацию фраз
        /// и свойств относящихся непосредственно к фразам.
        /// </summary>
        /// <param name="projects"></param>
        /// <returns></returns>
        public int SyncKeywords(IEnumerable<XmlProject> projects)
        {
            int counter = 0;

            counter += AddKeywords(projects);
            counter += UpdateKeywordsProperties(projects);
            counter += DeleteKeywords(projects);

            return counter;
        }

        /// <summary>
        /// Добавляет недостающие фразы.
        /// </summary>
        /// <param name="projects"></param>
        /// <returns></returns>
        public int AddKeywords(IEnumerable<XmlProject> projects)
        {
            var syncGroupPair = GetSyncGroupsPair(projects);

            int counter = 0;

            foreach (var groupPair in syncGroupPair)
            {
                var apiGroup = groupPair.Item2;

                var createWords = SyncHelper.GetItemsForCreate(
                    groupPair.Item1.Keywords.Where(k => !string.IsNullOrEmpty(k.Phrase)),
                    apiGroup.Keywords,
                    w => GetPhraseKey(w.Phrase),
                    w => GetPhraseKey(w.Phrase));

                // пакетное добавление фраз без таргета
                var addKeywords = createWords
                    .Where(k => string.IsNullOrEmpty(k.TargetUrl))
                    .ToList();

                if (addKeywords.Count > 0)
                {
                    var request = _requestBuilder.GetAddKeywordsRequest(
                        apiGroup.ProjectId, addKeywords.Select(p => p.Phrase), apiGroup.Id);

                    var res = _client.GetResponseResult<List<int>>(request);

                    foreach (var word in addKeywords)
                    {
                        apiGroup.AddKeyword(word.Phrase);
                    }

                    counter += addKeywords.Count;
                }

                // добавление по одной фразе с таргетом, чтобы получать их id
                addKeywords = createWords
                    .Where(k => !string.IsNullOrEmpty(k.TargetUrl))
                    .ToList();

                foreach (var word in addKeywords)
                {
                    var request = _requestBuilder.GetAddKeywordRequest(
                        apiGroup.ProjectId, word.Phrase, apiGroup.Id);

                    var phraseId = _client.GetIntResponse(request);
                    var keyword = apiGroup.AddKeyword(word.Phrase);
                    keyword.Id = phraseId;

                    ++counter;
                }
            }

            return counter;
        }

        /// <summary>
        /// Удаляет лишние ключевые фразы.
        /// </summary>
        /// <param name="projects"></param>
        /// <returns></returns>
        public int DeleteKeywords(IEnumerable<XmlProject> projects)
        {
            var syncGroupPair = GetSyncGroupsPair(projects);

            int counter = 0;

            foreach (var groupPair in syncGroupPair)
            {
                var dropWords = SyncHelper.GetItemsForDelete(
                        groupPair.Item1.Keywords,
                        groupPair.Item2.Keywords,
                        w => GetPhraseKey(w.Phrase),
                        w => GetPhraseKey(w.Phrase))
                    .ToList();

                foreach (var word in dropWords)
                {
                    var request = _requestBuilder.GetDeleteKeywordRequest(word.Id);
                    var res = _client.GetBoolResponse(request);

                    if (res)
                    {
                        groupPair.Item2.RemoveKeyword(word);
                        ++counter;
                    }
                }
            }

            return counter;
        }

        /// <summary>
        /// Обновляет свойства ключевых фраз.
        /// </summary>
        /// <param name="projects"></param>
        /// <returns></returns>

        public int UpdateKeywordsProperties(IEnumerable<XmlProject> projects)
        {
            var syncGroupPair = GetSyncGroupsPair(projects);

            int counter = 0;

            foreach (var groupPair in syncGroupPair)
            {
                var pairs = SyncHelper.GetItemsForUpdate(
                    groupPair.Item1.Keywords,
                    groupPair.Item2.Keywords,
                    w => GetPhraseKey(w.Phrase),
                    w => GetPhraseKey(w.Phrase));

                foreach (var wordPair in pairs)
                {
                    var apiKeyword = wordPair.Item2;

                    if (GetSiteKey(wordPair.Item1.TargetUrl) != GetSiteKey(apiKeyword.Target))
                    {
                        var request = _requestBuilder.GetUpdateKeywordTargetRequest(
                            apiKeyword.Id, wordPair.Item1.TargetUrl);

                        var res = _client.GetBoolResponse(request);

                        if (res)
                        {
                            apiKeyword.Target = wordPair.Item1.TargetUrl;
                            ++counter;
                        }
                    }
                }
            }

            return counter;
        }

        #endregion

        private IEnumerable<Tuple<XmlKeywordGroup, SyncKeywordGroup>> GetSyncGroupsPair(
            IEnumerable<XmlProject> projects)
        {
            var syncGroups = GetSyncProjectGroups(projects);

            foreach (var sync in syncGroups)
            {
                var updateGroups = SyncHelper.GetItemsForUpdate(
                    sync.XmlObjects,
                    sync.ApiObjects,
                    g => GetGroupNameKey(g.Name),
                    g => GetGroupNameKey(g.GroupName));

                foreach (var pair in updateGroups)
                {
                    yield return pair;
                }
            }
        }
        
        private IEnumerable<SyncObject<XmlKeywordGroup, SyncKeywordGroup>> GetSyncProjectGroups(
            IEnumerable<XmlProject> projects)
        {
            var updateProjects = SyncHelper.GetItemsForUpdate(
                projects, _apiProjects, p => GetSiteKey(p.Site), p => GetSiteKey(p.Site));

            foreach (var pair in updateProjects)
            {
                var xmlProject = pair.Item1;
                var apiProject = pair.Item2;

                var projectGroups = _apiGroups
                    .Where(g => g.ProjectId == apiProject.Id)
                    .ToList();

                yield return new SyncObject<XmlKeywordGroup, SyncKeywordGroup>(
                    pair.Item2.Id, pair.Item1.KeywordGroups, projectGroups);
            }
        }

        private static string GetSiteKey(string site)
        {
            if (string.IsNullOrEmpty(site))
            {
                return string.Empty;
            }

            return site.Trim().TrimEnd('/', '.').ToUpper()
                .Replace("HTTP://", "").Replace("HTTPS://", "");
        }

        private static string GetGroupNameKey(string groupName)
        {
            return groupName.Trim().ToUpper();
        }

        private static string GetPhraseKey(string phrase)
        {
            return phrase.Trim().ToUpper();
        }

        private class SyncObject<T1, T2>
        {
            public SyncObject(
                int projectId, IEnumerable<T1> xmlObjects, IEnumerable<T2> apiObjects)
	        {
                ParentId = projectId;
                XmlObjects = xmlObjects;
                ApiObjects = apiObjects;
	        }

            public SyncObject(
                int projectId, int groupId, IEnumerable<T1> xmlObjects, IEnumerable<T2> apiObjects)
            {
                ParentId = projectId;
                XmlObjects = xmlObjects;
                ApiObjects = apiObjects;
            }

            public int ParentId { get; private set; }

            public IEnumerable<T1> XmlObjects { get; private set; }
        
            public IEnumerable<T2> ApiObjects { get; private set; }
        }
    }
}
