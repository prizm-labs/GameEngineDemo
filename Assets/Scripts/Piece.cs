using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;
using System.Linq;
using System;

public class Piece : MonoBehaviour {

	private TypeOfPiece _myType;
	public TypeOfPiece myType {
		get { return _myType; }
		set 
		{
			_myType = value; 
			if (_myType == TypeOfPiece.sprite2d) {
				meshFilter.mesh = Helper.GetQuadMesh ();
			} else if (_myType == TypeOfPiece.playerCircle) {
				gameObject.AddComponent<Player> ();
			} 
		}
	}
		

	public static Color defaultNewPieceColor = Color.white;

	private ObjectCreatorButtons _myCategory;
	public ObjectCreatorButtons myCategory {
		get { return _myCategory; }
		set 
		{
			_myCategory = value; 

			if (bootstrapped && myCategory == ObjectCreatorButtons.Dice) {
				ThisPieceIsADice ();
			}
		}
	}

		
	private Location _myLocation;
	public Location myLocation {
		get { return _myLocation; }
		set 
		{
			_myLocation = value; 
		}
	}

	private Color _myColor;
	public Color myColor {
		get { return _myColor;}
		set {
			_myColor = value;
			Debug.Log ("setting color of " + gameObject.name + " to: " + _myColor.ToString());
			Debug.Log ("ps, we are bootstrapped: " + bootstrapped.ToString ());

			//this code runs if the piece was created by the save game manager (already was bootstrapped by DraMe)
			if (bootstrapped) {
				SetMeshesColorsDelay (_myColor);
				if (GetComponent<TransformGesture> () == null) {
					gameObject.AddComponent<TransformGesture> ();
					if (GetComponent<ApplyTransform> () != null) {
						gameObject.GetComponent<ApplyTransform> ().ReloadApplyTransform ();

					}
				}
				if (myType.ToString ().ToLower ().Contains ("dice") || myType.ToString().ToLower().Contains("cards")) {
					Debug.Log ("this piece is either a dice or a card, and was recovered from when saving.  Going to destroy it and create a new one");
					NewPieceCreator.CreateNewPiece (myCategory, myType, transform.position);
					Destroy (this.gameObject);
				}
			}
		}
	}
		

	public bool twoDimensional = false;
	public AudioSource audioSource;
	public MeshFilter meshFilter;
	public GameObject childMeshobject;

	private Material _myMaterial;
	public Material myMaterial{
		get { return _myMaterial; }
		set 
		{
			Debug.Log ("an outside source tried f*cking with my material, going to reload it just incase");
			if (bootstrapped) {
				Debug.Log ("this thing was recovered from when saving, reloading its material");
				ReloadMyMaterial ();
			}
		}

	}

	public bool bootstrapped = false;

	public List<AudioClip> myAudioClips = new List<AudioClip>();

	//used only if the piece is a deck of cards
	public List<GameObject> myPotentialCardPrefabs;

	public Color myStoredColor;

	void Awake(){
		meshFilter = GetComponent<MeshFilter> ();
		audioSource = GetComponent<AudioSource> ();
		defaultNewPieceColor = CategoryInitializer.Instance.storeObjectColor;
	}

	void Start () {
		gameObject.AddComponent<ApplyTransform> ();
		myLocation = Location.onBoard;
	}


	void OnDisable() {
		if (myCategory == ObjectCreatorButtons.Dice) {
			GetComponent<FlickGesture>().Flicked -= DiceFlick;
		}
	}

	void DiceFlick (object sender, System.EventArgs e)
	{
		Debug.Log ("dice flicked");
		//Debug.Log ("direction: " + GetComponent<FlickGesture> ().Direction.ToString ());

		Vector3 flickDirection = new Vector3(GetComponent<FlickGesture>().ScreenFlickVector.x, 50.0f, GetComponent<FlickGesture>().ScreenFlickVector.y);
		Debug.Log("vector: " + flickDirection);
		GetComponent<Rigidbody> ().AddForce (flickDirection);
		GetComponent<Rigidbody> ().AddTorque (flickDirection);
	}

	public void Bootstrap() {
		StartCoroutine (_Bootstrap ());
	}

	private IEnumerator _Bootstrap() {
		yield return StartCoroutine (LoadMesh ());
		yield return StartCoroutine (LoadAudio ());
		yield return StartCoroutine (AddSaveGameComponents ());

		if (myCategory == ObjectCreatorButtons.Dice) {
			ThisPieceIsADice ();
		} else if (myCategory == ObjectCreatorButtons.Cards) {
			ThisPieceIsADeckOfCards ();
		}
		bootstrapped = true;

		myStoredColor = myColor;
		Debug.Log ("_Bootstrap() completed!");
		//SetMeshesColorsDelay (Color.cyan);
	}

