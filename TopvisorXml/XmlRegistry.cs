using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Topvisor.Xml
{
    /// <summary>
    /// Реест проектов топвизора.
    /// </summary>
    [XmlType("ProjectRegistry")]
    public class XmlRegistry
    {
        private static XmlSerializer _serializer = new XmlSerializer(typeof(XmlRegistry));

        public XmlRegistry(IEnumerable<XmlProject> projects)
        {
            Projects = new List<XmlProject>(projects);
        }

        protected XmlRegistry()
        {
            Projects = new List<XmlProject>();
        }

        /// <summary>
        /// Список проектов.
        /// </summary>
        [XmlArray("Projects")]
        [XmlArrayItem("Project")]
        public List<XmlProject> Projects { get; set; }

        /// <summary>
        /// Загрузить реест проектов.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static XmlRegistry Load(Stream stream)
        {
            return (XmlRegistry)_serializer.Deserialize(stream);
        }

        /// <summary>
        /// Загрузить реестр проектов.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static XmlRegistry Load(string fileName)
        {
            using (var stream = new FileStream(
                fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Load(stream);
            }
        }

        /// <summary>
        /// Сохранить реестр проектов.
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            _serializer.Serialize(stream, this);
        }

        /// <summary>
        /// Сохранить реестр проектов.
        /// </summary>
        /// <param name="fileName"></param>
        public void Save(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                Save(stream);
            }
        }
    }
}
