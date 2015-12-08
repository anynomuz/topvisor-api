using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Topvisor.Api;

namespace SyncAppConsole
{
    /// <summary>
    /// Вспомогательные методы синхронизации.
    /// </summary>
    public class SyncHelper
    {
        /// <summary>
        /// Возвращает пары соответсвующих обеъктов для обновления.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="newItems"></param>
        /// <param name="oldItems"></param>
        /// <param name="newKeySelector"></param>
        /// <param name="oldKeySelector"></param>
        /// <returns></returns>
        public static IEnumerable<Tuple<T1, T2>> GetItemsForUpdate<T1, T2, K>(
            IEnumerable<T1> newItems,
            IEnumerable<T2> oldItems,
            Func<T1, K> newKeySelector,
            Func<T2, K> oldKeySelector)
        {
            return newItems.Join(
                oldItems,
                t1 => newKeySelector(t1),
                t2 => oldKeySelector(t2),
                (t1, t2) => new Tuple<T1, T2>(t1, t2));
        }

        /// <summary>
        /// Возвращает набор новых элементов,
        /// которые отсутсвуют в коллекции старых элементов.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="newItems"></param>
        /// <param name="oldItems"></param>
        /// <param name="newKeySelector"></param>
        /// <param name="oldKeySelector"></param>
        /// <returns></returns>
        public static IEnumerable<T1> GetItemsForCreate<T1, T2, K>(
            IEnumerable<T1> newItems,
            IEnumerable<T2> oldItems,
            Func<T1, K> newKeySelector,
            Func<T2, K> oldKeySelector)
        {
            return SubtractSets(newItems, oldItems, newKeySelector, oldKeySelector);
        }

        /// <summary>
        /// Возвращает набор старых элементов,
        /// которые отсутсвуют в коллекции новых элементов.
        /// </summary>
        public static IEnumerable<T2> GetItemsForDelete<T1, T2, K>(
            IEnumerable<T1> newItems,
            IEnumerable<T2> oldItems,
            Func<T1, K> newKeySelector,
            Func<T2, K> oldKeySelector)
        {
            return SubtractSets(oldItems, newItems, oldKeySelector, newKeySelector);
        }

        /// <summary>
        /// Вычитает из левого множества правое множество.
        /// </summary>
        /// <typeparam name="T1">Тип левого множества.</typeparam>
        /// <typeparam name="T2">Тип правого множества.</typeparam>
        /// <typeparam name="K">Тип ключа множеств.</typeparam>
        /// <param name="leftItems">Левое множество.</param>
        /// <param name="rigthItems">Правое множество.</param>
        /// <param name="leftKeySelector">Селектор ключа левого множества.</param>
        /// <param name="rigthKeySelector">Селектор ключа правого множества.</param>
        /// <returns>Разность множеств.</returns>
        private static IEnumerable<T1> SubtractSets<T1, T2, K>(
            IEnumerable<T1> leftItems,
            IEnumerable<T2> rigthItems,
            Func<T1, K> leftKeySelector,
            Func<T2, K> rigthKeySelector)
        {
            var dic = new Dictionary<K, T2>();

            foreach (var val in rigthItems)
            {
                dic[rigthKeySelector(val)] = val;
            }

            return leftItems.Where(i => !dic.ContainsKey(leftKeySelector(i)));
        }
    }
}
