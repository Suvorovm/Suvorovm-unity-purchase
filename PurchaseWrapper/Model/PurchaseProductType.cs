using System;
using System.Xml.Serialization;

namespace PurchaseWrapper.Model
{
    [Serializable]
    public enum PurchaseProductType
    {
        [XmlEnum("consumable")]
        Consumable,
        
        [XmlEnum("nonConsumable")]
        NonConsumable,
        
        [XmlEnum("subscription")]
        Subscription
    }
}