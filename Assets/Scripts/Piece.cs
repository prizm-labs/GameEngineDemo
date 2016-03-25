using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;
using System.Linq;
using System;

[System.Serializable]
public class Piece : MonoBehaviour {

	[SerializeField]
	private TypeOfPiece _myType;
	public TypeOfPiece myType {
		get { return _myType; }
		set 
		{
			_myType = value; 
			if (_myType == TypeOfPiece.sprite2d) {
				meshFilter.mesh = Helper.GetQuadMesh ();
			} 
		}
	}
	[SerializeField]
	private Player _myNewOwner;
	public Player myNewOwner {
		get { return _myNewOwner; }
		set{
			_myNewOwner = value;
		}
	}

	public static Color defaultNewPieceColor = Color.white;

	[SerializeField]
	private ObjectCreatorButtons _myCategory;
	public ObjectCreatorButtons myCategory {
		get { return _myCategory; }
		set 
		{
			_myCategory = value; 

		}
	}

	[SerializeField]
	private bool _SerializationChecker;
	public bool SerializationChecker {
		get { return _SerializationChecker; }
		set {
			//this code runs if the piece was created by the save game manager (already was bootstrapped by DraMe)
			if (bootstrapped) {
				if (GetComponent<TransformGesture> () == null) {
					gameObject.AddComponent<TransformGesture> ();
					if (GetComponent<ApplyTransform> () != null) {
						gameObject.GetComponent<ApplyTransform> ().ReloadApplyTransform ();

					}
				}

				if (myType.ToString ().ToLower ().Contains ("dice") || myType.ToString().ToLower().Contains("cards")) {
					Debug.Log ("this piece is either a dice or a cardDeck, and was recovered from when saving.  Going to destroy it and create a new one");
					StartCoroutine (CreateNewPieceDelayed ());
				}
			}
		}
	}
	[SerializeField]
	private Color _myColor;
	public Color myColor {
		get { return _myColor;}
		set {
			_myColor = value;

			if (bootstrapped) {
				SetMeshesColors (_myColor);
			} else {
				SetMeshesColorsDelay (_myColor);
			}
		}
	}

	IEnumerator CreateNewPieceDelayed() {
		yield return new WaitForSeconds (Constants.timeDelayToLoad);
		NewPieceCreator.CreateNewPiece (myCategory, myType, transform.position);
		Destroy (this.gameObject);
	}
		

	public bool twoDimensional = false;
	[SerializeField]
	public AudioSource audioSource;
	[SerializeField]
	public MeshFilter meshFilter;
	[SerializeField]
	public GameObject childMeshobject;

	[SerializeField]
	private Material _myMaterial;
	public Material myMaterial{
		get { return _myMaterial; }
		set 
		{
			Debug.Log ("an outside source tried f*cking with my material, going to reload it just incase");
			if (bootstrapped) {
				Debug.Log ("this thing was recovered from when saving, reloading its material");
				StartCoroutine (ReloadMyMaterialDelayed (Constants.timeDelayToLoad));
				//ReloadMyMaterial ();
			}
		}
	}
	[SerializeField]
	public bool bootstrapped = false;
	[SerializeField]
	public List<AudioClip> myAudioClips = new List<AudioClip>();

	//used only if the piece is a deck of cards
	[SerializeField]
	public List<GameObject> myPotentialCardPrefabs;

	[SerializeField]
	public Color myStoredColor;

	void Awake(){
		meshFilter = GetComponent<MeshFilter> ();
		audioSource = GetComponent<AudioSource> ();
		defaultNewPieceColor = CategoryInitializer.Instance.storeObjectColor;
	}

	void Start () {
		gameObject.AddComponent<ApplyTransform> ();

		if (myCategory != ObjectCreatorButtons.Player) {
			gameObject.AddComponent<PressGesture> ();
			gameObject.GetComponent<PressGesture> ().Pressed += OnPressed;
		}

	}



	void OnDisable() {
		if (myCategory == ObjectCreatorButtons.Dice) {
			GetComponent<FlickGesture>().Flicked -= DiceFlick;
		}
	}

	void DiceFlick (object sender, System.EventArgs e)
	{
		//Debug.Log ("dice flicked");
		//Debug.Log ("direction: " + GetComponent<FlickGesture> ().Direction.ToString ());

		Vector3 flickDirection = new Vector3(GetComponent<FlickGesture>().ScreenFlickVector.x, 50.0f, GetComponent<FlickGesture>().ScreenFlickVector.y);
		//Debug.Log("vector: " + flickDirection);
		GetComponent<Rigidbody> ().AddForce (flickDirection);
		GetComponent<Rigidbody> ().AddTorque (flickDirection);
	}

