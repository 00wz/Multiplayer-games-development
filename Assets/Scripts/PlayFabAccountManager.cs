using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayFabAccountManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _titleLabel;

    private void Start()
    {
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetAccount, OnError);
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest(), OnGetCatalogSuccess, OnError);
        PlayFabServerAPI.GetRandomResultTables(new PlayFab.ServerModels.GetRandomResultTablesRequest(), 
            OnGetTableSuccess, OnError);
    }

    private void OnGetTableSuccess(PlayFab.ServerModels.GetRandomResultTablesResult result)
    {
        
    }

    private void OnGetCatalogSuccess(GetCatalogItemsResult result)
    {
        Debug.Log("OnGetCatalogSuccess");
        ShowItems(result.Catalog);
    }

    private void ShowItems(List<CatalogItem> catalog)
    {
        foreach(var item in catalog)
        {
            Debug.Log($"{item.ItemId}");
        }
    }

    private void OnError(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        Debug.LogError($"Something went wrong: {errorMessage}");
    }

    private void OnGetAccount(GetAccountInfoResult result)
    {
        _titleLabel.text = $"PlayFabId: {result.AccountInfo.PlayFabId}";
    }
}
