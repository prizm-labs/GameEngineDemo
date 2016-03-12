using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
			meshRender.material.color = _myColor;
		}
	}


	private bool twoDimensional = false;
	private AudioSource audioSource;
	private MeshFilter meshFilter;
	private MeshRenderer meshRender;
	private List<AudioClip> myAudioClips = new List<AudioClip>();



	void Awake() {
		meshFilter = GetComponent<MeshFilter> ();
		meshRender = GetComponent<MeshRenderer> ();
		audioSource = GetComponent<AudioSource> ();
	}

	void Start () {
		gameObject.AddComponent<ApplyTransform> ();
		myLocation = Location.onBoard;
	}

	public void Bootstrap() {
		StartCoroutine (_Bootstrap ());
	}

	private IEnumerator _Bootstrap() {
		yield return StartCoroutine (LoadMesh ());
		yield return StartCoroutine (LoadAudio ());
	}
	private IEnumerator LoadMesh() {
		Debug.Log ("loading from streaming assets");

		//try to load mesh
		if (!twoDimensional) {
			WWW data = new WWW (Application.streamingAssetsPath + "/" + myCategory + "/" + myType.ToString ());
			yield return data;
			if (string.IsNullOrEmpty (data.error)) {
				Debug.LogError ("error loading: " + myType.ToString ());
			}

			meshRender.material = new Material (Shader.Find("Standard"));
			meshRender.material.color = Color.gray;		//set the unowned objects color to gray
		
		} else {
			//if we are dealing with a 2d object like card, 
			//assign new material,
			//give that material a sprite

			Texture tempTexture = new Texture();
			//load the texture from streaming assets folder

			Debug.Log ("going to load 2 dimensional material to show");
			meshRender.material = new Material (Shader.Find ("Standard"));
			meshRender.material.mainTexture = tempTexture;

		}

		Debug.Log ("_Bootstrap() Completed!");
	}

	private IEnumerator LoadAudio() {
		Debug.Log ("now going to try to load audio");

		//maybe not use streaming assets for this
		WWW data = new WWW (Application.streamingAssetsPath + "/" + myCategory + "/" + myType.ToString () + "Sounds");
		yield return data;
		if (string.IsNullOrEmpty (data.error)) {
			Debug.LogError ("error loading: " + myType.ToString ());
		}

		Debug.Log ("sounds Loaded");
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

		audioSource.clip = myAudioClips [Random.Range (0, myAudioClips.Count - 1)];
		audioSource.Play ();
	}

}