	public void Bootstrap() {
		Debug.Log ("Bootstrap() called");
		StartCoroutine (_Bootstrap ());
	}

	private IEnumerator _Bootstrap() {
		yield return StartCoroutine (LoadMesh ());
		yield return StartCoroutine (LoadAudio ());

		if (myType != TypeOfPiece.playerCircle)
			yield return StartCoroutine (AddSaveGameComponents ());

		if (myCategory == ObjectCreatorButtons.Dice) {
			ThisPieceIsADice ();
		} else if (myCategory == ObjectCreatorButtons.Cards) {
			ThisPieceIsADeckOfCards ();
		} else if (myCategory == ObjectCreatorButtons.Player) {
			ThisPieceIsAPlayer ();
		} else {
			NewPieceCreator.RegisterObjectColors (this.gameObject);
		}

		GetComponent<TransformGesture> ().Type = (TouchScript.Gestures.Base.TransformGestureBase.TransformType) 0x3;
		bootstrapped = true;

		myStoredColor = myColor;
		PlayASound ();
	}

	private IEnumerator AddSaveGameComponents() {
		Debug.Log ("adding save game components");
		gameObject.AddComponent<StoreInformation> ();
		transform.GetChild (0).gameObject.AddComponent<StoreInformation> ();
		if (transform.GetChild (0).gameObject.GetComponent<MeshRenderer> () != null) {	//if the child has a mesh renderer, we we need to store its material
			transform.GetChild(0).gameObject.AddComponent<StoreMaterials>();
		}


		foreach (Transform child in transform.GetChild(0)) {
			child.gameObject.AddComponent<StoreInformation> ();
			if (child.gameObject.GetComponent<MeshRenderer> () != null) {
				child.gameObject.AddComponent<StoreMaterials> ();
			}
		}

		yield return null;
	}

	private IEnumerator LoadMesh() {
		GameObject piecePrefab = Resources.Load (myCategory + "/" + myType.ToString (), typeof(GameObject)) as GameObject;
		if (piecePrefab == null) {
			Debug.LogError ("missing resource for: " + myCategory + "/" + myType.ToString ());
			yield break;
		}
		childMeshobject = Instantiate (piecePrefab);
		childMeshobject.transform.SetParent (this.transform);
		childMeshobject.transform.localPosition = Vector3.zero;

		//correct the color on all of the meshes (dice should stay whatever color they are)
		if (myCategory != ObjectCreatorButtons.Dice && myCategory != ObjectCreatorButtons.Cards) {
			myColor = defaultNewPieceColor;

			Material tempMaterial = new Material (Shader.Find ("Standard"));
			//Debug.Log ("made temp material, name is: " + tempMaterial.name);
			tempMaterial.name = "instanceMaterial_" + childMeshobject.name;
			tempMaterial.color = defaultNewPieceColor;
			tempMaterial.shader = Shader.Find ("Standard");

			_myMaterial = tempMaterial;
		

			if (childMeshobject.GetComponent<MeshRenderer> () != null) {
				MeshRenderer tempMesh = childMeshobject.GetComponent<MeshRenderer> ();
				//Debug.Log ("trying to set material for main childmesh object to tempMaterial: " + tempMaterial.name);
				tempMesh.sharedMaterial = tempMaterial;
				//Debug.Log ("did the material get set? : " + tempMesh.sharedMaterial.name);
			}

			for (int i = 0; i < transform.GetChild (0).childCount; i++) {
				if (transform.GetChild (0).GetChild (i).gameObject.GetComponent<MeshRenderer> () != null) {
					MeshRenderer tempMesh = transform.GetChild (0).GetChild (i).gameObject.GetComponent<MeshRenderer> ();
					tempMesh.sharedMaterial = tempMaterial;
				}
			}
		} else if (transform.GetChild(0).gameObject.GetComponent<MeshRenderer>() != null) {
			_myMaterial = transform.GetChild (0).gameObject.GetComponent<MeshRenderer> ().sharedMaterial;
		}
		if (myMaterial != null)
			Debug.Log ("MY MATERIAL SET: " + myMaterial.name);

		if (!twoDimensional) {
			//dont have to set texture
			Debug.Log("this is a 3d object, not setting texture");

		} else {
			Debug.Log ("this is a 2dimensional object, setting texture for child meshes");

			Texture tempTexture = new Texture();
			for (int i = 0; i < transform.childCount; i++) {
				if (transform.GetChild(i).gameObject.GetComponent<MeshRenderer>() != null) {
					MeshRenderer tempMesh = transform.GetChild (i).gameObject.GetComponent<MeshRenderer> ();
					tempMesh.sharedMaterial.mainTexture = tempTexture;
				}
			}
		}
			
		yield return null;
	}

