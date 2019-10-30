using System.Xml.Serialization;

namespace civica_service.Helpers.SessionProvider.Models
{
    [XmlRoot("SetPerson")]
    public class SetPersonModel
    {
        public ErrorCodeModel ErrorCode { get; set; }
    }
}