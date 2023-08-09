using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapInteractions : MonoBehaviour {
	public static event System.Action<Transform, bool> OnPoisoned;

	public enum InteractionType {
		Poision,
		Fire
	}

	public InteractionType interactionType;
	
	[Space]
	[SerializeField]
	private float poisionDuration = 5f;


	//resets player back to start when standing in fire
	//sets poision to true when standing in poision
	private void OnTriggerEnter(Collider other) {
		if(other.tag == "Player") {
			PlayerController p = other.GetComponent<PlayerController>();

			switch(interactionType) {
				case InteractionType.Fire:
					Vector3 startPos = p.startPos.position;
					other.transform.position = startPos;
					break;

				case InteractionType.Poision:
					OnPoisoned?.Invoke(p.transform, true);
					p.poisioned = true;
					break;
			}
		}
	}

	//start poision timer when leaving poision
	private void OnTriggerExit(Collider other) {
		if (other.tag == "Player" && interactionType == InteractionType.Poision) {
			StartCoroutine(Poisioned(other.GetComponent<PlayerController>()));
		}
	}

	//wait a small amount of time for the poision to wear off
	IEnumerator Poisioned(PlayerController p) {		
		yield return new WaitForSeconds(poisionDuration);
		OnPoisoned?.Invoke(p.transform, false);
		p.poisioned = false;
	}
}
