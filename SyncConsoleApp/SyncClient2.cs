using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topvisor.Api;
using Topvisor.Xml;

namespace SyncConsoleApp
{
    public class SyncClient2
    {
        private readonly List<ApiProject> _apiProjects =
            new List<ApiProject>();

        private readonly List<SyncKeywordGroup> _apiGroups =
            new List<SyncKeywordGroup>();

        private readonly ApiRequestBuilder _requestBuilder;

        public SyncClient2(ClientConfig config)
        {
            _requestBuilder = new ApiRequestBuilder();
        }

        public void LoadApiObjects(ApiClient client)
        {
            _apiProjects.Clear();
            _apiGroups.Clear();

#warning Как обрабатывать дубли проектов?

            var getProjects = _requestBuilder.GetProjectsRequest();
            var projects = client.GetObjects<ApiProject>(getProjects);

            _apiProjects.AddRange(projects);

            var keywords = projects
                .Select(p => _requestBuilder.GetKeywordsRequest(p.Id, false))
                .SelectMany(p => client.GetObjects<ApiKeyword>(p))
                .ToList();

            // не удалось найти признак вкл/выкл у группы, поэтому
            // дополнительным запросом собираем id'шники включеных групп
            var enabledKeywords = projects
                .Select(p => _requestBuilder.GetKeywordsRequest(p.Id, true))
                .SelectMany(p => client.GetObjects<ApiKeyword>(p).Select(k => k.GroupId))
                .Distinct()
                .ToList();

            var groups = keywords
                .GroupBy(k => k.GroupId)
                .Select(g => new SyncKeywordGroup(g, enabledKeywords.Contains(g.Key)));

            _apiGroups.AddRange(groups);
        }

        #region Синхронизация проектов

        public void SyncProjects(IEnumerable<XmlProject> projects, ApiClient client)
        {
            var list = new List<SyncRequest>();

            list.AddRange(AddProjects(projects));
            list.AddRange(DeleteProjects(projects));
            list.AddRange(UpdateProjectsProperties(projects));

            foreach (var rq in list)
            {
                rq.ExecRequest(client);
            }
        }

        public IEnumerable<SyncRequest> AddProjects(IEnumerable<XmlProject> projects)
        {
            var createProjects = SyncHelper.GetItemsForCreate(
                projects, _apiProjects, p => GetSiteKey(p.Site), p => GetSiteKey(p.Site))
                .ToList();

            foreach (var proj in createProjects)
            {
                var newProject = new ApiProject() { Id = -1, Site = proj.Site };

                var request = _requestBuilder.GetAddProjectRequest(proj.Site);
                yield return SyncHelper.GetAddRequest(request, _apiProjects, newProject);
            }
        }

        public IEnumerable<SyncRequest> DeleteProjects(IEnumerable<XmlProject> projects)
        {
            var dropProjects = SyncHelper.GetItemsForDelete(
                projects, _apiProjects, p => GetSiteKey(p.Site), p => GetSiteKey(p.Site))
                .ToList();

            foreach (var proj in dropProjects)
            {
                var request = _requestBuilder.GetDeleteProjectRequest(proj.Id);
                yield return SyncHelper.GetDeleteRequest(request, _apiProjects, proj);
            }
        }

        public IEnumerable<SyncRequest> UpdateProjectsProperties(IEnumerable<XmlProject> projects)
        {
            var updateProjects = SyncHelper.GetItemsForUpdate(
                projects, _apiProjects, p => GetSiteKey(p.Site), p => GetSiteKey(p.Site));

            foreach (var pair in updateProjects)
            {
                // включить или выключить (перенести в архив) проект
                var stateOn = (pair.Item1.Enabled) ? 0 : -1;

                var apiProject = pair.Item2;

                if (stateOn != apiProject.On)
                {
                    var request = _requestBuilder.GetUpdateProjectRequest(
                        apiProject.Id, stateOn);

                    yield return new SyncRequest(
                        SyncRequestType.Update,
                        request,
                        (id) =>
                        {
                            if (id > 0)
                            {
                                apiProject.On = stateOn;
                            }
                        }
                    );
                }
            }
        }

        #endregion

        #region Синхронизация групп

        public IEnumerable<SyncRequest> AddGroups(IEnumerable<XmlProject> projects)
        {
            var syncGroups = GetSyncProjectGroups(projects);

            foreach (var sync in syncGroups)
            {
                var createGroups = SyncHelper.GetItemsForCreate(
                    sync.XmlObjects,
                    sync.ApiObjects,
                    g => GetGroupNameKey(g.Name),
                    g => GetGroupNameKey(g.GroupName));

                foreach (var xmlGroup in createGroups)
                {
                    var apiGroup = new SyncKeywordGroup(
                        sync.ParentId, -1, xmlGroup.Name, xmlGroup.Enabled);

                    var request = _requestBuilder.GetAddKeywordGroupRequest(
                        sync.ParentId, xmlGroup.Name, xmlGroup.Enabled);

                    yield return SyncHelper.GetAddRequest(request, _apiGroups, apiGroup);
                }
            }
        }

        public IEnumerable<SyncRequest> DeleteGroups(IEnumerable<XmlProject> projects)
        {
            var syncGroups = GetSyncProjectGroups(projects);

            foreach (var sync in syncGroups)
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

                    yield return SyncHelper.GetDeleteRequest(request, _apiGroups, group);
                }
            }
        }

        public IEnumerable<SyncRequest> UpdateGroupsProperties(IEnumerable<XmlProject> projects)
        {
            var syncGroupPair = GetSyncGroupsPair(projects);

            foreach (var pair in syncGroupPair)
            {
                var xmlGroup = pair.Item1;
                var apiGroup = pair.Item2;

                // включить / выключить группы
                if (apiGroup.Enabled != xmlGroup.Enabled)
                {
                    var request = _requestBuilder.GetUpdateKeywordGroupRequest(
                        apiGroup.ProjectId, apiGroup.Id, xmlGroup.Enabled);

                    yield return new SyncRequest(
                        SyncRequestType.Update,
                        request,
                        (id) =>
                        {
                            if (id > 0)
                            {
                                apiGroup.Enabled = xmlGroup.Enabled;
                            }
                        });
                }
            }
        }

        #endregion

        #region Синхронизация фраз

        public IEnumerable<SyncRequest> AddKeywords(IEnumerable<XmlProject> projects)
        {
            var syncGroupPair = GetSyncGroupsPair(projects);

            foreach (var groupPair in syncGroupPair)
            {
                var xmlGroup = groupPair.Item1;
                var apiGroup = groupPair.Item2;

                // добавить фразы
                var createWords = SyncHelper.GetItemsForCreate(
                    xmlGroup.Keywords, apiGroup.Keywords, w => w.Phrase, w => w.Phrase);

                var addKeywords = createWords
                    .Where(p => !string.IsNullOrEmpty(p.Phrase)).Select(p => p.Phrase);

                var addRequest = _requestBuilder.GetAddKeywordsRequest(
                    apiGroup.ProjectId, apiGroup.Id, addKeywords);

                yield return SyncHelper.GetAddRequest(request, );

                _client.GetResponseResult<List<int>>(addRequest);

                // TODO: Установить target-страницу
            }
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
            return site.Trim().TrimEnd('/', '.').ToUpper()
                .Replace("HTTP://", "").Replace("HTTPS://", "");
        }

        private static string GetGroupNameKey(string groupName)
        {
            return groupName.Trim().ToUpper();
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
