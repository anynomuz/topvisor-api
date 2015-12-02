using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RestSharp.Deserializers;

namespace Topvisor.Api
{
    using BindingMap = Dictionary<string, PropertyInfo>;

    /// <summary>
    /// Вспомогательный класс валидации десериализованных объектов.
    /// </summary>
    internal static class DeserializedValidateHelper
    {
        private static Dictionary<Type, BindingMap> _typesBindingMap =
            new Dictionary<Type, BindingMap>();

        /// <summary>
        /// Значение типа int больше ноля.
        /// </summary>
        /// <param name="obj">Валидируемый объект.</param>
        /// <param name="key">Ключ валидируемого свойства.</param>
        public static void KeyIntAboveZero(this IValidable obj, string key)
        {
            var value = GetPropertyValue<int>(obj, key);

            if (value > 0)
            {
                var error = GetErrorMessage(obj, key, "Value <= 0");
                throw new ArgumentException(error);
            }
        }

        /// <summary>
        /// Значение типа string не пустое.
        /// </summary>
        /// <param name="obj">Валидируемый объект.</param>
        /// <param name="key">Ключ валидируемого свойства.</param>
        public static void KeyStringIsNoEmpty(this IValidable obj, string key)
        {
            var value = GetPropertyValue<string>(obj, key);

            if (string.IsNullOrWhiteSpace(value))
            {
                var error = GetErrorMessage(obj, key, "String is empty");
                throw new ArgumentException(error);
            }
        }

        /// <summary>
        /// Возвращает форматированную строку ошибки валидации объекта.
        /// </summary>
        /// <param name="obj">Валидируемый объект.</param>
        /// <param name="key">Ключ валидируемого свойства.</param>
        /// <param name="error">Сообщение об ошибке.</param>
        /// <returns>Форматированная строка сообщения.</returns>
        public static string GetErrorMessage(this IValidable obj, string key, string error)
        {
            return string.Format(
                "Object of type '{0}' with Id = '{1}', key '{2}' is invalid: {3}",
                obj.GetType(),
                obj.Id,
                key,
                error);
        }

        private static T GetPropertyValue<T>(object obj, string key)
        {
            var type = obj.GetType();
            var prop = GetPropertyInfo(type, key);

            if (prop.PropertyType != typeof(T))
            {
                var error = string.Format(
                    "Invalid type of key {0} for object '{1}'", key, type);

                throw new ArgumentException(error);
            }

            return (T)prop.GetValue(obj);
        }

        private static PropertyInfo GetPropertyInfo(Type type, string key)
        {
            var map = GetBingingMap(type);

            if (!map.ContainsKey(key.ToUpper()))
            {
                var error = string.Format("Validate key '{0}' not found", key);
                throw new InvalidOperationException(error);
            }

            return map[key.ToUpper()];
        }

        private static BindingMap GetBingingMap(Type type)
        {
            if (!_typesBindingMap.ContainsKey(type))
            {
                var map = CreateBingingMap(type);
                _typesBindingMap[type] = map;
                return map;
            }

            return _typesBindingMap[type];
        }

        private static BindingMap CreateBingingMap(Type type)
        {
            var map = new BindingMap();

            var properties = type.GetProperties();

            foreach (var prop in properties)
            {
                var attr = (DeserializeAsAttribute)prop.GetCustomAttribute(
                    typeof(DeserializeAsAttribute));

                if (attr != null)
                {
                    if (string.IsNullOrEmpty(attr.Name))
                    {
                        var error = string.Format(
                            "Attribute DeserializeAs Name is empty for type '{0}', property {1}",
                            type.FullName,
                            prop.Name);

                        throw new InvalidOperationException(error);
                    }

                    map.Add(attr.Name.ToUpper(), prop);
                }
            }

            return map;
        }
    }
}
