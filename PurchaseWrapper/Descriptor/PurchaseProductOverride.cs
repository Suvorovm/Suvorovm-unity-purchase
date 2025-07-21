using System.Xml.Serialization;
using PurchaseWrapper.Model;

namespace PurchaseWrapper.Descriptor
{
    [XmlRoot("override")]
    public class PurchaseProductOverride
    {
        [XmlAttribute("productId")]
        public string ProductId;

        [XmlAttribute("store")]
        public StoreType StoreType;
    }
}