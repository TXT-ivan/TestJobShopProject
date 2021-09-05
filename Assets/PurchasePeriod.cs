using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurchasePeriod : MonoBehaviour
{
    private Item _item;

    // Start is called before the first frame update
    void Start()
    {
        _item = this.GetComponentInParent<Item>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_item.IsBuy == true)
        {
            this.GetComponent<Dropdown>().interactable = false;
        }
        else
        {
            this.GetComponent<Dropdown>().interactable = true;
        }
    }
}
