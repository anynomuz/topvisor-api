using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topvisor.Api;

namespace SyncAppConsole
{
    /// <summary>
    /// Группа фраз для задач синхронизации.
    /// </summary>
    internal class SyncKeywordGroup : IApiObject
    {
        private readonly List<ApiKeyword> _keywords;

        public SyncKeywordGroup(IEnumerable<ApiKeyword> keywords, bool enabled)
	    {
            var keyword = keywords.FirstOrDefault();

            if (keyword != null)
            {
                ProjectId = keyword.ProjectId;
                Id = keyword.GroupId;
                GroupName = keyword.GroupName;
            }

            Enabled = enabled;
            _keywords = new List<ApiKeyword>(keywords);
	    }

        public SyncKeywordGroup(int projectId, int groupId, string groupName, bool enabled)
        {
            ProjectId = projectId;
            Id = groupId;
            GroupName = groupName;
            Enabled = enabled;

            _keywords = new List<ApiKeyword>();
        }

        public int Id { get; set; }

        public bool Enabled { get; set; }

        public int ProjectId { get; private set; }

        public string GroupName { get; private set; }

        public IEnumerable<ApiKeyword> Keywords
        {
            get { return _keywords; }
        }

        public ApiKeyword AddKeyword(string phrase)
        {
            var keyword = new ApiKeyword()
                {
                    Id = -1,
                    ProjectId = this.ProjectId,
                    GroupId = this.Id,
                    GroupName = this.GroupName,
                    Phrase = phrase,
                };

            _keywords.Add(keyword);
            return keyword;
        }

        public bool RemoveKeyword(ApiKeyword keyword)
        {
            return _keywords.Remove(keyword);
        }
    }
}
