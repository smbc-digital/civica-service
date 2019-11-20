using System.Collections.Generic;
using System.Xml.Serialization;
using StockportGovUK.NetStandard.Models.RevsAndBens;

namespace civica_service.Services.Models
{
    [XmlRoot("TranList")]
    public class TransactionListModel
    {
        [XmlElement("Transaction")]
        public List<Transaction> Transaction { get; set; }
    }
}

