using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;

[System.Serializable]
public class NewPieceCreator : MonoBehaviour {
	public static int numplayers = 1;

	private static Dictionary<string, Color> colorsRegistry = new Dictionary<string, Color>();

	public static void CreateNewPiece(ObjectCreatorButtons buttonName, TypeOfPiece typeToInstantiate, Vector3 location) {
		GameObject newPiece = Instantiate (Resources.Load ("Piece", typeof(GameObject))) as GameObject;
		newPiece.GetComponent<Piece> ().myCategory = buttonName;
		newPiece.GetComponent<Piece> ().myType = typeToInstantiate;
		newPiece.GetComponent<Piece> ().Bootstrap ();
		newPiece.transform.position = location;
	}

	public static void RegisterObjectColors(GameObject pieceToRegister) {
		if (pieceToRegister.GetComponent<StoreInformation>() != null && pieceToRegister.GetComponent<Piece>().myColor != null)
			colorsRegistry.Add (pieceToRegister.GetComponent<StoreInformation> ().Id, pieceToRegister.GetComponent<Piece> ().myColor);
	}

	public static Color RetrieveObjectColor(GameObject registeredPiece) {
		if (registeredPiece.GetComponent<StoreInformation> () != null)
			return colorsRegistry [registeredPiece.GetComponent<StoreInformation> ().Id];
		else
			return Color.black;
	}

}
