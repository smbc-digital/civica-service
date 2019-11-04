using civica_service.Enums;
using civica_service.Utils.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace civica_service.Helpers.SessionProvider.Models
{
    [XmlRoot("TransactionList")]
    public class TransactionListModel : List<TransactionModel>, IXmlSerializer
    {
        public XmlSchema GetSchema()
        {
            return null;
        }
        public void ReadXml(XmlReader reader)
        {
            XElement node = (XElement)XElement.ReadFrom(reader);
            var transactions = node.Elements()
                .Where(e => e.Attribute("TranType") != null)
                .Select(e =>
                {
                    var typeName = e.Attribute("TranType").Value;
                    var type = GetTransactionType(typeName);
                    XmlSerializer serializer = XmlSerializer.FromTypes(new Type[] { type }).First();
                    TransactionModel transaction = (TransactionModel)serializer.Deserialize(e.CreateReader());
                    return transaction;
                });

            AddRange(transactions);
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        private static Type GetTransactionType(string attributeType)
        {
            string[] propertyBasedTranTypes =
            {
                "charge", "discount", "levy", "annexe", "disregard", "exemption",
                "reduction", "disabled"
            };

            if (propertyBasedTranTypes.Contains(attributeType.ToLower()))
            {
                return typeof(PropertyBasedTransactionResponse);
            }

            return attributeType.ToLower().Equals("payments") ? typeof(PaymentTransactionResponse) : typeof(TransactionModel);
        }

    }

    [XmlRoot("Transaction")]
    public class TransactionModel
    {

        protected decimal _amount;

        public string Date { get; set; }

        public DateTime TransactionDate
        {
            get { return DateTime.Parse(Date); }
        }

        public virtual decimal Amount
        {
            get { return _amount; }
            set { _amount = value; }
        }
      
        public string Type { get; set; }

        public string ChargeType { get; set; }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(null, other) || other.GetType() != this.GetType())
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            var transaction = (TransactionModel)other;
            return _amount == transaction._amount && string.Equals(Date, transaction.Date) &&
                   string.Equals(Type, transaction.Type) &&
                   string.Equals(ChargeType, transaction.ChargeType);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _amount.GetHashCode();
                hashCode = (hashCode * 397) ^ (Date != null ? Date.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Type != null ? Type.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ChargeType != null ? ChargeType.GetHashCode() : 0);
                return hashCode;
            }
        }
    }

    [XmlRoot("Transaction")]
    public class PaymentTransactionResponse : TransactionModel
    {
        public string SubCode { get; set; }

        public PaymentMethod Method
        {
            get { return PaymentMethodHelper.Convert(SubCode); }
        }

        public override decimal Amount
        {
            get { return 0 - _amount; }
        }
    }

    [XmlRoot("Transaction")]
    public class PropertyBasedTransactionResponse : TransactionModel
    {
        public PlaceDetailResponse PlaceDetail { get; set; }
    }

    public class PlaceDetailResponse
    {
        [XmlIgnore]
        public PostcodeName EnumType;

        [XmlChoiceIdentifier("EnumType")]
        [XmlElement("Postcode")]
        [XmlElement("PostCode")]
        public string PostCode
        {
            get { return _postcode != null ? _postcode.Trim() : _postcode; }
            set { _postcode = value; }
        }

        [XmlType(IncludeInSchema = false)]
        public enum PostcodeName
        {
            Postcode,
            PostCode
        }

        private string _postcode;
    }
}

