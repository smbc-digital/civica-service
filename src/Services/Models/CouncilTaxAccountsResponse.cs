using System.Collections.Generic;
using System.Xml.Serialization;

namespace civica_service.Services.Models
{
    [XmlRoot(ElementName = "FormFieldValues")]
    public class FormFieldValues
    {
        [XmlElement(ElementName = "outputtype")]
        public string Outputtype { get; set; }
        [XmlElement(ElementName = "docid")]
        public string Docid { get; set; }
        [XmlElement(ElementName = "sessionId")]
        public string SessionId { get; set; }
    }

    [XmlRoot(ElementName = "CtaxActDetails")]
    public class CtaxActDetails
    {
        [XmlElement(ElementName = "CtaxActAddress")]
        public string CtaxActAddress { get; set; }
        [XmlAttribute(AttributeName = "AccountStatus")]
        public string AccountStatus { get; set; }
        [XmlAttribute(AttributeName = "CtaxActRef")]
        public string CtaxActRef { get; set; }
        [XmlAttribute(AttributeName = "CtaxBalance")]
        public string CtaxBalance { get; set; }
    }

    [XmlRoot(ElementName = "CtaxActList")]
    public class CtaxActList
    {
        [XmlElement(ElementName = "CtaxActDetails")]
        public List<CtaxActDetails> CtaxActDetails { get; set; }
    }

    [XmlRoot(ElementName = "CtaxSelectDoc")]
    public class CtaxSelectDoc
    {
        [XmlElement(ElementName = "accbal")]
        public string Accbal { get; set; }
        [XmlElement(ElementName = "FormFieldValues")]
        public FormFieldValues FormFieldValues { get; set; }
        [XmlElement(ElementName = "PersonName")]
        public PersonName PersonName { get; set; }
        [XmlElement(ElementName = "PersonalDetails")]
        public PersonalDetails PersonalDetails { get; set; }
        [XmlElement(ElementName = "ExternalAddresses")]
        public string ExternalAddresses { get; set; }
        [XmlElement(ElementName = "CtaxActList")]
        public CtaxActList CtaxActList { get; set; }
        [XmlElement(ElementName = "StandardInfo")]
        public StandardInfo StandardInfo { get; set; }
        [XmlAttribute(AttributeName = "Method")]
        public string Method { get; set; }
    }
}
