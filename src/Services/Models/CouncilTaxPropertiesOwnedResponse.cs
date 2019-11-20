using System.Collections.Generic;
using System.Xml.Serialization;
using StockportGovUK.NetStandard.Models.RevsAndBens;

namespace civica_service.Services.Models
{
    [XmlRoot("CtaxPropDetails")]
    public class CouncilTaxPropertyDetails
    {
        [XmlElement("CtxActRef")]
        public string AccountReference { get; set; }
        [XmlElement("PersonName")]
        public PersonName PersonName { get; set; }
        [XmlElement("PersonalDetails")]
        public PersonalDetails PersonalDetails { get; set; }
        [XmlElement("ExternalAddresses")]
        public string ExternalAddresses { get; set; }
        [XmlElement("PropertyList")]
        public PropertyList PropertyList { get; set; }
        [XmlElement("StandardInfo")]
        public StandardInfo StandardInfo { get; set; }
    }

    [XmlRoot("PropertyList")]
    public class PropertyList
    {
        [XmlElement("Places")]
        public List<Place> Places { get; set; }
    }
}
