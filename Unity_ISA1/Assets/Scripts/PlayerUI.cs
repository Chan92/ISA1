using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerUI : NetworkBehaviour {
	[Header("UI")]
	[SerializeField]
	private Text playerNameText;
	[SerializeField]
	private GameObject playerArrow;

	[Space(10)]
	[SerializeField]
	private GameObject poisonedUI;
	[SerializeField]
	private GameObject playerFinishUI;


	private void OnEnable() {	
		transform.GetComponent<PlayerInfo>().OnSetName += UpdateName;
		transform.GetComponent<PlayerController>().OnStartGame += StartUISettings;
		MapInteractions.OnPoisoned += SetPoisonedState;
		Finishline.OnFinished += SetFinishState;
	}

	public void OnDisable() {
		transform.GetComponent<PlayerInfo>().OnSetName -= UpdateName;
		transform.GetComponent<PlayerController>().OnStartGame -= StartUISettings;
		MapInteractions.OnPoisoned -= SetPoisonedState;
		Finishline.OnFinished -= SetFinishState;
	}

	public void StartUISettings(Transform player) {
		if(player == transform) {
			playerFinishUI.SetActive(false);
			poisonedUI.SetActive(false);
			playerArrow.SetActive(true);
		}
	}

	public void UpdateName(string name) {		
		playerNameText.text = "[" + name + "]";
	}

	public void SetPoisonedState(Transform player, bool state) {
		if(player == transform) {
			poisonedUI.SetActive(state);
		}
	}

	public void SetFinishState(PlayerInfo player, string rank) {
		if(isLocalPlayer) {
			if(player.transform == transform && player.playerIsFinished) {
				playerFinishUI.SetActive(true);
				playerFinishUI.GetComponentInChildren<Text>().text = rank;
			}
		}
	}
}
