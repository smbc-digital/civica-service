using System.Xml.Serialization;

namespace civica_service_tests.Utils
{
    [XmlRoot("Root")]
    public class XmlParserModel
    {
        [XmlElement("Element")]
        public string XmlParserTest { get; set; }
    }
}