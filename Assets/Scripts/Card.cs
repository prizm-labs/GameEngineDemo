using UnityEngine;
using System.Collections;
using TouchScript.Gestures;

public class Card : MonoBehaviour {

	private TapGesture myTapGesture;


	void Awake() {
		myTapGesture = GetComponent<TapGesture> ();
		if (myTapGesture == null) {
			Debug.LogError ("WE ARE IN TROUBLE, CARD DOESN'T HAVE TAP GESTURE");
		}

		myTapGesture.NumberOfTapsRequired = 2;	//double tap will turn it over
	}

	void OnEnable() {
		myTapGesture.Tapped += MyTapGesture_Tapped;
	}

	void OnDisable() {
		myTapGesture.Tapped -= MyTapGesture_Tapped;
	}

	void MyTapGesture_Tapped (object sender, System.EventArgs e)
	{
		FlipMeOver ();
	}

	public void FlipMeOver() {
		transform.localEulerAngles = transform.localEulerAngles + Vector3.right * 180;
	}

	void OnTriggerEnter(Collider other) {
		Debug.Log ("triggered with: " + other.name);
		if (other.tag == "player") {
			Debug.Log ("triggered with player, adding this card to their list");
			Player myNewOwner = other.gameObject.GetComponent<Player> ();
			if (!myNewOwner.myOwnedCards.Contains (this)) {
				myNewOwner.myOwnedCards.Add (this);
			}
		}
	}
}
