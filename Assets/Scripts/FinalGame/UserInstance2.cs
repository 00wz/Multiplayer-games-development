using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Realtime;
using Cinemachine;

namespace Photon.Pun.Demo.PunBasics
{
#pragma warning disable 649

	/// <summary>
	/// Game manager.
	/// Connects and watch Photon Status, Instantiate Player
	/// Deals with quiting the room and the game
	/// Deals with level loading (outside the in room synchronization)
	/// </summary>
	public class UserInstance2 : MonoBehaviourPunCallbacks
	{

		#region Public Fields

		static public UserInstance2 Instance;

		#endregion

		#region Private Fields

		private GameObject instance;

		[Tooltip("The prefab to use for representing the player")]
		[SerializeField]
		private GameObject playerPrefab;

		[Tooltip("Main Virtual Camera")]
		[SerializeField]
		private CamerManager camerManager;

		private const string CAMERA_RESOURSES_PATH = "CameraRoot";
		#endregion

		#region MonoBehaviour CallBacks

		void Start()
		{
			Instance = this;

			if (PhotonNetwork.InRoom && instance == null)
			{
				SpawnPlayer();
			}
			else
			{
				Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
			}

		}
		/*
		/// <summary>
		/// MonoBehaviour method called on GameObject by Unity on every frame.
		/// </summary>
		void Update()
		{
			// "back" button of phone equals "Escape". quit app if that's pressed
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				QuitApplication();
			}
		}
		*/
		#endregion

		#region Photon Callbacks

		public override void OnJoinedRoom()
		{
			// Note: it is possible that this monobehaviour is not created (or active) when OnJoinedRoom happens
			// due to that the Start() method also checks if the local player character was network instantiated!
			if (instance == null)
			{
				Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);

				SpawnPlayer();
			}
		}
		/*
		/// <summary>
		/// Called when a Photon Player got connected. We need to then load a bigger scene.
		/// </summary>
		/// <param name="other">Other.</param>
		public override void OnPlayerEnteredRoom(Player other)
		{
			Debug.Log("OnPlayerEnteredRoom() " + other.NickName); // not seen if you're the player connecting

			if (PhotonNetwork.IsMasterClient)
			{
				Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

				LoadArena();
			}
		}
		
		/// <summary>
		/// Called when a Photon Player got disconnected. We need to load a smaller scene.
		/// </summary>
		/// <param name="other">Other.</param>
		public override void OnPlayerLeftRoom(Player other)
		{
			Debug.Log("OnPlayerLeftRoom() " + other.NickName); // seen when other disconnects

			if (PhotonNetwork.IsMasterClient)
			{
				Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

				LoadArena();
			}
		}
		*/
		/// <summary>
		/// Called when the local player left the room. We need to load the launcher scene.
		/// </summary>
		public override void OnLeftRoom()
		{
			SceneManager.LoadScene("Photon");
		}

		#endregion

		#region Public Methods

		public void LeaveRoom()
		{
			PhotonNetwork.LeaveRoom();
		}

		public void QuitApplication()
		{
			Application.Quit();
		}

		#endregion

		#region Private Methods
		private void SpawnPlayer()
        {
			// we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
			instance = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
			PlayerClass player = instance.GetComponent<PlayerClass>();
			player.TakeControl();
			camerManager.SetTarget(player.PlayerCameraRoot.transform);
		}
		void LoadArena()
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
				return;
			}

			Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);

			PhotonNetwork.LoadLevel("PunBasics-Room for " + PhotonNetwork.CurrentRoom.PlayerCount);
		}

		#endregion

	}

}
