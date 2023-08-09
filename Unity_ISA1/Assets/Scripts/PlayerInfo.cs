using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInfo : MonoBehaviour {
	public event System.Action<string> OnSetName;

	public string PlayerName {
		get; private set;
	}

	public int PlayerId {
		get; private set;
	}

	public bool playerIsFinished = false;

	public void SetId(int newId) {
		PlayerId = newId;
	}
	
	//sets a base name
	public void SetName() {
		string newName = "Player " + PlayerId;
		SetName(newName);
	}

	//sets a selected name
	public void SetName(string newName) {
		PlayerName = newName;
		OnSetName?.Invoke(PlayerName);
	}
}
