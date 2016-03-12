using UnityEngine;
using System.Collections;

public class Piece : MonoBehaviour {

	//load mesh renderer
	//if 2d object, load quad + image


	private TypeOfPiece _myType;
	public TypeOfPiece myType {
		get { return _myType; }
		set { _myType = value; }
	}

	private MeshRenderer meshRender;

	// Use this for initialization
	void Start () {
		gameObject.AddComponent<ApplyTransform> ();
	}

	public void Bootstrap() {
		StartCoroutine (_Bootstrap ());
	}

	private IEnumerator _Bootstrap() {
		Debug.Log ("loading from streaming assets");

		WWW data = new WWW (Application.streamingAssetsPath + "/" + myType.ToString ());
		yield return data;

		if (string.IsNullOrEmpty (data.error)) {
			Debug.LogError ("error loading: " + myType.ToString ());
		}

		Debug.Log ("out of _Bootstrap()");

	}

}
