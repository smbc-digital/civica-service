using System.Xml.Serialization;
using StockportGovUK.NetStandard.Models.RevsAndBens;

namespace civica_service.Services.Models
{
    [XmlRoot("CtaxDetails")]
    public class CouncilTaxAccountSummary
    {
        [XmlElement("FinancialDetails")]
        public FinancialDetails FinancialDetails { get; set; }
    }

    [XmlRoot("FinancialDetails")]
    public class FinancialDetails
    {
        [XmlElement("RecYrTotals")]
        public RecievedYearTotal RecievedYearTotal { get; set; }
    }
}