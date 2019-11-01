using civica_service.Enums;
using civica_service.Helpers.SessionProvider.Models;

namespace civica_service.Helpers
{
    public static class PaymentMethodHelper
    { 
    public static PaymentMethod Convert(string paymentMethod)
    {
        if (string.IsNullOrEmpty(paymentMethod))
        {
            return PaymentMethod.Unknown;
        }

        switch (paymentMethod.Trim().ToUpper())
        {
            case "CASH":
            case "PP":
                return PaymentMethod.Cash;

            case "CCARD":
            case "DCARD":
                return PaymentMethod.DebitCreditCard;

            case "POCH":
                return PaymentMethod.Cheque;

            case "D/D CASH":
            case "DIRECTDEBIT":
                return PaymentMethod.DirectDebit;

            case "S/O":
            case "POTHER":
                return PaymentMethod.StandingOrder;

            case "DISCOUNT":
                return PaymentMethod.Discount;

            case "SUSPENSE":
                return PaymentMethod.Suspense;

            case "TRANSFER":
                return PaymentMethod.Transfer;

            case "OTHER":
                return PaymentMethod.Other;

            case "BAILIF":
                return PaymentMethod.Bailiff;

            default:
                return PaymentMethod.Unknown;
        }
    }

    public static bool IsRelevantRecentPaymentMethod(PaymentTransactionResponse payment)
    {
        return payment.Method == PaymentMethod.DirectDebit ||
               payment.Method == PaymentMethod.DebitCreditCard ||
               payment.Method == PaymentMethod.Cash ||
               payment.Method == PaymentMethod.Cheque ||
               payment.Method == PaymentMethod.StandingOrder;
    }
}
}
