using UnityEngine;
using Mirror;
using Steamworks;
using Utils;
using System.Collections;

public class SteamNetworkManagerHUD : MonoBehaviour
{
	NetworkManager manager;
	[HideInInspector] public bool initialized;
	public int offsetX;
	public int offsetY;
	public float linkSteamTickTime;

	private const string hostAddressKey = "Bubble";
	public static CSteamID lobbyId { get; private set; }

	protected Callback<LobbyCreated_t> lobbyCreated;
	protected Callback<GameLobbyJoinRequested_t> lobbyJoinRequested;
	protected Callback<LobbyEnter_t> lobbyEntered;

	private void Awake()
	{
		manager = GetComponent<NetworkManager>();
		if (!initialized)
		{
			StartCoroutine(LinkSteam(linkSteamTickTime));
		}
	}

	void RegisterSteamCallback()
	{
		lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
		lobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnLobbyJoinRequested);
		lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
		SteamClient.SetWarningMessageHook(new SteamAPIWarningMessageHook_t(SteamAPIDebugTextHook));
	}

	private void Update()
	{
		if (initialized)
		{
			SteamAPI.RunCallbacks();
		}
	}

	private void OnDestroy()
	{
		if (initialized)
		{
			SteamAPI.Shutdown();
		}
	}

	IEnumerator LinkSteam(float tickTime)
	{
		Debug.Log("正在连接Steam服务器");
		while (!initialized)
		{
			initialized = SteamAPI.Init();
			yield return new WaitForSecondsRealtime(tickTime);
		}
		Debug.Log("已连接到Steam服务器");
		RegisterSteamCallback();
	}

	void SteamAPIDebugTextHook(int nSeverity, System.Text.StringBuilder pchDebugText)
	{
		Debug.LogError(pchDebugText);
	}

	void OnGUI()
	{
		// If this width is changed, also change offsetX in GUIConsole::OnGUI
		int width = 300;

		GUILayout.BeginArea(new Rect(10 + offsetX, 40 + offsetY, width, 9999));

		if (!NetworkClient.isConnected && !NetworkServer.active)
			StartButtons();
		else
			StatusLabels();

		//if (NetworkClient.isConnected && !NetworkClient.ready)
		//{
		//	if (GUILayout.Button("Client Ready"))
		//	{
		//		// client ready
		//		NetworkClient.Ready();
		//		if (NetworkClient.localPlayer == null)
		//			NetworkClient.AddPlayer();
		//	}
		//}

		StopButtons();

		GUILayout.EndArea();
	}

	private void OnLobbyCreated(LobbyCreated_t callback)
	{
		if (callback.m_eResult != EResult.k_EResultOK)
		{
			Debug.Log("Steam联机大厅创建失败");
			return;
		}
		Debug.Log("Steam联机大厅创建成功");
		manager.StartHost();
		SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), hostAddressKey, SteamUser.GetSteamID().ToString());

	}
	private void OnLobbyJoinRequested(GameLobbyJoinRequested_t callback)
	{
		Debug.Log("收到加入大厅的申请");
		SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
	}
	private void OnLobbyEntered(LobbyEnter_t callback)
	{
		lobbyId = new CSteamID(callback.m_ulSteamIDLobby);
		Debug.Log("有玩家进入大厅");
		string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), hostAddressKey);
		manager.networkAddress = hostAddress;

		if (!manager.isNetworkActive)
		{
			manager.StartClient();
			Debug.Log("玩家正在连接到主机...请稍候...");
		}
	}

	public void HostLobby()
	{
		SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, manager.maxConnections);
	}

	void StartButtons()
	{
		if (!NetworkClient.active)
		{
#if UNITY_WEBGL
                // cant be a server in webgl build
                if (GUILayout.Button("Single Player"))
                {
                    NetworkServer.listen = false;
                    manager.StartHost();
                }
#else
			// Server + Client
			if (GUILayout.Button("Create Room"))
				HostLobby();
#endif
		}
		else
		{
			// Connecting
			GUILayout.Label($"Connecting to {manager.networkAddress}..");
			if (GUILayout.Button("Cancel Connection Attempt"))
				manager.StopClient();
		}
	}

	void StatusLabels()
	{
		// host mode
		// display separately because this always confused people:
		//   Server: ...
		//   Client: ...
		if (NetworkServer.active && NetworkClient.active)
		{
			// host mode
			GUILayout.Label($"<b>Host</b>: running via {Transport.active}");
		}
		else if (NetworkClient.isConnected)
		{
			// client only
			GUILayout.Label($"<b>Client</b>: connected to {manager.networkAddress} via {Transport.active}");
		}
	}

	void StopButtons()
	{
		if (NetworkClient.isConnected)
		{
			if (NetworkServer.active)
			{

				GUILayout.BeginHorizontal();
#if UNITY_WEBGL
                if (GUILayout.Button("Stop Single Player"))
                    manager.StopHost();
#else
				// stop host if host mode
				if (GUILayout.Button("Close Room"))
					manager.StopHost();

#endif
				GUILayout.EndHorizontal();
			}
			else
			{
				// stop client if client-only
				if (GUILayout.Button("Leave Room"))
					manager.StopClient();

			}

			if (GUILayout.Button("Invite Friend"))
			{
				SteamFriends.ActivateGameOverlayInviteDialog(lobbyId);
			}
		}
	}
}
