using UnityEngine;
using System.Collections;
using TouchScript.Gestures;

public class PreLoadObjects : MonoBehaviour {
	//attach this script to the game manager object in pre-loaded scenes

	//finds the dice on the screen and adds the flick gesture
	void Start () {
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag ("Dice")) {
			obj.GetComponent<Piece> ().ThisPieceIsADice ();
		}
	}
}
