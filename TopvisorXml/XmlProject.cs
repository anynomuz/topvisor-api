using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Topvisor.Xml
{
    [DebuggerDisplay("Site = {Site}, Enabled = {Enabled}")]
    public class XmlProject
    {
        public XmlProject(string site, bool enabled = true)
            : this()
        {
            Site = site;
            Enabled = enabled;
        }

        protected XmlProject()
        {
            KeywordGroups = new List<XmlKeywordGroup>(); 
        }

        /// <summary>
        /// Сайт проекта.
        /// </summary>
        public string Site { get; set; }

        /// <summary>
        /// Включен / выключен.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Группы фраз.
        /// </summary>
        [XmlArray("KeywordGroups")]
        [XmlArrayItem("Group")]
        public List<XmlKeywordGroup> KeywordGroups { get; set; }
    }
}