	private IEnumerator LoadAudio() {
		myAudioClips = new List<AudioClip>(Resources.LoadAll (myCategory.ToString() + "/" + myCategory.ToString () + "Sounds", typeof(AudioClip)).Cast<AudioClip>().ToArray());
		if (myAudioClips.Count == 0) {
			Debug.LogError ("no sounds for: " + myCategory + "/" + myType.ToString ());
		}
		yield return null;
	}


	void SetMeshesColorsDelay(Color newColor) {
		StartCoroutine (WaitToSetMeshes (newColor));
	}

	IEnumerator WaitToSetMeshes(Color theColor) {
		yield return new WaitForSeconds (Constants.timeDelayToLoad / 2);
		SetMeshesColors (theColor);
	}

	private IEnumerator ReloadMyMaterialDelayed(float delayTime) {
		yield return new WaitForSeconds (delayTime);
		if (myMaterial == null)
			yield break;

		Debug.Log ("SETTTING MATERIAL TO: " + myMaterial.name);
		if (myCategory == ObjectCreatorButtons.Dice) {
			Debug.Log ("not reloading dice material");
			yield break;
		}

		if (childMeshobject.GetComponent<MeshRenderer> () != null) {
			MeshRenderer tempMesh = childMeshobject.GetComponent<MeshRenderer> ();
			tempMesh.sharedMaterial = myMaterial;
		}

		for (int i = 0; i < transform.GetChild (0).childCount; i++) {
			if (transform.GetChild (0).GetChild (i).gameObject.GetComponent<MeshRenderer> () != null) {
				MeshRenderer tempMesh = transform.GetChild (0).GetChild (i).gameObject.GetComponent<MeshRenderer> ();
				tempMesh.sharedMaterial = myMaterial;
			}
		}
	}

	private void ReloadMyMaterial() {
		//Color tempColor = NewPieceCreator.RetrieveObjectColor (this.gameObject);

		if (myMaterial == null)
			return;
		


		Debug.Log ("SETTTING MATERIAL TO: " + myMaterial.name);
		if (myCategory == ObjectCreatorButtons.Dice) {
			Debug.Log ("not reloading dice material");
			return;
		}
		
		if (childMeshobject.GetComponent<MeshRenderer> () != null) {
			MeshRenderer tempMesh = childMeshobject.GetComponent<MeshRenderer> ();
			tempMesh.sharedMaterial = myMaterial;
		}

		for (int i = 0; i < transform.GetChild (0).childCount; i++) {
			if (transform.GetChild (0).GetChild (i).gameObject.GetComponent<MeshRenderer> () != null) {
				MeshRenderer tempMesh = transform.GetChild (0).GetChild (i).gameObject.GetComponent<MeshRenderer> ();
				tempMesh.sharedMaterial = myMaterial;
			}
		}

		//SetMeshesColors (tempColor);
	}

	void SetMeshesColors(Color newColor) {
		//Debug.Log ("setting all the meshes colors to: " + newColor.ToString ());

		//correct the color on all of the meshes
		if (transform.childCount == 0) return;

		if (transform.GetChild(0).gameObject.GetComponent<MeshRenderer> () != null) {
			transform.GetChild(0).gameObject.GetComponent<MeshRenderer> ().sharedMaterial.color = newColor;
		}

		for (int i = 0; i < transform.GetChild(0).childCount; i++) {
			if (transform.GetChild(0).GetChild(i).gameObject.GetComponent<MeshRenderer>() != null) {
				MeshRenderer tempMesh = transform.GetChild(0).GetChild (i).gameObject.GetComponent<MeshRenderer> ();
				tempMesh.sharedMaterial.color = newColor;		//set the unowned objects color to gray
			}
		}
	}

	void OnCollisionEnter(Collision coll) {
		Debug.Log ("Collided with: " + coll.gameObject.name);
	}

	void OnTriggerEnter(Collider other) {
		Debug.Log ("triggered with: " + other.name);
		if (other.tag == "Player" && myCategory != ObjectCreatorButtons.Player) {
			Debug.Log ("triggered with player");
			Player theOtherPlayer = other.gameObject.GetComponent<Player> ();

			theOtherPlayer.CollidedWithAPiece (this);
		}
	}

	void OnPressed (object sender, EventArgs e)
	{
		PlayASound ();
	}

