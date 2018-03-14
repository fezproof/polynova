using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype.NetworkLobby
{
	public class LobbyDirectPlayPanel : MonoBehaviour {

		public LobbyManager lobbyManager;
		public RectTransform lobbyPanel;
		public InputField ipInput;


		public void OnEnable()
		{
			lobbyManager.topPanel.ToggleVisibility(true);

			ipInput.onEndEdit.RemoveAllListeners();
			ipInput.onEndEdit.AddListener(onEndEditIP);
		}

		public void OnClickHost()
		{
			lobbyManager.StartHost();
		}

		public void OnClickJoin()
		{
			lobbyManager.ChangeTo(lobbyPanel);

			lobbyManager.networkAddress = ipInput.text;

			lobbyManager.StartClient();


			lobbyManager.backDelegate = lobbyManager.StopClientClbk;
			lobbyManager.DisplayIsConnecting();

			lobbyManager.SetServerInfo("Connecting...", lobbyManager.networkAddress);
		}

		public void OnClickDedicated()
		{
			lobbyManager.ChangeTo(null);
			lobbyManager.StartServer();

			lobbyManager.backDelegate = lobbyManager.StopServerClbk;

			lobbyManager.SetServerInfo("Dedicated Server", lobbyManager.networkAddress);
		}

		void onEndEditIP(string text)
		{
			if (Input.GetKeyDown(KeyCode.Return))
			{
				OnClickJoin();
			}
		}
	}
}
