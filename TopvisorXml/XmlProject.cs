using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Topvisor.Xml
{
    public class XmlProject
    {
        public XmlProject(string name, string comment)
            : this()
        {
            Name = name;
            Comment = comment;
        }

        protected XmlProject()
        {
            KeywordGroups = new List<XmlKeywordGroup>(); 
        }

        [XmlAttribute(AttributeName="Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "Comment")]
        public string Comment { get; set; }

        [XmlArray("KeywordGroups")]
        [XmlArrayItem("Group")]
        public List<XmlKeywordGroup> KeywordGroups { get; set; }
    }
}
