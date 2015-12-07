using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topvisor.Api
{
    /// <summary>
    /// Интерфейс объекта Api умеющего валидироваться.
    /// </summary>
    internal interface IValidable : IApiObject
    {
        /// <summary>
        /// Проверить данные объекта.
        /// </summary>
        void Validate();
    }
}
