using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour{
	public event System.Action<Transform> OnStartGame;

	[Header("Movement")]
	[SerializeField]
	private float moveSpeed = 10;
	[SerializeField]
	private float poisonMoveSpeed = 5;

	[Space]
	[SerializeField]
	private float jumpHeight = 8f;
	[SerializeField]
	private int maxJumpCount = 2;
	[SerializeField]
	private float dropMultiplier = 2.5f;
	[SerializeField]
	private float lowJumpMultiplier = 2f;
	private int jumpCount;

	[Header("Other")]
	[SerializeField]
	private GameObject playerCam;
	[SerializeField]
	private GameObject playerUICam;
	[SerializeField]
	private Color selfColor = Color.blue;

	[HideInInspector]
	public Transform startPos;
	[HideInInspector]
	public bool poisioned = false;

	public int playerCounter {
		get;
		private set;
	}

	private void Start() {
		startPos = FindObjectOfType<NetworkStartPosition>().transform;		
	}

	void Update()    {
		//needing to add to avoid having all players move
		if(!isLocalPlayer) {
			return;
		}

		Movement();
		Jump();
	}

	//moves the player
	void Movement() {
		float x = Input.GetAxis("Horizontal");
		Vector3 dir = new Vector3(x, 0, 0);
		RaycastHit hit;
		Physics.Raycast(transform.position, dir, out hit, 1f);
		if (hit.transform == null || hit.transform.tag != "Ground") {
			transform.Translate(dir * Speed() * Time.deltaTime);
		}
	}

	//checks which speed to use
	float Speed() {
		if (poisioned) {
			return poisonMoveSpeed;
		} else {
			return moveSpeed;
		}
	}

	//lets the player jump up to a maximum amount of times
	void Jump() {
		if (Input.GetButtonDown("Jump") && jumpCount < maxJumpCount) {
			jumpCount++;
			transform.GetComponent<Rigidbody>().velocity = Vector3.up * jumpHeight;
		}

		//fix to make the jump look better
		if (transform.GetComponent<Rigidbody>().velocity.y < 0) {
			transform.GetComponent<Rigidbody>().velocity += Vector3.up * Physics.gravity.y * (dropMultiplier -1) * Time.deltaTime;
		} else if (transform.GetComponent<Rigidbody>().velocity.y > 0 && !Input.GetButtonDown("Jump")) {
			transform.GetComponent<Rigidbody>().velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
		}
	}

	//called when the game starts
	//changes the player color and name
	public override void OnStartLocalPlayer() {
		base.OnStartLocalPlayer();
		gameObject.GetComponent<MeshRenderer>().material.color = selfColor;
		playerCam.SetActive(true);
		playerUICam.SetActive(true);
		OnStartGame?.Invoke(transform);
		StartCoroutine(SetPlayerName());
	}

	//lowers playercount when a player leaves
	public override void OnNetworkDestroy() {
		base.OnNetworkDestroy();
		playerCounter--;
	}

	//sets the name of each player
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
			PlayerInfo p = players[playerCounter].GetComponent<PlayerInfo>();
			p.SetId(playerCounter + 1);
			p.SetName();
		}
	}

	//resets the jumpcount
	private void OnCollisionEnter(Collision collision) {
		if (collision.transform.tag == "Ground") {
			jumpCount = 0;
		}
	}
}
