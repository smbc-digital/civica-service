using System.Xml.Serialization;

namespace civica_service.Helpers.SessionProvider.Models
{
    [XmlRoot("Login")]
    public class SessionIdModel
    {

        [XmlElement("Result")]
        public Result Result { get; set;}
    }
    
    public class Result
    {
        public ErrorCodeModel ErrorCode { get; set; }

        [XmlElement("SessionID")]
        public string SessionID { get; set; }
    }
}