using System.Collections.Generic;
using System.Xml.Serialization;
using StockportGovUK.NetStandard.Models.RevsAndBens;

namespace civica_service.Services.Models
{
    [XmlRoot("HBPaymentDetails")]
    public class PaymentDetailsResponse
    {
        [XmlElement("PaymentList")]
        public PaymentDetailList PaymentList { get; set; }
    }

    [XmlRoot("PaymentList")]
    public class PaymentDetailList
    {
        [XmlElement("PaymentDetails")]
        public List<PaymentDetail> PaymentDetails { get; set; }
    }
}
