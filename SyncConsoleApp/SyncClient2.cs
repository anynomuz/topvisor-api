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

        private readonly List<ApiKeyword> _apiKeywords =
            new List<ApiKeyword>();

        private readonly List<int> _enabledGroupsId =
            new List<int>();

        private readonly ApiRequestBuilder _requestBuilder;

        public IEnumerable<SyncRequest> AddProjects(IEnumerable<XmlProject> projects)
        {
            var createProjects = GetItemsForCreate(
                projects, _apiProjects, p => GetSiteKey(p.Site), p => GetSiteKey(p.Site))
                .ToList();

            var newProjects = createProjects.Select(
                p => new ApiProject() { Id = -1, Site = p.Site })
                .ToList();

            foreach (var proj in newProjects)
            {
                var request = _requestBuilder.GetAddProjectRequest(proj.Site);
                yield return GetAddRequest(request, _apiProjects, proj);
            }
        }

        public IEnumerable<SyncRequest> DeleteProjects(IEnumerable<XmlProject> projects)
        {
            var dropProjects = GetItemsForDelete(
                projects, _apiProjects, p => GetSiteKey(p.Site), p => GetSiteKey(p.Site))
                .ToList();

            foreach (var proj in dropProjects)
            {
                var request = _requestBuilder.GetDeleteProjectRequest(proj.Id);
                yield return GetDeleteRequest(request, _apiProjects, proj);
            }
        }

        public IEnumerable<SyncRequest> UpdateProjectsProperty(
            IEnumerable<XmlProject> projects)
        {
            var updateProjects = GetItemsForUpdate(
                projects, _apiProjects, p => GetSiteKey(p.Site), p => GetSiteKey(p.Site));

            foreach (var pair in updateProjects)
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
            }
        }

        public IEnumerable<SyncRequest> AddGroups()
        {

        }

        public IEnumerable<SyncRequest> DeleteGroups()
        {
        }

        public IEnumerable<SyncRequest> UpdateGroupsProperty()
        {
        }

        private static SyncRequest GetAddRequest<T>(
            IRestRequest request, IList<T> collection, T obj)
            where T : IApiObject
        {
            return new SyncRequest(
                SyncRequestType.Create,
                request,
                (id) =>
                {
                    obj.Id = id;
                    collection.Add(obj);
                });
        }

        private static SyncRequest GetDeleteRequest<T>(
            IRestRequest request, IList<T> collection, T obj)
        {
            return new SyncRequest(
                SyncRequestType.Delete,
                request,
                (id) =>
                {
                    if (id > 0)
                    {
                        collection.Remove(obj);
                    }
                });
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

        private static IEnumerable<Tuple<T1, T2>> GetItemsForUpdate<T1, T2, K>(
            IEnumerable<T1> newItems,
            IEnumerable<T2> oldItems,
            Func<T1, K> newKeySelector,
            Func<T2, K> oldKeySelector)
        {
            return newItems.Join(
                oldItems,
                t1 => newKeySelector(t1),
                t2 => oldKeySelector(t2),
                (t1, t2) => new Tuple<T1, T2>(t1, t2));
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
