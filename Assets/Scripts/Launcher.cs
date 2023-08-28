using UnityEngine;
using Photon.Pun;
public class Launcher : MonoBehaviourPunCallbacks
{
    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during early
    ///initialization phase.
    /// </summary>
    void Awake()
    {
        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client
        // and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during initialization
    ///phase.
    /// </summary>
    void Start()
    {
        Connect();
    }
    /// <summary>
    /// Start the connection process.
    /// - If already connected, we attempt joining a random room
    /// - if not yet connected, Connect this application instance to Photon Cloud
    ///Network
    /// </summary>
    public void Connect()
    {
        // we check if we are connected or not, we join if we are , else we initiate
        //the connection to the server.
        if (PhotonNetwork.IsConnected)
            return;

            // #Critical, we must first and foremost connect to Photon Online
            //Server.
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
