using UnityEngine.Purchasing;

namespace PurchaseWrapper.Service
{
    public interface IPurchaseAnalytics
    {
        void PurchaseCompleted(Product product);
    }
}