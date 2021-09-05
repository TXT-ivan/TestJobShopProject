using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static Shop;

public class Item : MonoBehaviour, ItemInterface
{
    #region Variables

    private Shop shop;

    [HideInInspector]
    public Dropdown dropdown_purchase_period;

    [SerializeField]
    private string _ScrinName;
    public string ScrinName
    {
        get { return _ScrinName; }
        set { _ScrinName = value; }
    }

    [SerializeField]
    private string _ItemName;
    public string ItemName
    {
        get { return _ItemName; }
        set { _ItemName = value; }
    }

    [SerializeField]
    private int _CoinsPrice;
    public int CoinsPrice
    {
        get { return _CoinsPrice; }
        set { _CoinsPrice = value; }
    }

    [SerializeField]
    private int _GemsPrice;
    public int GemsPrice
    {
        get { return _GemsPrice; }
        set { _GemsPrice = value; }
    }

    [SerializeField]
    private bool _IsBuy = false;
    public bool IsBuy
    {
        get { return _IsBuy; }
        set { _IsBuy = value; }
    }

    private string _ItemEndPerchaseDateTime = null;
    public string ItemEndPerchaseDateTime
    {
        get { return _ItemEndPerchaseDateTime; }
        set { _ItemEndPerchaseDateTime = value; }
    }

    private Dictionary<string, int> _ItemCurrencyCollection = new Dictionary<string, int>();

    public Dictionary<string, int> ItemCurrencyCollection
    {
        get { return _ItemCurrencyCollection; }
        set { _ItemCurrencyCollection = value; }
    }

    #endregion Variables

    // Start is called before the first frame update
    void Start()
    {
        shop = this.GetComponentInParent<Shop>();
        dropdown_purchase_period = this.GetComponentsInChildren<Dropdown>().First(x => x.name == "PurchasePeriod");
        CurrencyCheckExisting();

        this.GetComponentsInChildren<Text>().First(x => x.name == "TextNameItem").text = ScrinName;
        this.GetComponentsInChildren<Text>().First(x => x.name == "IsBuyTimer").text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if(shop != null && ItemEndPerchaseDateTime != null)
        {
            var response = shop.www_yandex.GetResponseHeaders();

            if (response != null)
            {
                response.TryGetValue("Date", out string str_date_time);
                DateTime.TryParse(str_date_time, out DateTime current_date_time);
                TimeSpan remaining_time = Convert.ToDateTime(ItemEndPerchaseDateTime) - current_date_time;

                this.GetComponentsInChildren<Text>().First(x => x.name == "IsBuyTimer").text = remaining_time.ToString();

                if(remaining_time.ToString() == "00:00:00" || remaining_time.TotalSeconds < 0)
                {
                    IsBuy = false;
                    this.GetComponentsInChildren<Text>().First(x => x.name == "IsBuyTimer").text = "";
                    this.GetComponentInParent<Shop>().DataCurrentPlayer.PurchasedItems.Remove(ItemName);
                }
            }
        }
    }

    // Функция покупки
    public void Buy(string currency, int value)
    {
        int? reduce_price = CheckPurchasePeriod(out string purchase_date_time, out int? purchase_period);

        if (!IsBuy && reduce_price != null)
        {
            this.GetComponentInParent<Shop>().DataCurrentPlayer.Currency.TryGetValue(currency, out int current_value);

            int buy_value = value / reduce_price.Value;

            if (current_value >= buy_value)
            {
                Dictionary<string, int?> date_time_dic = new Dictionary<string, int?>();
                date_time_dic.Add(purchase_date_time, purchase_period);

                current_value = current_value - buy_value;
                this.GetComponentInParent<Shop>().DataCurrentPlayer.Currency[currency] = current_value;
                this.GetComponentInParent<Shop>().DataCurrentPlayer.PurchasedItems.Add(ItemName, date_time_dic);

                IsBuy = true;
                this.GetComponentInChildren<Toggle>().isOn = true;

                if (purchase_period != null)
                {
                    ItemEndPerchaseDateTime = Convert.ToDateTime(purchase_date_time).AddHours(purchase_period.Value).ToString();
                }
            }

            shop.SaveGame();
        }
    }

    // Проверка наличия валюты в данных пользователя
    public void CurrencyCheckExisting()
    {
        shop.LoadGame();

        foreach (var current_currency_item in ItemCurrencyCollection)
        {
            bool exist_flag = false;

            foreach (var currency_item in this.GetComponentInParent<Shop>().DataCurrentPlayer.Currency)
            {
                if(current_currency_item.Key == currency_item.Key)
                {
                    exist_flag = true;
                }
            }

            if(!exist_flag)
            {
                this.GetComponentInParent<Shop>().DataCurrentPlayer.Currency.Add(current_currency_item.Key, current_currency_item.Value);
                shop.SaveGame();
            }
        }
    }

    // Проверка периода покупки
    public int? CheckPurchasePeriod(out string purchase_date_time, out int? purchase_period)
    {
        string period = dropdown_purchase_period.captionText.text;

        purchase_period = null;
        purchase_date_time = CheckGlobalTime();

        if (purchase_date_time == null)
        {
            return null;
        }

        switch (period)
        {
            case "Навсегда":
                return 1;
            case "На 1 час":
                purchase_period = 1;
                return 10;
            case "На 2 часа":
                purchase_period = 2;
                return 6;
        }

        return null;
    }

    // Получение глобального таймера
    public string CheckGlobalTime()
    {
        Dictionary<string, string> response = null;

        while(response == null)
        {
            response = shop.www_yandex.GetResponseHeaders();
            Thread.Sleep(1);
        }

        response.TryGetValue("Date", out string str_date_time);

        if (str_date_time != null)
        {
            DateTime.TryParse(str_date_time, out DateTime date_time);
            return date_time.ToString();
        }
        else
        {
            return null;
        }
    }
}
