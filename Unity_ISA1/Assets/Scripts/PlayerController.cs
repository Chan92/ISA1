using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour{
	public float moveSpeed = 10;
	public Color selfColor = Color.blue;
	public GameObject playerCam;
	private int playerCounter;

	void Update()    {
		if(!isLocalPlayer) {
			return;
		}

		Movement();
	}


	void Movement() {
		float x = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
		transform.Translate(x, 0, 0);
	}

	public override void OnStartLocalPlayer() {
		base.OnStartLocalPlayer();
		gameObject.GetComponent<MeshRenderer>().material.color = selfColor;
		playerCam.SetActive(true);
		StartCoroutine(SetPlayerName());
	}

	public override void OnNetworkDestroy() {
		base.OnNetworkDestroy();
		playerCounter--;
	}

	IEnumerator SetPlayerName() {
		yield return new WaitForSeconds(0);

		int lobbyPlayers = GameObject.FindGameObjectsWithTag("LobbyPlayer").Length;
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

		while(players.Length < lobbyPlayers) {
			yield return new WaitForSeconds(0);
			players = GameObject.FindGameObjectsWithTag("Player");
			if (players.Length >= lobbyPlayers) {
				break;
			}
		}

		for(; playerCounter < players.Length; playerCounter++) {
			Text playerName = players[playerCounter].GetComponentInChildren<Text>();
			if(playerName != null) {
				playerName.text = "[Player " + (playerCounter + 1) + "]";
			}
		}
	}
}
