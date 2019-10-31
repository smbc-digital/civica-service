using System.Xml.Serialization;

namespace civica_service.Helpers.SessionProvider.Models
{
    public class SessionIdModel
    {
        [XmlElement("Result")]
        public Result Result { get; set; }

    }
    public class Result {
        [XmlElement("SessionID")]
        public string SessionID { get; set; }

        [XmlElement("ErrorCode")]
        public ErrorCode ErrorCode { get; set; }
    }
}