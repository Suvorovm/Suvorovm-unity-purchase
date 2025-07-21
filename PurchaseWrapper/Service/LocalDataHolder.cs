using CGK.Snapshot.Model;
using CGK.Snapshot.Service;
using PurchaseWrapper.Model;

namespace PurchaseWrapper.Service
{
    public class LocalDataHolder : IPurchaseDataHolder, ISnapshotService
    {
        private PurchaseModel _purchaseModel;
        public void SavePurchase(string data)
        {
            _purchaseModel.Purchases.Add(data);
        }

        public bool HasPurchase(string data)
        {
            return _purchaseModel.Purchases.Exists(d => d == data);
        }

        public ISnapshotModel CreateSnapshotModel()
        {
            return _purchaseModel;
        }

        public void LoadSnapshotModel(ISnapshotModel snapshotModel)
        {
            _purchaseModel = (PurchaseModel) snapshotModel;
        }
    }
}