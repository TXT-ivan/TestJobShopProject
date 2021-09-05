using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace Assets.Scripts
{
    interface ItemInterface
    {
        void CurrencyCheckExisting();
        void Buy(string currency, int value);
        int? CheckPurchasePeriod(out string purchase_date_time, out int? purchase_period);
        string CheckGlobalTime();
        string ItemEndPerchaseDateTime { get; set; }
        string ScrinName { get; set; }
        string ItemName { get; set; }
        int CoinsPrice { get; set; }
        int GemsPrice { get; set; }
        bool IsBuy { get; set; }
        Dictionary<string, int> ItemCurrencyCollection { get; set; }
    }
}
