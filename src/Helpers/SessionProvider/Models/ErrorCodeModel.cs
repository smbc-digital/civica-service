using System.Xml.Serialization;

namespace civica_service.Helpers.SessionProvider.Models
{
    public class ErrorCode {
        [XmlAttribute("Description")]
        public string Description { get; set; }

        [XmlText]
        public string Text { get; set; }
    }
}