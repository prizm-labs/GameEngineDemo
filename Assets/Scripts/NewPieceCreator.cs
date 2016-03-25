using UnityEngine;
using System.Collections;
using TouchScript.Gestures;

public class NewPieceCreator : MonoBehaviour {
	public static int numplayers = 1;

	public static void CreateNewPiece(ObjectCreatorButtons buttonName, TypeOfPiece typeToInstantiate, Vector3 location) {
		GameObject newPiece = Instantiate (Resources.Load ("Piece", typeof(GameObject))) as GameObject;
		newPiece.GetComponent<Piece> ().myCategory = buttonName;
		newPiece.GetComponent<Piece> ().myType = typeToInstantiate;
		newPiece.GetComponent<Piece> ().Bootstrap ();
		newPiece.transform.position = location;
	}

}