	private IEnumerator AddSaveGameComponents() {
		
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
		Debug.Log ("loading meshrenderer from resources folder");

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
			Debug.Log ("made temp material, name is: " + tempMaterial.name);
			tempMaterial.name = "instanceMaterial_" + childMeshobject.name;
			tempMaterial.color = defaultNewPieceColor;
			tempMaterial.shader = Shader.Find ("Standard");

			_myMaterial = tempMaterial;
		

			if (childMeshobject.GetComponent<MeshRenderer> () != null) {
				MeshRenderer tempMesh = childMeshobject.GetComponent<MeshRenderer> ();
				Debug.Log ("trying to set material for main childmesh object to tempMaterial: " + tempMaterial.name);
				tempMesh.sharedMaterial = tempMaterial;
				Debug.Log ("did the material get set? : " + tempMesh.sharedMaterial.name);
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
		Debug.Log ("now going to try to load audio");


		myAudioClips = new List<AudioClip>(Resources.LoadAll (myCategory.ToString() + "/" + myCategory.ToString () + "Sounds", typeof(AudioClip)).Cast<AudioClip>().ToArray());
		if (myAudioClips.Count == 0) {
			Debug.LogError ("no sounds for: " + myCategory + "/" + myType.ToString ());
		}
		Debug.Log ("sounds Loaded");
		yield return null;
	}

	void SetMeshesColorsDelay(Color newColor) {
		StartCoroutine (WaitToSetMeshes (newColor));
	}

	IEnumerator WaitToSetMeshes(Color theColor) {
		yield return new WaitForSeconds (0.2f);
		SetMeshesColors (theColor);
	}

	private void ReloadMyMaterial() {
		if (myMaterial == null)
			return;

		Debug.Log ("SETTTING MATERIAL TO: " + myMaterial.name);
		if (myCategory == ObjectCreatorButtons.Dice) {
			Debug.Log ("reloading material from dicematerialHolder");
			ReloadDiceMaterials ();
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
	}
	void SetMeshesColors(Color newColor) {
		Debug.Log ("setting all the meshes colors to: " + newColor.ToString ());

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
		if (other.tag == "player") {
			Debug.Log ("triggered with player, setting our color to their color");
			Player myNewOwner = other.gameObject.GetComponent<Player> ();
			myColor = myNewOwner.myColor;
			if (!myNewOwner.myOwnedPieces.Contains (this)) {
				myNewOwner.myOwnedPieces.Add (this);
			}
		} else if (other.tag == "drawer") {
			transform.SetParent (other.transform, false);
			myLocation = Location.inDrawer;
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.tag == "drawer") {
			//when the piece leaves the drawer, put its location back to onboard
			transform.SetParent(null);
			myLocation = Location.onBoard;
		}
	}

	public void PlayASound() {
		if (myAudioClips.Count == 0) {
			Debug.LogError ("we have no audioClips for: " + name);
			return;
		}

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


	private void ReloadDiceMaterials() {
		Material theDiceMaterial = DiceMaterialHolder.Instance.blueDiceMaterial;
		//theDiceMaterial.mainTexture = DiceMaterialHolder.Instance.blueDiceTexture;
		//theDiceMaterial.shader.

		if (childMeshobject.GetComponent<MeshRenderer> () != null) {
			MeshRenderer tempMesh = childMeshobject.GetComponent<MeshRenderer> ();
			tempMesh.sharedMaterial = theDiceMaterial;
		}
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

	void DeckTapped (object sender, EventArgs e)
	{
		DrawRandomCardFromDeck ();
	}

	private void DrawRandomCardFromDeck() {
		int randomCardIndex = UnityEngine.Random.Range (0, myPotentialCardPrefabs.Count - 1);
		GameObject theCardPrefab = myPotentialCardPrefabs[randomCardIndex];

		GameObject newCard = Instantiate (theCardPrefab);
		newCard.GetComponent<Card> ()._myDataPath = "Cards/deckCards_riskCards/risk_card_argentina";	//fix this

		newCard.transform.position = transform.position + Vector3.up * 8;
		if (newCard.name.ToLower ().Contains ("risk")) {
			newCard.transform.localScale = Vector3.one * 10.0f;
		} else {
			newCard.transform.localScale = Vector3.one * 150.0f;
		}
		newCard.GetComponent<Card> ().FlipMeOver ();
	}

}
