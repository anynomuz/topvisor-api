using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopvisorApi
{
    /// <summary>
    /// Интерфейс объекта Api умеющего валидироваться.
    /// </summary>
    internal interface IValidable
    {
        /// <summary>
        /// Id объекта для идентификации.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Проверить данные объекта.
        /// </summary>
        void Validate();
    }
}
