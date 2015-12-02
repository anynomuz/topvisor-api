using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topvisor.Xml
{
    [Serializable]
    public class XmlProject
    {
        public XmlProject()
        {
            KeywordGroups = new List<XmlKeywordGroup>(); 
        }

        public string Name { get; set; }

        public string Comment { get; set; }

        public List<XmlKeywordGroup> KeywordGroups { get; set; }
    }
}
