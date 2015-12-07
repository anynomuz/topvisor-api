using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topvisor.Api;

namespace SyncConsoleApp
{
    public class SyncKeywordGroup : IApiObject
    {
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
            Keywords = new List<ApiKeyword>();
            Keywords.AddRange(keywords);
	    }

        public SyncKeywordGroup(int projectId, int groupId, string groupName, bool enabled)
        {
            ProjectId = projectId;
            Id = groupId;
            GroupName = groupName;
            Enabled = enabled;

            Keywords = new List<ApiKeyword>();
        }

        public int ProjectId { get; set; }

        public int Id { get; set; }

        public string GroupName { get; set; }

        public bool Enabled { get; set; }

        public List<ApiKeyword> Keywords { get; private set; }
    }
}
