using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Topvisor.Xml
{
    [DebuggerDisplay("Name = {Name}")]
    public class XmlProject
    {
        public XmlProject(string name)
            : this()
        {
            Name = name;
        }

        protected XmlProject()
        {
            KeywordGroups = new List<XmlKeywordGroup>(); 
        }

        [XmlAttribute(AttributeName="Name")]
        public string Name { get; set; }

        [XmlArray("KeywordGroups")]
        [XmlArrayItem("Group")]
        public List<XmlKeywordGroup> KeywordGroups { get; set; }
    }
}
