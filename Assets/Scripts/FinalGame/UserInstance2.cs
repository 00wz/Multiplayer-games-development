using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Realtime;
using Cinemachine;
using StarterAssets;
using PlayFab;

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

		private GameObject localPlayer;

		[Tooltip("The prefab to use for representing the player")]
		[SerializeField]
		private GameObject playerPrefab;

		[Tooltip("Main Virtual Camera")]
		[SerializeField]
		private CamerManager camerManager;

		[Tooltip("UIView")]
		[SerializeField]
		private UIView UIView;

		[Tooltip("GameMenu")]
		[SerializeField]
		private GameMenuView gameMenuView;

		[Tooltip("Spawn Points")]
		[SerializeField]
		private Transform[] SpawnPoints;

		private const string MAIN_SCENE_NAME = "PhotonLobby";
		#endregion

		#region MonoBehaviour CallBacks
		
		void Start()
		{
			gameMenuView.AddExitListener(LeaveRoom);
		}
		
		/// <summary>
		/// MonoBehaviour method called on GameObject by Unity on every frame.
		/// </summary>
		void Update()
		{
			if(PhotonNetwork.InRoom && localPlayer == null)
            {
				camerManager.AimingIsEnabled(false);
                if (Inputs.Instance.jump)
                {
					SpawnPlayer();
                }
			}
			else
            {
				camerManager.AimingIsEnabled(true);
			}
			/*
			// "back" button of phone equals "Escape". quit app if that's pressed
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				QuitApplication();
			}
			*/
		}
		
		#endregion

		#region Photon Callbacks

		/// <summary>
		/// Called when the local player left the room. We need to load the launcher scene.
		/// </summary>
		public override void OnLeftRoom()
		{
			SceneManager.LoadScene(MAIN_SCENE_NAME);
		}

		#endregion

		#region Public Methods

		public void LeaveRoom()
		{
			PhotonNetwork.LeaveRoom();
		}

        private void OnDestroy()
        {
			Debug.LogWarning("PlayFabClientAPI.IsClientLoggedIn()=" + PlayFabClientAPI.IsClientLoggedIn());
            if (PlayFabClientAPI.IsClientLoggedIn())
            {
				PlayFabClientAPI.AddUserVirtualCurrency(new PlayFab.ClientModels.AddUserVirtualCurrencyRequest
				{
					Amount = UIView._frags,
					VirtualCurrency = "FR"
				},
				(result) => Debug.LogWarning(result.VirtualCurrency + "=" + result.Balance + 
					"	change=" + result.BalanceChange),
				(result) => Debug.LogError(result.ErrorMessage));
			}
		}

		#endregion

		#region Private Methods
		private void SpawnPlayer()
        {
			var index = UnityEngine.Random.Range(0, SpawnPoints.Length);
			// we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
			localPlayer = PhotonNetwork.Instantiate(this.playerPrefab.name, SpawnPoints[index].position,
				SpawnPoints[index].rotation, 0);
			PlayerClass player = localPlayer.GetComponent<PlayerClass>();
			player.TakeControl();
			camerManager.SetTarget(player.PlayerCameraRoot.transform);
			UIView.SetPlayer(player);
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
