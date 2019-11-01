using System.Collections.Generic;
using System.Xml.Serialization;

namespace civica_service.Services.Models
{
    [XmlRoot("HBSelectDoc")]
    public class ClaimsSummaryResponse
    {
        [XmlElement("HBClaimList")]
        public ClaimListResponse ClaimsList { get; set; }
    }

    [XmlRoot("HBClaimList")]
    public class ClaimListResponse
    {
        [XmlElement("HBClaimDetails")]
        public List<ClaimSummaryResponse> ClaimSummary { get; set; }
    }

    [XmlRoot("HBClaimDetails")]
    public class ClaimSummaryResponse
    {
        [XmlAttribute("ClaimNumber")]
        public string ClaimNumber { get; set; }

        [XmlAttribute("ClaimPlaceRef")]
        public string ClaimPlaceRef { get; set; }

        [XmlAttribute("ClaimStatus")]
        public string ClaimStatus { get; set; }

        [XmlAttribute("PersonType")]
        public string PersonType { get; set; }

        [XmlElement("HBClaimAddress")]
        public string ClaimAddress { get; set; }
    }
}