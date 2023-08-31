using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class CreateAccountWindow : AccountDataWibdowBase
{
    [SerializeField]
    private TMP_InputField _mailField;

    [SerializeField]
    private Button _createAccountButton;

    private string _mail;

    protected override void SubscriptionsElementsUi()
    {
        base.SubscriptionsElementsUi();

        _mailField.onValueChanged.AddListener(UpdateMail);
        _createAccountButton.onClick.AddListener(CreateAccount);
    }

    private void CreateAccount()
    {
        PlayFabClientAPI.RegisterPlayFabUser(new RegisterPlayFabUserRequest
        {
            Username = _username,
            Email = _mail,
            Password = _password
        }, result =>
         {
             Debug.Log($"Success create account: {_username}");
             EnterInGameScene();
         }, error =>
         {
             Debug.LogError($"Fail: {error.ErrorMessage}");
         });
    }

    private void UpdateMail(string mail)
    {
        _mail = mail;
    }
}
