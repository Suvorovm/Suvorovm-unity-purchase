using System.Collections.Generic;
using System.Xml.Serialization;

namespace PurchaseWrapper.Descriptor
{
    [XmlRoot("purchaseConfig")]
    public class PurchaseProductConfig
    {
        [XmlAttribute("fake")]
        public bool Fake;
        
        [XmlElement("product")]
        public List<PurchaseProductDescriptor> Products;
    }
}