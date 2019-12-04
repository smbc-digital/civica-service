using System.Collections.Generic;
using System.Xml.Linq;

namespace civica_service.Utils.Xml
{
    public interface IXmlParser
    {
        T DeserializeXmlStringToType<T>(string xmlElement, string nodeName);

        T DeserializeNodeToType<T>(XElement root, string nodeName);

        IEnumerable<T> DeserializeDescendentsToIEnumerable<T>(string xmlElement, string descendentName);
    }
}