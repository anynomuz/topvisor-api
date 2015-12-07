using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topvisor.Api
{
    public interface IApiObject
    {
        /// <summary>
        /// Id объекта для идентификации.
        /// </summary>
        int Id { get; set; }
    }
}
