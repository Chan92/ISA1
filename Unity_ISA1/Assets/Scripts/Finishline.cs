using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Finishline : MonoBehaviour {
	public static event System.Action<PlayerInfo, string> OnFinished;

	public static Finishline instance;
	private List<string> finishLeaderboard = new List<string>();
	private GameObject[] players;
	public Text leaderBoardText;

	private void Awake() {
		instance = this;
	}

	private void Start() {
		StartCoroutine(DelayedStart());
	}

	IEnumerator DelayedStart() {
		yield return new WaitForSeconds(0.5f);
		players = GameObject.FindGameObjectsWithTag("Player");
		print("Game start with: " + players.Length + " players.");
		leaderBoardText.text = UpdateLeaderBoard();
	}

	private void OnTriggerEnter(Collider other) {
		if(other.tag == "Player") {
			PlayerInfo p = other.GetComponent<PlayerInfo>();
			if(!finishLeaderboard.Contains(p.PlayerName)) {
				p.playerIsFinished = true;
				finishLeaderboard.Add(p.PlayerName);
				OnFinished?.Invoke(p, GetRankName(p.PlayerName));
				leaderBoardText.text = UpdateLeaderBoard();
			}
		}
	}

	private string GetRankName(string playerName) {
		string rankName = "";

		switch(finishLeaderboard.IndexOf(playerName)) {
			case 0:
				rankName = "WINNER";
				break;
			case 1:
				rankName = "2nd PLACE";
				break;
			case 2:
				rankName = "3rd PLACE";
				break;
			case 3:
				rankName = "4th PLACE";
				break;
			default:
				rankName = "LOSE";
				print("Error: rank not found~");
				break;
		}

		return rankName;
	}

	private string UpdateLeaderBoard() {		
		string leaderboardInfo = "LEADERBOARD \n\r";

		for(int i = 0; i < finishLeaderboard.Count; i++) {
			string playerName = finishLeaderboard[i];
			leaderboardInfo += "#" + (i+1) + ": " + playerName + "\n";
		}

		return leaderboardInfo;
	}
}
