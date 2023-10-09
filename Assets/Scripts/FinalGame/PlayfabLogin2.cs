using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class PlayfabLogin2 : MonoBehaviour
{
    private const string AuthGuidKey = "auth_guid_key";
    public void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
            {
                PlayFabSettings.staticSettings.TitleId = "164CE";
            }

        var needCreation = PlayerPrefs.HasKey(AuthGuidKey);
        var id = PlayerPrefs.GetString(AuthGuidKey, Guid.NewGuid().ToString());

        var request = new LoginWithCustomIDRequest
        {
            CustomId = id,
            CreateAccount = !needCreation
        };

        PlayFabClientAPI.LoginWithCustomID(request,
            result =>
            {
                PlayerPrefs.SetString(AuthGuidKey, id);
                OnLoginSuccess(result);
            },
             OnFailure);
    }
    private void OnLoginSuccess(LoginResult result)
    {
        Debug.LogWarning("OnLoginSuccess!");
        //SetUsrData(result.PlayFabId);
        //MacePurchase();
        //GetUserInventory();
        GetCurrencies();
    }

    private void GetCurrencies()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
            result => DisplayCurrencies(result.VirtualCurrency),
            result => OnFailure(result));

    }

    private void DisplayCurrencies(Dictionary<string,int> currencys)
    {
        foreach(var c in currencys)
        {
            Debug.LogWarning(c.Key + "=" + c.Value);
        }
    }
    
    private void GetUserInventory()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
            result => ShowInventory(result.Inventory), OnFailure);
    }

    private void ShowInventory(List<ItemInstance> inventory)
    {
        var item = inventory.First();
        Debug.Log(item.ItemInstanceId + "     " + item.ItemId);
        ConsumePotion(item);
    }

    private void ConsumePotion(ItemInstance item)
    {
        PlayFabClientAPI.ConsumeItem(new ConsumeItemRequest
        {
            ConsumeCount = 1,
            ItemInstanceId = item.ItemInstanceId
        },
        result =>
        {
            Debug.Log("Success ConsumePotion");
        },
        error => OnFailure(error));
    }

    private void MacePurchase()
    {
        PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest
        {
            CatalogVersion = "main",
            ItemId = "armor",
            Price=10,
            VirtualCurrency = "CO"
        },
        result =>
        {
            Debug.Log("Success PurchaseItem");
        },
        error => OnFailure(error));
    }

    private void SetUsrData(string playFabId)
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
        {
            Data = new System.Collections.Generic.Dictionary<string, string>
            {
                { "time_daily_reward",DateTime.UtcNow.ToString() }
            }
        },
        result =>
        {
            Debug.Log("SetUsrData");
            GetUserData(playFabId, "time_daily_reward");
        },
        error => OnFailure(error));
    }

    private void GetUserData(string playFabId, string key)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest
        {
            PlayFabId = playFabId
        },
        result =>
        {
            if (result.Data.ContainsKey(key))
 
            {
                Debug.Log($"{key} = {result.Data[key].Value}");
            }
        },
        OnFailure);
    }

    private void OnFailure(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        Debug.LogError($"Something went wrong: {errorMessage}");
    }
}
