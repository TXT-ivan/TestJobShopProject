using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine.UI;
using System.Threading;
using System;
using UnityEngine.Networking;
using System.Security.Cryptography;
using System.Text;

public class Shop : MonoBehaviour
{
    public DataPlayer DataCurrentPlayer = new DataPlayer();
    public List<string> AllItems;
    public UnityWebRequest www_yandex;

    // Данные пользователя
    public class DataPlayer
    {
        public Dictionary<string, int> Currency = new Dictionary<string, int>();
        public Dictionary<string, Dictionary<string, int?>> PurchasedItems = new Dictionary<string, Dictionary<string, int?>>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetTimeCorroutine());

        Item[] ItemCollection = this.GetComponentsInChildren<Item>();

        if (ItemCollection != null && ItemCollection.Length > 0)
        {
            foreach (Item item in ItemCollection)
            {
                AllItems.Add(item.ItemName);
            }
        }

        if(PlayerPrefs.HasKey("TestJobShop"))
        {
            LoadGame();
        }
        else
        {
            SaveGame();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Функция сохранения игры
    public void SaveGame()
    {
        PlayerPrefs.SetString("TestJobShop", JsonConvert.SerializeObject(DataCurrentPlayer));

        string checksum = Md5Sum(JsonConvert.SerializeObject(DataCurrentPlayer));
        PlayerPrefs.SetString("CHECKSUM", checksum);

        SetCurrentCurrencyValue();
    }

    // Функция загрузки игры
    public void LoadGame()
    {
        if (PlayerPrefs.HasKey("TestJobShop"))
        {
            DataCurrentPlayer = JsonConvert.DeserializeObject<DataPlayer>(PlayerPrefs.GetString("TestJobShop"));

            if (HasBeenEdited())
            {
                foreach (var purchased_item in DataCurrentPlayer.PurchasedItems)
                {
                    for (int y = 0; y < AllItems.Count; y++)
                    {
                        if (AllItems[y] == purchased_item.Key)
                        {
                            this.GetComponentsInChildren<Item>().First(x => x.ItemName == AllItems[y]).IsBuy = true;
                            this.GetComponentsInChildren<Item>().First(x => x.ItemName == AllItems[y]).GetComponentInChildren<Toggle>().isOn = true;

                            foreach (var date_time in purchased_item.Value)
                            {
                                if (date_time.Value != null)
                                {
                                    this.GetComponentsInChildren<Item>().First(x => x.ItemName == AllItems[y]).ItemEndPerchaseDateTime = Convert.ToDateTime(date_time.Key).AddHours(date_time.Value.Value).ToString();
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.Log("CHECKSUM has been edited!");

                DataCurrentPlayer.Currency = this.GetComponentInChildren<Item>().ItemCurrencyCollection;
                DataCurrentPlayer.PurchasedItems.Clear();
            }

            SetCurrentCurrencyValue();
        }
    }

    public void SetCurrentCurrencyValue()
    {
        Text[] counters = this.GetComponentsInChildren<Text>().First(x => x.name == "Currency").GetComponentsInChildren<Text>();

        DataCurrentPlayer.Currency.TryGetValue("Coins", out int coin_value);
        counters.First(x => x.name == "CoinsCounter").text = coin_value.ToString();

        DataCurrentPlayer.Currency.TryGetValue("Gems", out int gems_value);
        counters.First(x => x.name == "GemsCounter").text = gems_value.ToString();
    }

    // Получения соединения по URI
    IEnumerator GetTimeCorroutine()
    {
        while(true)
        {
            www_yandex = UnityWebRequest.Get("https://www.microsoft.com/");
            www_yandex.SendWebRequest();
            yield return new WaitForSeconds(1);
        }
    }

    // Получение чек суммы данынх
    private string Md5Sum(string prefs)
    {
        MD5 md5 = MD5.Create();

        byte[] prefs_bytes = Encoding.ASCII.GetBytes(prefs);
        byte[] hash_bytes = md5.ComputeHash(prefs_bytes);

        StringBuilder SB = new StringBuilder();
        for (int i = 0; i < hash_bytes.Length; i++)
        {
            SB.Append(hash_bytes[i].ToString("X2"));
        }
        return SB.ToString();
    }

    // Проверка соответствия чек суммы загруженных данных
    public bool HasBeenEdited()
    {
        if (!PlayerPrefs.HasKey("CHECKSUM"))
        {
            return true;
        }

        string checksum_saved = PlayerPrefs.GetString("CHECKSUM");
        string checksum_current = Md5Sum(JsonConvert.SerializeObject(DataCurrentPlayer));

        return checksum_saved.Equals(checksum_current);
    }
}
