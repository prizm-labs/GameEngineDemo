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
			Debug.Log ("setting color of material to: " + _myColor.ToString());
			SetMeshesColors (_myColor);
		}
	}
		

	private bool twoDimensional = false;
	private AudioSource audioSource;
	private MeshFilter meshFilter;
	private GameObject childMeshobject;
	private Material myMaterial;

	private bool bootstrapped = false;

	public List<AudioClip> myAudioClips = new List<AudioClip>();



	void Awake() {
		meshFilter = GetComponent<MeshFilter> ();
		audioSource = GetComponent<AudioSource> ();
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

		if (myCategory == ObjectCreatorButtons.Dice) {
			ThisPieceIsADice ();
		}
		bootstrapped = true;
		Debug.Log ("_Bootstrap() completed!");
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
		if (myCategory != ObjectCreatorButtons.Dice) {
			for (int i = 0; i < transform.GetChild(0).childCount; i++) {
				if (transform.GetChild(0).GetChild (i).gameObject.GetComponent<MeshRenderer> () != null) {
					MeshRenderer tempMesh = transform.GetChild(0).GetChild (i).gameObject.GetComponent<MeshRenderer> ();
					tempMesh.materials [0] = new Material (Shader.Find ("Standard"));
					tempMesh.materials [0].name = "fuck";
					tempMesh.materials [0].color = Color.blue;		//set the unowned objects color to gray
				}
			}
		}

		if (!twoDimensional) {
			//dont have to set texture
			Debug.Log("this is a 3d object, not setting texture");

		} else {
			Debug.Log ("this is a 2dimensional object, setting texture for child meshes");

			Texture tempTexture = new Texture();
			for (int i = 0; i < transform.childCount; i++) {
				if (transform.GetChild(i).gameObject.GetComponent<MeshRenderer>() != null) {
					MeshRenderer tempMesh = transform.GetChild (i).gameObject.GetComponent<MeshRenderer> ();
					tempMesh.material.mainTexture = tempTexture;
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

	void SetMeshesColors(Color newColor) {
		Debug.Log ("setting all the meshes colors to: " + newColor.ToString ());

		//correct the color on all of the meshes
		for (int i = 0; i < transform.childCount; i++) {
			if (transform.GetChild(i).gameObject.GetComponent<MeshRenderer>() != null) {
				MeshRenderer tempMesh = transform.GetChild (i).gameObject.GetComponent<MeshRenderer> ();
				//tempMesh.material = new Material (Shader.Find ("Standard"));
				tempMesh.material.color = newColor;		//set the unowned objects color to gray
			}
		}
	}

	void OnTriggerEnter(Collider other) {
		Debug.Log ("collided with: " + other.name);
		if (other.tag == "player") {
			Debug.Log ("collided with player, setting our color to their color");
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
		myColor = Color.white;
		GetComponent<TransformGesture> ().MinTouches = 2;

		gameObject.AddComponent<FlickGesture> ();
		GetComponent<FlickGesture>().Flicked += DiceFlick;

		if (gameObject.GetComponent<Rigidbody>() == null)
			gameObject.AddComponent<Rigidbody> ();
		GetComponent<Rigidbody> ().mass = 0.1f;
		GetComponent<Rigidbody> ().angularDrag = 0.8f;

		if (gameObject.GetComponent<BoxCollider>() == null)
			gameObject.AddComponent<BoxCollider> ();
		gameObject.GetComponent<BoxCollider> ().size = new Vector3 (5.0f, 5.0f, 5.0f);
		gameObject.layer = 8;	//Dice Layer

		gameObject.tag = "Dice";
	}

}
