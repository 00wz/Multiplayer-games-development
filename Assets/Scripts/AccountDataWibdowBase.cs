using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AccountDataWibdowBase : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField _usernameField;
    [SerializeField]
    private TMP_InputField _passwordField;
    
    protected string _username;
    protected string _password;

    private void Start()
    {
        SubscriptionsElementsUi();
    }

    protected virtual void SubscriptionsElementsUi()
    {
        _usernameField.onValueChanged.AddListener(UpdateUsername);
        _passwordField.onValueChanged.AddListener(UpdatePassword);
    }

    private void UpdatePassword(string password)
    {
        _password = password;
    }

    private void UpdateUsername(string username)
    {
        _username = username;
    }

    protected void EnterInGameScene()
    {
        SceneManager.LoadScene(1);
    }
}
