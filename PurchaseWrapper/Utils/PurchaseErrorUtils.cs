using System.Collections.Generic;
using UnityEngine.Purchasing;

namespace PurchaseWrapper.Utils
{
    public class PurchaseErrorUtils
    {
        private static readonly Dictionary<PurchaseFailureReason, string> PurchaseErrorMessage =
            new Dictionary<PurchaseFailureReason, string>()
            {
                { PurchaseFailureReason.Unknown, "somethings went wrong" },
                {
                    PurchaseFailureReason.DuplicateTransaction,
                    "The transaction has already been completed successfully"
                },
                { PurchaseFailureReason.PaymentDeclined, "There was a problem with the payment." },
                { PurchaseFailureReason.PurchasingUnavailable, "Purchase is unavailable." },
                { PurchaseFailureReason.ProductUnavailable, "The product is unavailable." },
                { PurchaseFailureReason.SignatureInvalid, "Signature validation error." },
                { PurchaseFailureReason.UserCancelled, "You canceled the purchase." },
                { PurchaseFailureReason.ExistingPurchasePending, "You have  already bought this product " }
            };

        public static string GetErrorMessage(PurchaseFailureReason purchaseFailureReason)
        {
            return PurchaseErrorMessage[purchaseFailureReason];
        }
    }
}