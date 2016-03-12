using UnityEngine;
using System.Collections;
using TouchScript.Gestures;

public class DraggableInstantiator : MonoBehaviour {

	//attach this script to a image that you can drag, when you release it, 
	//it will destroy itself and instantiate a piece of the corresponding type

	public TypeOfPiece typeToInstantiate;


	void Start () {
		gameObject.AddComponent<ApplyTransform> ();
	}

	void OnEnable() {
		GetComponent<ReleaseGesture>().Released += HandleOnRelease;
	}

	void OnDisable() {
		GetComponent<ReleaseGesture>().Released -= HandleOnRelease;
	}

	void HandleOnRelease (object sender, System.EventArgs e)
	{
		Debug.Log ("released, instantiating new object");

		GameObject newPiece = Instantiate (Resources.Load ("Piece", typeof(GameObject))) as GameObject;
		newPiece.GetComponent<Piece> ().myType = typeToInstantiate;
		newPiece.GetComponent<Piece> ().Bootstrap ();
	}

}
