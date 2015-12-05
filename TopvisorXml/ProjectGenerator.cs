using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topvisor.Xml;

namespace Topvisor.Xml
{
    /// <summary>
    /// Генератор xml-проектов.
    /// </summary>
    public class ProjectGenerator
    {
        private readonly Random _random;

        public ProjectGenerator()
        {
            _random = new Random(Environment.TickCount);
        }

        public IEnumerable<XmlProject> CreateProjects(int count, int maxPhrasesCount)
        {
            for (int i = 0; i < count; ++i)
            {
                yield return CreateProject(i, maxPhrasesCount);
            }
        }

        public XmlProject CreateProject(int id, int maxPhrasesCount)
        {
            var proj = new XmlProject(string.Format("http://project{0}.com", id));

            var group = new XmlKeywordGroup("defaultGroup");
            proj.KeywordGroups.Add(group);

            var baseWord = string.Format("proj{0}-{1}-{2}", id, group.Name, "keyword");
            var words = GetWords(baseWord, _random.Next(maxPhrasesCount));

            foreach (var word in words)
            {
                var url = (_random.Next(3) == 0)
                    ? string.Format("{0}/{1}", proj.Site, word)
                    : string.Empty;

                var xmlKeyword = new XmlKeyword(word, url);
                group.Keywords.Add(xmlKeyword);
            }

            return proj;
        }

        private static IEnumerable<string> GetWords(string baseWord, int count)
        {
            for (int i = 0; i < count; ++i)
            {
                yield return string.Concat(baseWord, i.ToString());
            }
        }
    }
}
