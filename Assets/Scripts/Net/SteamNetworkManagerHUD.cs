using UnityEngine;
using Mirror;
using Steamworks;

public class SteamNetworkManagerHUD : MonoBehaviour
{
	NetworkManager manager;

	public int offsetX;
	public int offsetY;

	private const string hostAddressKey = "Bubble";

	protected Callback<LobbyCreated_t> lobbyCreated;
	protected Callback<GameLobbyJoinRequested_t> lobbyJoinRequested;
	protected Callback<LobbyEnter_t> lobbyEntered;

	void Awake()
	{
		manager = GetComponent<NetworkManager>();
	}

	private void Start()
	{
		if (!SteamAPI.Init())
		{
			Debug.Log("Steam��ʼ��ʧ��/δ���ӵ�Steam������");
			return;
		}
		Debug.Log("Steam��ʼ���ɹ�/�����ӵ�Steam������");
		lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
		lobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnLobbyJoinRequested);
		lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
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
			Debug.Log("Steam������������ʧ��");
			return;
		}
		Debug.Log("Steam�������������ɹ�");
		manager.StartHost();
		SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), hostAddressKey, SteamUser.GetSteamID().ToString());

	}
	private void OnLobbyJoinRequested(GameLobbyJoinRequested_t callback)
	{
		Debug.Log("�յ��������������");
		SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
	}
	private void OnLobbyEntered(LobbyEnter_t callback)
	{
		Debug.Log("����ҽ������");
		string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), hostAddressKey);
		manager.networkAddress = hostAddress;

		if (!manager.isNetworkActive)
		{
			manager.StartClient();
			Debug.Log("����������ӵ�����...���Ժ�...");
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
		if (NetworkServer.active && NetworkClient.isConnected)
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
		else if (NetworkClient.isConnected)
		{
			// stop client if client-only
			if (GUILayout.Button("Leave Room"))
				manager.StopClient();
		}
	}
}
