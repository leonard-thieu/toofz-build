using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace toofz.Build
{
    internal static class XContainerExtensions
    {
        public static IEnumerable<XElement> ElementsByLocalName(this XContainer container, string localName)
        {
            return (from e in container.Elements()
                    where e.Name.LocalName == localName
                    select e)
                    .ToList();
        }
    }
}
