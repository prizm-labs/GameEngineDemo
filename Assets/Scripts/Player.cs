using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	private Color _myColor;
	public Color myColor {
		get { return _myColor;}
		set {
			_myColor = value;
		}
	}

	public List<Piece> myOwnedPieces;

	public int myNumSoldiers;
	public int myNumCalvalry;
	public int myNumCannons;

	//should also update the player's displayer
	void UpdateMyDockCard() {
		myNumSoldiers = CountMyOwnedPieces (TypeOfPiece.soldier);
		myNumCalvalry = CountMyOwnedPieces (TypeOfPiece.cavalry);
		myNumCannons = CountMyOwnedPieces (TypeOfPiece.cannon);
	}

	//helper function to determine how many of a piece a player has
	int CountMyOwnedPieces(TypeOfPiece typeToCount) {
		int amnt = 0;

		foreach (Piece peace in myOwnedPieces) {
			if (peace.myType == typeToCount) {
				amnt++;
			}
		}

		return amnt;
	}
}
