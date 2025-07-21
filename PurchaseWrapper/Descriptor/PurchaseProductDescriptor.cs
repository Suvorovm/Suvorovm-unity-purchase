using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using PurchaseWrapper.Model;

namespace PurchaseWrapper.Descriptor
{
    [Serializable]
    [XmlRoot("product")]
    public class PurchaseProductDescriptor
    {
        [XmlAttribute("productId")]
        public string ProductId;

        [XmlAttribute("productType")]
        public PurchaseProductType PurchaseProductType;
        
        [XmlElement("override")]
        public List<PurchaseProductOverride> Overrides;
    }
}