using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp.Deserializers;

namespace Topvisor.Api
{

    public class ApiKeywordGroup
    {
        [DeserializeAs(Name = "project_id")]
        public int ProjectId { get; set; }

        [DeserializeAs(Name = "group_id")]
        public int GroupId { get; set; }

        [DeserializeAs(Name = "group_name")]
        public string GroupName { get; set; }
    }
}
