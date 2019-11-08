using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace civica_service.Helpers.SessionProvider.Models
{
    [XmlRoot(ElementName = "TranList")]
    public class TransactionListModel
    {
        [XmlElement(ElementName = "Transaction")]
        public List<Transaction> Transaction { get; set; }
    }

    [XmlRoot(ElementName = "Date")]
    public class Date
    {
        [XmlAttribute(AttributeName = "NumericEquiv")]
        public string NumericEquiv { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Period")]
    public class Period
    {
        [XmlAttribute(AttributeName = "End")]
        public string End { get; set; }
        [XmlAttribute(AttributeName = "Start")]
        public string Start { get; set; }
    }

    [XmlRoot(ElementName = "PlaceDetail")]
    public class PlaceDetail
    {
        [XmlElement(ElementName = "Valid")]
        public string Valid { get; set; }
        [XmlElement(ElementName = "PlaceRef")]
        public string PlaceRef { get; set; }
        [XmlElement(ElementName = "Address1")]
        public string Address1 { get; set; }
        [XmlElement(ElementName = "Address2")]
        public string Address2 { get; set; }
        [XmlElement(ElementName = "Address3")]
        public string Address3 { get; set; }
        [XmlElement(ElementName = "Address4")]
        public string Address4 { get; set; }
        [XmlElement(ElementName = "Address5")]
        public string Address5 { get; set; }
        [XmlElement(ElementName = "PostCode")]
        public string PostCode { get; set; }
    }

    [XmlRoot(ElementName = "Transaction")]
    public class Transaction
    {
        [XmlElement(ElementName = "Date")]
        public Date Date { get; set; }
        [XmlElement(ElementName = "Amount")]
        public string Amount { get; set; }
        [XmlElement(ElementName = "Claim")]
        public string Claim { get; set; }
        [XmlElement(ElementName = "Period")]
        public Period Period { get; set; }
        [XmlElement(ElementName = "PlaceDetail")]
        public PlaceDetail PlaceDetail { get; set; }
        [XmlAttribute(AttributeName = "Number")]
        public string Number { get; set; }
        [XmlAttribute(AttributeName = "TranType")]
        public string TranType { get; set; }
        [XmlAttribute(AttributeName = "Year")]
        public string Year { get; set; }
        [XmlElement(ElementName = "ChargeType")]
        public string ChargeType { get; set; }
        [XmlElement(ElementName = "SubCode")]
        public string SubCode { get; set; }
    }
}

