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
    public class XmlKeywordGroup
    {
        public XmlKeywordGroup(string name)
            : this()
        {
            Name = name;
        }

        protected XmlKeywordGroup()
        {
            Keywords = new List<XmlKeyword>();
        }

        [XmlAttribute(AttributeName="Name")]
        public string Name { get; set; }

        [XmlElement(ElementName="Keyword")]
        public List<XmlKeyword> Keywords { get; set; }
    }
}
