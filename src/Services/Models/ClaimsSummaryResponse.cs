using System.Collections.Generic;
using System.Xml.Serialization;

namespace civica_service.Services.Models
{
    [XmlRoot("HBSelectDoc")]
    public class ClaimsSummaryResponse
    {
        [XmlElement("HBClaimList")]
        public ClaimList Claims { get; set; }
    }

    public class ClaimList
    {
        [XmlElement("HBClaimDetails")]
        public List<ClaimSummary> Summary { get; set; }
    }

    public class ClaimSummary
    {
        [XmlAttribute("ClaimNumber")]
        public string Number { get; set; }

        [XmlAttribute("ClaimPlaceRef")]
        public string PlaceRef { get; set; }

        [XmlAttribute("ClaimStatus")]
        public string Status { get; set; }

        [XmlAttribute("PersonType")]
        public string PersonType { get; set; }

        [XmlElement("HBClaimAddress")]
        public string Address { get; set; }
    }
}