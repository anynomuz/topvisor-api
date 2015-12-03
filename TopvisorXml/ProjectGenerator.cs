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
            var proj = new XmlProject("Project" + id);

            var group = new XmlKeywordGroup("DefaultGroup");
            proj.KeywordGroups.Add(group);

            var baseWord = string.Format("{0}-{1}-{2}", proj.Name, group.Name, "keyword");
            var words = GetWords(baseWord, _random.Next(maxPhrasesCount));

            foreach (var word in words)
            {
                group.Keywords.Add(new XmlKeyword(word, string.Empty));
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
