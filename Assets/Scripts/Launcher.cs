using UnityEngine;
using Photon.Pun;
public class Launcher : MonoBehaviourPunCallbacks
{

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        Connect();
    }

    /// </summary>
    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
            return;

        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = Application.version;
        
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        PhotonNetwork.CreateRoom("NewRoom");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
    }
}
