using System;
using UnityEngine.Purchasing;

namespace PurchaseWrapper.Error
{
    public class PurchaseProcessionError : SystemException
    {
        public string ProductId { get; private set; }

        public PurchaseFailureReason Reason { get; private set; }

        public PurchaseProcessionError(string productId, PurchaseFailureReason reason)
        {
            ProductId = productId;
            Reason = reason;
        }
    }
}