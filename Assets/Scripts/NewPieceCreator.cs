using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;

public class NewPieceCreator : MonoBehaviour {
	public static int numplayers = 1;
	private static Dictionary<string, Material> materialsRegistry = new Dictionary<string, Material>();

	public static void CreateNewPiece(ObjectCreatorButtons buttonName, TypeOfPiece typeToInstantiate, Vector3 location) {
		GameObject newPiece = Instantiate (Resources.Load ("Piece", typeof(GameObject))) as GameObject;
		newPiece.GetComponent<Piece> ().myCategory = buttonName;
		newPiece.GetComponent<Piece> ().myType = typeToInstantiate;
		newPiece.GetComponent<Piece> ().Bootstrap ();
		newPiece.transform.position = location;
	}

	public static void RegisterObjectMaterials(GameObject pieceToRegister) {
		if (pieceToRegister.GetComponent<StoreInformation>() != null && pieceToRegister.GetComponent<Piece>().myMaterial != null)
			materialsRegistry.Add (pieceToRegister.GetComponent<StoreInformation> ().Id, pieceToRegister.GetComponent<Piece> ().myMaterial);
	}

	public static Material RetrieveObjectMaterial(GameObject registeredPiece) {
		if (registeredPiece.GetComponent<StoreInformation>() != null)
			return materialsRegistry [registeredPiece.GetComponent<StoreInformation> ().Id];
	}

}
