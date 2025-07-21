namespace PurchaseWrapper.Service
{
    public interface IPurchaseDataHolder
    {
        public void SavePurchase(string data);

        public bool HasPurchase(string data);
    }
}