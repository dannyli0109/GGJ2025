using Mirror;
using Steamworks;
using UnityEngine;

public class MyNetworkRoomPlayer : NetworkRoomPlayer
{
	[SyncVar]
	public ulong steamId;
	[SyncVar]
	public string steamName;

	public override void OnStartClient()
	{
		ulong id = SteamUser.GetSteamID().m_SteamID;
		CmdSetSteamId(id);
	}

	[Command]
	void CmdSetSteamId(ulong id)
	{
		steamId = id;
		CSteamID cSteamID = new CSteamID(id);
		steamName = SteamFriends.GetFriendPersonaName(cSteamID);
	}

	public override void OnClientEnterRoom()
	{
		//Debug.Log($"OnClientEnterRoom {SceneManager.GetActiveScene().path}");
	}

	public override void OnClientExitRoom()
	{
		//Debug.Log($"OnClientExitRoom {SceneManager.GetActiveScene().path}");
	}

	public override void IndexChanged(int oldIndex, int newIndex)
	{
		//Debug.Log($"IndexChanged {newIndex}");
	}

	public override void ReadyStateChanged(bool oldReadyState, bool newReadyState)
	{
		//Debug.Log($"ReadyStateChanged {newReadyState}");
	}

	#region Optional UI

	/// <summary>
	/// Render a UI for the room. Override to provide your own UI
	/// </summary>
	public override void OnGUI()
	{
		if (!showRoomGUI)
			return;

		NetworkRoomManager room = NetworkManager.singleton as NetworkRoomManager;
		if (room)
		{
			if (!room.showRoomGUI)
				return;

			if (!Mirror.Utils.IsSceneActive(room.RoomScene))
				return;

			DrawPlayerReadyState();
			DrawPlayerReadyButton();
		}
	}

	void DrawPlayerReadyState()
	{
		GUILayout.BeginArea(new Rect(20f + (index * 100), 200f, 90f, 130f));
		if (string.IsNullOrEmpty(steamName))
		{
			GUILayout.Label($"Player [{index + 1}]");
		}
		else
		{
			GUILayout.Label(steamName);
		}

		if (readyToBegin)
			GUILayout.Label("Ready");
		else
			GUILayout.Label("Not Ready");

		if (((isServer && index > 0) || isServerOnly) && GUILayout.Button("REMOVE"))
		{
			// This button only shows on the Host for all players other than the Host
			// Host and Players can't remove themselves (stop the client instead)
			// Host can kick a Player this way.
			GetComponent<NetworkIdentity>().connectionToClient.Disconnect();
		}

		GUILayout.EndArea();
	}

	void DrawPlayerReadyButton()
	{
		if (NetworkClient.active && isLocalPlayer)
		{
			GUILayout.BeginArea(new Rect(20f, 300f, 120f, 20f));

			if (readyToBegin)
			{
				if (GUILayout.Button("Cancel"))
					CmdChangeReadyState(false);
			}
			else
			{
				if (GUILayout.Button("Ready"))
					CmdChangeReadyState(true);
			}

			GUILayout.EndArea();
		}
	}

	#endregion
}
