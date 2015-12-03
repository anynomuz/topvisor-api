using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Topvisor.Xml
{
    [DebuggerDisplay("Phrase = {Phrase}, Url = {TargetUrl}")]
    public class XmlKeyword
    {
        public XmlKeyword(string phrase, string targetUrl)
        {
            Phrase = phrase;
            TargetUrl = targetUrl;
        }

        protected XmlKeyword()
        {
        }

        public string Phrase { get; set; }

        public string TargetUrl { get; set; }
    }
}
