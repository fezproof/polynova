using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype.NetworkLobby
{
	public class LobbyMatchmakerPanel : MonoBehaviour {

		public LobbyManager lobbyManager;

		public RectTransform lobbyServerList;
		public InputField matchNameInput;

		public void OnEnable()
		{
			lobbyManager.topPanel.ToggleVisibility(true);

			matchNameInput.onEndEdit.RemoveAllListeners();
			matchNameInput.onEndEdit.AddListener(onEndEditGameName);
		}

		public void OnClickOpenServerList()
		{
			lobbyManager.StartMatchMaker();
			lobbyManager.backDelegate = lobbyManager.SimpleBackClbk;
			lobbyManager.ChangeTo(lobbyServerList);
		}

		public void OnClickCreateMatchmakingGame()
		{
			lobbyManager.StartMatchMaker();
			lobbyManager.matchMaker.CreateMatch(
				matchNameInput.text,
				(uint)lobbyManager.maxPlayers,
				true,
				"", "", "", 0, 0,
				lobbyManager.OnMatchCreate);

			lobbyManager.backDelegate = lobbyManager.StopHost;
			lobbyManager._isMatchmaking = true;
			lobbyManager.DisplayIsConnecting();

			lobbyManager.SetServerInfo("Matchmaker Host", lobbyManager.matchHost);
		}

		void onEndEditGameName(string text)
		{
			if (Input.GetKeyDown(KeyCode.Return))
			{
				OnClickCreateMatchmakingGame();
			}
		}
	}
}
