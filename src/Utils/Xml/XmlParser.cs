using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace civica_service.Utils.Xml
{
    public static class XmlParser
    {
        public static T DeserializeXmlStringToType<T>(string xmlElement, string nodeName)
        {
            XElement xml = XElement.Parse(xmlElement);

            return xml.DeserializeNodeToType<T>(nodeName);
        }

        public static T DeserializeNodeToType<T>(this XElement root, string nodeName)
        {
            var xmlSerializers = XmlSerializer.FromTypes(new[] { typeof(T) });

            return (T)xmlSerializers.First().Deserialize(root.DescendantsAndSelf(nodeName).First().CreateReader());
        }
    }
}