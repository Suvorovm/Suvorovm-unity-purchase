using System;
using UnityEngine.Purchasing;

namespace PurchaseWrapper.Error
{
    public class PurchaseInitError : SystemException
    {
        public InitializationFailureReason Reason { get; private set; }

        public PurchaseInitError(InitializationFailureReason reason)
        {
            Reason = reason;
        }
        
        public PurchaseInitError(InitializationFailureReason reason, string message) : base(message)
        {
            Reason = reason;
        }
    }
}