	public void PlayASound() {
		if (myAudioClips.Count == 0) {
			Debug.LogError ("we have no audioClips for: " + transform.GetChild(0).name);
			return;
		}
		audioSource.volume = SoundManager.Instance.globalSFXVolume;
		audioSource.clip = myAudioClips [UnityEngine.Random.Range (0, myAudioClips.Count - 1)];
		audioSource.Play ();
	}


	//adds flick gesture, rigidbody, limits dragging to 2 finger drags
	public void ThisPieceIsADice() {
		_myColor = Color.white;
		GetComponent<TransformGesture> ().MinTouches = 2;

		gameObject.AddComponent<FlickGesture> ();
		GetComponent<FlickGesture>().Flicked += DiceFlick;

		if (gameObject.GetComponent<Rigidbody>() == null)
			gameObject.AddComponent<Rigidbody> ();
		GetComponent<Rigidbody> ().mass = 0.08f;
		GetComponent<Rigidbody> ().drag = 0.1f;
		GetComponent<Rigidbody> ().angularDrag = 0.01f;

		if (gameObject.GetComponent<BoxCollider>() == null)
			gameObject.AddComponent<BoxCollider> ();
		//gameObject.GetComponent<BoxCollider> ().size = new Vector3 (5.0f, 5.0f, 5.0f);
		gameObject.GetComponent<BoxCollider> ().isTrigger = false;
		gameObject.layer = 8;	//Dice Layer

		gameObject.tag = "Dice";
	}

	//adds double tap gesture to spawn a card of its category
	private void ThisPieceIsADeckOfCards() {
		myPotentialCardPrefabs = new List<GameObject>(Resources.LoadAll (myCategory.ToString() + "/" + myType.ToString () + "Cards", typeof(GameObject)).Cast<GameObject>().ToArray());

		if (gameObject.GetComponent<TapGesture> () == null) {
			gameObject.AddComponent<TapGesture> ();
		}
		gameObject.GetComponent<TapGesture> ().NumberOfTapsRequired = 2;
		gameObject.GetComponent<TapGesture>().Tapped += DeckTapped;

		//if (gameObject.GetComponent<BoxCollider>() == null)
		//	gameObject.AddComponent<BoxCollider> ();
		//gameObject.GetComponent<BoxCollider> ().size = new Vector3 (5.0f, 5.0f, 5.0f);S
	}

	private void ThisPieceIsAPlayer() {



		if (gameObject.GetComponent<Rigidbody>() == null)
			gameObject.AddComponent<Rigidbody> ();
		//GetComponent<Rigidbody> ().mass = 0.08f;
		GetComponent<Rigidbody> ().useGravity = false;
		GetComponent<Rigidbody> ().isKinematic = true;

		GetComponent<BoxCollider> ().size = new Vector3 (30f, 10f, 20f);
		GetComponent<BoxCollider> ().center = new Vector3 (-5f, 0f, 2.5f);
		gameObject.AddComponent<Player> ();
		GetComponent<Player> ().SetMyNameVisually (NewPieceCreator.numplayers++);
		GetComponent<Player> ().myColor = new Color (UnityEngine.Random.Range (0.0f, 1.0f), UnityEngine.Random.Range (0.0f, 1.0f), UnityEngine.Random.Range (0.0f, 1.0f));
		this.gameObject.tag = "Player";
	}

	void DeckTapped (object sender, EventArgs e)
	{
		DrawRandomCardFromDeck ();
	}

	private void DrawRandomCardFromDeck() {
		int randomCardIndex = UnityEngine.Random.Range (0, myPotentialCardPrefabs.Count - 1);
		GameObject theCardPrefab = myPotentialCardPrefabs[randomCardIndex];

		GameObject newCard = Instantiate (theCardPrefab);
		string theDataPath = "Cards/deckCards";

		if (theCardPrefab.name.ToLower ().Contains ("risk")) {
			theDataPath += "_riskCards/" + theCardPrefab.name;
		} else {
			theDataPath += "_playingCards/" + theCardPrefab.name;
		}

		Debug.Log("after drawing a random card, setting the card's data path to: " + theDataPath);


		newCard.GetComponent<Card> ().GiveMeMyDataPath (theDataPath);

		newCard.transform.position = transform.position + Vector3.up * 8;
		if (newCard.name.ToLower ().Contains ("risk")) {
			newCard.transform.localScale = Vector3.one * 10.0f;
		} else {
			newCard.transform.localScale = Vector3.one * 150.0f;
		}
		newCard.GetComponent<Card> ().FlipMeOver ();
	}

}
