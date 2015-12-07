using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topvisor.Api
{
    /// <summary>
    /// Фраза получаемая по Api.
    /// </summary>
    public class ApiKeyword : IValidable
    {
        [DeserializeAs(Name = "id")]
        public int Id { get; set; }

        [DeserializeAs(Name = "project_id")]
        public int ProjectId { get; set; }

        [DeserializeAs(Name = "group_id")]
        public int GroupId { get; set; }

        [DeserializeAs(Name = "group_name")]
        public string GroupName { get; set; }

        [DeserializeAs(Name = "phrase_id")]
        public int PhraseId { get; set; }

        [DeserializeAs(Name = "phrase")]
        public string Phrase { get; set; }

        [DeserializeAs(Name = "target")]
        public string Target { get; set; }

        /// <summary>
        /// Проверить данные объекта.
        /// </summary>
        public void Validate()
        {
            this.KeyIntAboveZero("id");
            this.KeyIntAboveZero("phrase_id");
            this.KeyIntAboveZero("project_id");
            this.KeyIntAboveZero("group_id");

            this.KeyStringIsNoEmpty("group_name");
            this.KeyStringIsNoEmpty("phrase");
        }
    }
}
