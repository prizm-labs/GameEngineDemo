using UnityEngine;
using System.Collections;
using TouchScript.Gestures;

public class Card : MonoBehaviour {

	private TapGesture myTapGesture;

	public bool recoveringFromSave = false;
	public string _myDataPath;
	public string myDataPath {
		get { return _myDataPath; }
		set {
			Debug.Log ("someone tried to set myDataPath for CARD, assuming it was the serializer");
			StartCoroutine(ReloadThisCardDelayed ());
		}
	}

	void Awake() {
		myTapGesture = GetComponent<TapGesture> ();
		if (myTapGesture == null) {
			Debug.LogError ("WE ARE IN TROUBLE, CARD DOESN'T HAVE TAP GESTURE");
		}

		myTapGesture.NumberOfTapsRequired = 2;	//double tap will turn it over
	}

	void Start() {
		Debug.LogError ("card recovering from save: " + recoveringFromSave.ToString ());

		if (recoveringFromSave)
			Debug.LogError ("THIS PIECE IS RECOVERING FROM A SAVE STATE");
		else
			recoveringFromSave = true;
		
	}

	IEnumerator ReloadThisCardDelayed() {
		yield return new WaitForSeconds (Constants.timeDelayToLoad);
		ReloadThisCard ();
	}

	void ReloadThisCard() {
		GameObject cardPrefab = Resources.Load (myDataPath, typeof(GameObject)) as GameObject;
		GameObject replacementCard = Instantiate (cardPrefab);
		//replacementCard.transform = this.transform;
		Debug.Log("created replacementCard: " + replacementCard.name);
		Destroy (this.gameObject);
	}

	public void GiveMeMyDataPath(string newDataPath) {
		_myDataPath = newDataPath;
	}

	void OnEnable() {
		if (myTapGesture == null)
			myTapGesture = GetComponent<TapGesture> ();
		myTapGesture.Tapped += MyTapGesture_Tapped;
	}

	void OnDisable() {
		if (myTapGesture == null)
			myTapGesture = GetComponent<TapGesture> ();
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
