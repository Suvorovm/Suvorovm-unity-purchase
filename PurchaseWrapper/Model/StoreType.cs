using System;
using System.Xml.Serialization;

namespace PurchaseWrapper.Model
{
    [Serializable]
    public enum StoreType
    {
        [XmlEnum("google")]
        GooglePlay,
        
        [XmlEnum("appStore")]
        AppleStore
    }
}