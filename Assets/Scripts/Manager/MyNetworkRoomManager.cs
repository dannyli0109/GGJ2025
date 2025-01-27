using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;
using Steamworks;

public class MyNetworkRoomManager : NetworkRoomManager
{
	bool showStartButton;

	public void ChangeScene(string newSceneName)
	{
		//foreach (NetworkRoomPlayer roomPlayer in roomSlots)
		//{
		//	Transform startPos = GetStartPosition();
		//	GameObject gamePlayer = startPos != null
		//			? Instantiate(playerPrefab, startPos.position, startPos.rotation)
		//			: Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
		//	NetworkServer.ReplacePlayerForConnection(roomPlayer.connectionToClient, gamePlayer, ReplacePlayerOptions.KeepAuthority);
		//}
		foreach (NetworkRoomPlayer roomPlayer in roomSlots)
		{
			if (roomPlayer == null)
				continue;

			// find the game-player object for this connection, and destroy it
			//NetworkIdentity identity = roomPlayer.GetComponent<NetworkIdentity>();

			if (NetworkServer.active)
			{
				// re-add the room object
				//roomPlayer.GetComponent<NetworkRoomPlayer>().readyToBegin = false;
				NetworkServer.ReplacePlayerForConnection(roomPlayer.connectionToClient, roomPlayer.gameObject, ReplacePlayerOptions.KeepAuthority);
			}
		}
		base.ServerChangeScene(newSceneName);

		//allPlayersReady = false;
	}

	public override void OnRoomServerPlayersReady()
	{
		// calling the base method calls ServerChangeScene as soon as all players are in Ready state.
		if (Mirror.Utils.IsHeadless())
		{
			base.OnRoomServerPlayersReady();
		}
		else
		{
			showStartButton = true;
		}
	}

	public override void OnRoomStopClient()
	{
		Debug.Log("OnRoomStopClient");
		SteamMatchmaking.LeaveLobby(SteamNetworkManagerHUD.lobbyId);
	}

	public override void OnGUI()
	{
		base.OnGUI();

		if (/*NetworkServer.active && */allPlayersReady && showStartButton && GUI.Button(new Rect(150, 300, 120, 20), "START GAME"))
		{
			// set to false to hide it in the game scene
			showStartButton = false;

			ServerChangeScene(GameplayScene);
		}
	}
}
