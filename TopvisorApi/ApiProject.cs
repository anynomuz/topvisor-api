using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topvisor.Api
{
    /// <summary>
    /// Проект получаемый по Api. 
    /// </summary>
    [DebuggerDisplay("Id = {Id}, Site = {Site}, On = {On}")]
    public class ApiProject : IValidable
    {
        [DeserializeAs(Name = "id")]
        public int Id { get; set; }

        [DeserializeAs(Name = "name")]
        public string Name { get; set; }

        [DeserializeAs(Name = "site")]
        public string Site { get; set; }

        [DeserializeAs(Name = "comment")]
        public string Comment { get; set; }

        [DeserializeAs(Name = "on")]
        public int On { get; set; }

        /// <summary>
        /// Проверить данные объекта.
        /// </summary>
        public void Validate()
        {
            this.KeyIntAboveZero("id");
            this.KeyStringIsNoEmpty("name");
            this.KeyStringIsNoEmpty("site");
        }
    }
}
