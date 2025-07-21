namespace PurchaseWrapper.Service
{
    public interface IRestorePurchase
    {
        void TryRestore(string productId);
    }
}