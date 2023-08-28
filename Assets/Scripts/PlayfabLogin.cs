using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
public class PlayfabLogin : MonoBehaviour
{
    public void Start()
    {
        // Here we need to check whether TitleId property is configured in settings ornot
    if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            /*
            * If not we need to assign it to the appropriate variable manually
            * Otherwise we can just remove this if statement at all
            */
            PlayFabSettings.staticSettings.TitleId = "164CE";
        }
        var request = new LoginWithCustomIDRequest
        {
            CustomId = "Player 1",
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }
    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Congratulations, you made successful API call!");
    }
    private void OnLoginFailure(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        Debug.LogError($"Something went wrong: {errorMessage}");
    }
}
