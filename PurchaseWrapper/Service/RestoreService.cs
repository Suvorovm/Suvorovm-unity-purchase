using System.Collections.Generic;

namespace PurchaseWrapper.Service
{
    public class RestoreService
    {
        private readonly IEnumerable<IRestorePurchase> _restorePurchasesService;

        public RestoreService(IEnumerable<IRestorePurchase> restorePurchasesService)
        {
            _restorePurchasesService = restorePurchasesService;
        }

        public void RestorePurchases(string productId)
        {
            foreach (IRestorePurchase restorePurchase in _restorePurchasesService)
            {
                restorePurchase.TryRestore(productId);
            }
        }
    }
}