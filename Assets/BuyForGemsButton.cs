using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyForGemsButton : MonoBehaviour, BuyButtonInterface
{
    private Item _item;
    private string _item_dropdown_text;

    private string _CurrencyName = "Gems";
    public string CurrencyName
    {
        get { return _CurrencyName; }
        set { _CurrencyName = value; }
    }

    private int _CurrencyDefaultValue = 300;
    public int CurrencyDefaultValue
    {
        get { return _CurrencyDefaultValue; }
        set { _CurrencyDefaultValue = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(ButtonOnClick);
        _item = this.GetComponentInParent<Item>();
        if (!_item.ItemCurrencyCollection.ContainsKey(CurrencyName))
        {
            _item.ItemCurrencyCollection.Add(CurrencyName, CurrencyDefaultValue);
        }
        this.GetComponentInChildren<Text>().text = _item.GemsPrice.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        string purchase_period = _item.dropdown_purchase_period.captionText.text;

        if (_item.dropdown_purchase_period != null && _item_dropdown_text != purchase_period)
        {
            switch (purchase_period)
            {
                case "Навсегда":
                    this.GetComponentInChildren<Text>().text = (_item.GemsPrice / 1).ToString();
                    break;
                case "На 1 час":
                    this.GetComponentInChildren<Text>().text = (_item.GemsPrice / 10).ToString();
                    break;
                case "На 2 часа":
                    this.GetComponentInChildren<Text>().text = (_item.GemsPrice / 6).ToString();
                    break;
            }

            _item_dropdown_text = purchase_period;
        }

        if(_item.IsBuy == true)
        {
            this.GetComponent<Button>().interactable = false;
        }
        else
        {
            this.GetComponent<Button>().interactable = true;
        }
    }

    public void ButtonOnClick()
    {
        _item.Buy("Gems", _item.GemsPrice);
    }
}
