using System.Xml.Serialization;

namespace civica_service.Helpers.SessionProvider.Models
{
    [XmlRoot("Login")]
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

    [XmlRoot("StandardInfo")]
    public class AnonymousSessionIdModel
    {
        [XmlElement("SessionID")]
        public string SessionID { get; set; }
    }

    public class AnonymousResult
    {
        [XmlElement("SessionID")]
        public string SessionID { get; set; }
    }
}