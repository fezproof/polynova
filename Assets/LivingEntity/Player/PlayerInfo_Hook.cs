using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Prototype.NetworkLobby
{
	public class PlayerInfo_Hook : LobbyHook {

		public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
		{
			LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer> ();
			NetworkPlayer player = gamePlayer.GetComponent<NetworkPlayer> ();
			player.colour = lobby.playerColor;
			player.playerName = lobby.playerName;
		}
	}
}
