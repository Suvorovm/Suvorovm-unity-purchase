using System;
using System.Collections.Generic;
using CGK.Snapshot.Model;

namespace PurchaseWrapper.Model
{
    [Serializable]
    public class PurchaseModel : ISnapshotModel
    {
        public List<string> Purchases = new List<string>();
        public int SnapshotVersion;
        public int GetSnapshotVersion()
        {
            return SnapshotVersion;
        }
    }
}