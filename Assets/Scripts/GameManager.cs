using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {


	private Camera mainCamera;

	[System.NonSerialized]
	public static float DistanceFromCamera;
	private float BoundariesHeight = 5000.0f;

	// Use this for initialization
	void Start () {
		mainCamera = GameObject.Find ("Main Camera").GetComponent<Camera> ();

		DistanceFromCamera = (mainCamera.gameObject.transform.position.y - GameObject.Find ("Floor").transform.position.y) * 0.3f;
		//Debug.Log ("distance from camera is set at: " + DistanceFromCamera.ToString ());

		CreateBoundariesDice ();
	}


	//creates walls so balls can't escape world
	public void CreateBoundariesDice() {
		List<GameObject> boundaries = new List<GameObject> ();

		Vector3 lowerLeft = mainCamera.ViewportToWorldPoint (new Vector3 (0.05f, 0.05f, DistanceFromCamera));
		Vector3 lowerRight = mainCamera.ViewportToWorldPoint (new Vector3 (0.95f, 0.05f, DistanceFromCamera));
		Vector3 upperLeft = mainCamera.ViewportToWorldPoint (new Vector3 (0.05f, 0.95f, DistanceFromCamera));
		Vector3 upperRight = mainCamera.ViewportToWorldPoint (new Vector3 (0.95f, 0.95f, DistanceFromCamera));

		float width = lowerRight.x - lowerLeft.x;
		float height = upperRight.z - lowerRight.z; 


		Vector3 bottom = (lowerLeft + lowerRight ) / 2;
		Vector3 top = (upperLeft + upperRight ) / 2;
		Vector3 left = (upperLeft + lowerLeft ) / 2;
		Vector3 right = (lowerRight + upperRight ) / 2;


		GameObject bottomBound = GameObject.CreatePrimitive(PrimitiveType.Cube);
		bottomBound.transform.position = bottom;
		bottomBound.transform.localScale = new Vector3 (width, BoundariesHeight, 5f);

		GameObject topBound = GameObject.CreatePrimitive(PrimitiveType.Cube);
		topBound.transform.position = top;
		topBound.transform.localScale = new Vector3 (width, BoundariesHeight, 5f);

		GameObject leftBound = GameObject.CreatePrimitive(PrimitiveType.Cube);
		leftBound.transform.position = left;
		leftBound.transform.localScale = new Vector3 (5f, BoundariesHeight, height);

		GameObject rightBound = GameObject.CreatePrimitive(PrimitiveType.Cube);
		rightBound.transform.position = right;
		rightBound.transform.localScale = new Vector3 (5f, BoundariesHeight, height);

		boundaries.Add (bottomBound);
		boundaries.Add (topBound);
		boundaries.Add (leftBound);
		boundaries.Add (rightBound);

		foreach (GameObject bond in boundaries) {
			bond.AddComponent<Rigidbody> ();
			bond.GetComponent<Rigidbody> ().useGravity = false;
			bond.GetComponent<Rigidbody> ().isKinematic = true;
			bond.name = "diceBoundary";
		}

		bottomBound.layer = 8;	//Dice layer
		topBound.layer = 8;	//Dice layer
		leftBound.layer = 8;	//Dice layer
		rightBound.layer = 8;	//Dice layer

		//make boundaries invisible
		Destroy(bottomBound.GetComponent<MeshRenderer>());
		Destroy(bottomBound.GetComponent<MeshCollider>());

		Destroy(topBound.GetComponent<MeshRenderer>());
		Destroy(topBound.GetComponent<MeshCollider>());

		Destroy(leftBound.GetComponent<MeshRenderer>());
		Destroy(leftBound.GetComponent<MeshCollider>());

		Destroy(rightBound.GetComponent<MeshRenderer>());
		Destroy(rightBound.GetComponent<MeshCollider>());
	}
}
