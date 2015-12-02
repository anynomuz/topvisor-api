using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopvisorApi
{
    /// <summary>
    /// Проект получаемый по Api. 
    /// </summary>
    public class ApiProject : IValidable
    {
        [DeserializeAs(Name = "id")]
        public int Id { get; set; }

        [DeserializeAs(Name = "user")]
        public int UserId { get; set; }

        [DeserializeAs(Name = "name")]
        public string Name { get; set; }

        [DeserializeAs(Name = "comment")]
        public string Comment { get; set; }

        [DeserializeAs(Name = "date")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Проверить данные объекта.
        /// </summary>
        public void Validate()
        {
            this.KeyIntAboveZero("id");
            this.KeyStringIsNoEmpty("name");
        }
    }
}
