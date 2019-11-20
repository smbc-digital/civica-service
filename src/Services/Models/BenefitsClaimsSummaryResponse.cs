using System.Collections.Generic;
using System.Xml.Serialization;
using StockportGovUK.NetStandard.Models.RevsAndBens;

namespace civica_service.Services.Models
{
    [XmlRoot("HBSelectDoc")]
    public class BenefitsClaimsSummaryResponse
    {
        [XmlElement("HBClaimList")]
        public BenefitsClaimList Claims { get; set; }
    }

    public class BenefitsClaimList
    {
        [XmlElement("HBClaimDetails")]
        public List<BenefitsClaimSummary> Summary { get; set; }
    }
}