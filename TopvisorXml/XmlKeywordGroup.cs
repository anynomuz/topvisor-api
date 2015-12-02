using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topvisor.Xml
{
    [Serializable]
    public class XmlKeywordGroup
    {
        public XmlKeywordGroup()
        {
            Keywords = new List<XmlKeyword>();
        }

        public string Name { get; set; }

        public List<XmlKeyword> Keywords { get; set; }
    }
}
