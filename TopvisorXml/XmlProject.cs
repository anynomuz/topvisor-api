using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Topvisor.Xml
{
    [DebuggerDisplay("Site = {Site}")]
    public class XmlProject
    {
        public XmlProject(string name)
            : this()
        {
            Site = name;
        }

        protected XmlProject()
        {
            KeywordGroups = new List<XmlKeywordGroup>(); 
        }

        [XmlAttribute(AttributeName="Site")]
        public string Site { get; set; }

        [XmlArray("KeywordGroups")]
        [XmlArrayItem("Group")]
        public List<XmlKeywordGroup> KeywordGroups { get; set; }
    }
}
