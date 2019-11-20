using System.Collections.Generic;
using System.Xml.Serialization;
using StockportGovUK.NetStandard.Models.RevsAndBens;

namespace civica_service.Services.Models
{
    [XmlRoot("CtaxDetails")]
    public class CouncilTaxAccountResponse
    {
        [XmlElement("AccountDetails")]
        public AccountDetail AccountDetails { get; set; }

        [XmlElement("FinancialDetails")]
        public FinancialDetailsResponse FinancialDetails { get; set; }

        [XmlElement("CtxActRef")]
        public string CouncilTaxAccountReference { get; set; }

        [XmlElement("CtxActBalance")]
        public decimal CouncilTaxAccountBalance { get; set; }

        [XmlElement("CtxActClosed")]
        public string CtxActClosed { get; set; }
    }

    [XmlRoot("FinancialDetails")]
    public class FinancialDetailsResponse
    {
        [XmlElement("RecYrTotals")]
        public List<YearTotalResponse> YearTotals { get; set; }
    }

    [XmlRoot("RecYrTotals")]
    public class YearTotalResponse
    {
        [XmlAttribute("Year")]
        public int TaxYear { get; set; }

        [XmlElement("RecYrSummary")]
        public List<YearSummaryResponse> YearSummaries { get; set; }

        public decimal? InstArrears { get; set; }

        public decimal BalanceOutstanding { get; set; }

        public decimal TotalCharge { get; set; }

        public decimal TotalPayments { get; set; }

        public decimal TotalBenefits { get; set; }

        public decimal TotalCosts { get; set; }

        public decimal TotalRefunds { get; set; }

        public decimal TotalWriteoffs { get; set; }

        public decimal TotalTransfers { get; set; }

        public decimal TotalPenalties { get; set; }

        public int? SparNo { get; set; }
    }

    [XmlRoot("RecYrSummary")]
    public class YearSummaryResponse
    {
        public StageResponse Stage { get; set; }

        public PaymentSummaryResponse NextPayment { get; set; }
    }

    [XmlRoot("Stage")]
    public class StageResponse
    {
        [XmlAttribute("StageCode")]
        public string StageCode { get; set; }

        [XmlAttribute("StageDate")]
        public string StageDate { get; set; }
    }

    [XmlRoot("NextPayment")]
    public class PaymentSummaryResponse
    {
        [XmlAttribute("DateDue")]
        public string NextPaymentDate { get; set; }

        [XmlAttribute("AmountDue")]
        public decimal NextPaymentAmount { get; set; }
    }

    [XmlRoot("IRInstalments")]
    public class PaymentScheduleResponse
    {
        public List<Instalment> Instalments { get; set; }

        public string PaymentMethod { get; set; }
    }

    [XmlRoot("AccountDetails")]
    public class AccountDetail
    {
        public ActPayGrp ActPayGrp { get; set; }

        public BankAccountDetailsResponse BankDetails { get; set; }
    }

    [XmlRoot("ActPayGrp")]
    public class ActPayGrp
    {
        [XmlText]
        public string PaymentMethod { get; set; }

        [XmlAttribute("DirectDebit")]
        public string DirectDebit { get; set; }

        public bool IsDirectDebit()
        {
            return DirectDebit != null && DirectDebit.ToLower().Equals("yes");
        }
    }

    [XmlRoot("BankDetails")]
    public class BankAccountDetailsResponse
    {
        private string _accountNumber;

        [XmlAttribute("BankActName")]
        public string AccountName { get; set; }

        [XmlAttribute("BankActSortNumber")]
        public string SortCode { get; set; }

        [XmlAttribute("BankActNumber")]
        public string AccountNumber
        {
            get
            {
                return _accountNumber.Substring(_accountNumber.Length - 4);
            }

            set
            {
                _accountNumber = value;
            }
        }
    }
}
