using System.Collections.Generic;
using System.Xml.Serialization;
using StockportGovUK.NetStandard.Models.RevsAndBens;

namespace civica_service.Services.Models
{
    [XmlRoot("CtaxSelectDoc")]
    public class CtaxSelectDoc
    {
        [XmlElement("accbal")]
        public string AccountBalance { get; set; }
        [XmlElement("FormFieldValues")]
        public FormFieldValues FormFieldValues { get; set; }
        [XmlElement("PersonName")]
        public PersonName PersonName { get; set; }
        [XmlElement("PersonalDetails")]
        public PersonalDetails PersonalDetails { get; set; }
        [XmlElement("ExternalAddresses")]
        public string ExternalAddresses { get; set; }
        [XmlElement("CtaxActList")]
        public CounciltaxAccountList CounciltaxAccountList { get; set; }
        [XmlElement("StandardInfo")]
        public StandardInfo StandardInfo { get; set; }
        [XmlAttribute("Method")]
        public string Method { get; set; }
    }

    [XmlRoot("FormFieldValues")]
    public class FormFieldValues
    {
        [XmlElement("outputtype")]
        public string Outputtype { get; set; }
        [XmlElement("docid")]
        public string Docid { get; set; }
        [XmlElement("sessionId")]
        public string SessionId { get; set; }
    }

    [XmlRoot("CtaxActList")]
    public class CounciltaxAccountList
    {
        [XmlElement("CtaxActDetails")]
        public List<CouncilTaxAccountDetails> CouncilTaxAccounts { get; set; }
    }
}
