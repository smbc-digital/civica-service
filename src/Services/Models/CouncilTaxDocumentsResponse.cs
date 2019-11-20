using System.Xml.Serialization;
using StockportGovUK.NetStandard.Models.RevsAndBens;

namespace civica_service.Services.Models
{
    [XmlRoot("Documents")]
    public class CouncilTaxDocumentsResponse
    {
        [XmlArrayItem("Document")]
        public CouncilTaxDocument[] DocumentList { get; set; }
    }
}
