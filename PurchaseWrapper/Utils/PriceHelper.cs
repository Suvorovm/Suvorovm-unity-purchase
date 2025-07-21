using System.Globalization;
using UnityEngine.Purchasing;

namespace PurchaseWrapper.Utils
{
    public static class PriceHelper
    {
        public static decimal TryGetDecimalPriceOrDefault(this Product product)
        {
            if (product != null && product.metadata != null)
            {
                decimal price = product.metadata.localizedPrice;
                return price;
            }

            return 0;
        }

        public static string GetCurrencySymbolCode(this Product product)
        {
            string currencyCode = "$";
            if (product != null && product.metadata != null)
            {
                currencyCode = product.metadata.isoCurrencyCode;
                foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
                {
                    RegionInfo ri = new RegionInfo(ci.LCID);
                    if (ri.ISOCurrencySymbol == currencyCode)
                    {
                        return ri.CurrencySymbol;
                    }
                }
            }

            return currencyCode;
        }
    }
}