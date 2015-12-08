using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topvisor.Api
{
    /// <summary>
    /// Интерфейс объекта Api.
    /// </summary>
    public interface IApiObject
    {
        /// <summary>
        /// Id объекта для идентификации.
        /// </summary>
        int Id { get; set; }
    }
}
