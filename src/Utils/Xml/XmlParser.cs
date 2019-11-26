using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace civica_service.Utils.Xml
{
    public class XmlParser : IXmlParser
    {
        public T DeserializeXmlStringToType<T>(string xmlElement, string nodeName) => DeserializeNodeToType<T>(XElement.Parse(xmlElement), nodeName);

        public T DeserializeNodeToType<T>(XElement root, string nodeName)
        {
            var xmlSerializers = XmlSerializer.FromTypes(new[] { typeof(T) });

            return (T)xmlSerializers.First().Deserialize(root.DescendantsAndSelf(nodeName).First().CreateReader());
        }
    }
}