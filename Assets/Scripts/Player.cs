using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Player : MonoBehaviour {

	private Color _myColor;
	public Color myColor {
		get { return _myColor;}
		set {
			_myColor = value;
			SetPlayersColorVisually ();
		}
	}

	public List<Piece> myOwnedPieces = new List<Piece>();
	public List<Card> myOwnedCards = new List<Card>();

	public int myNumSoldiers;
	public int myNumCavalry;
	public int myNumCannons;

	private GameObject panelPrefab;

	private GameObject mySoldiersPanel;
	private GameObject myCannonsPanel;
	private GameObject myCavalryPanel;

	private Transform OrganizerTransform;

	void Awake() {
		OrganizerTransform = transform.GetChild(0).Find ("Canvas").Find ("Pnl_Background").Find ("Organizer");
		panelPrefab = Resources.Load ("RiskSprites/panelPrefab", typeof(GameObject)) as GameObject;
	}

	//should also update the player's displayer
	void UpdateMyDockCard() {
		if (mySoldiersPanel != null) {
			myNumSoldiers = CountMyOwnedPieces (TypeOfPiece.soldier);
			SetNumberForPanel (mySoldiersPanel, myNumSoldiers);
		}
		if (myCavalryPanel != null) {
			myNumCavalry = CountMyOwnedPieces (TypeOfPiece.cavalry);
			SetNumberForPanel (myCavalryPanel, myNumCavalry);
		}
		if (myCannonsPanel != null) {
			myNumCannons = CountMyOwnedPieces (TypeOfPiece.cannon);
			SetNumberForPanel (myCannonsPanel, myNumCannons);
		}
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

	public void CollidedWithAPiece(Piece newPiece) {
		if (!myOwnedPieces.Contains (newPiece)) {
			myOwnedPieces.Add (newPiece);
		}

		newPiece.myColor = this.myColor;
	
		if (newPiece.myNewOwner != null && newPiece.myNewOwner != this) {
			newPiece.myNewOwner.myOwnedPieces.Remove (newPiece);
			newPiece.myNewOwner.UpdateMyDockCard ();
		}

		newPiece.myNewOwner = this;

		if (mySoldiersPanel == null && newPiece.myType == TypeOfPiece.soldier){
			CreateNewPanel (TypeOfPiece.soldier);
		}
		else if (myCavalryPanel == null && newPiece.myType == TypeOfPiece.cavalry){
			CreateNewPanel (TypeOfPiece.cavalry);
		}
		else if (myCannonsPanel == null && newPiece.myType == TypeOfPiece.cannon){
			CreateNewPanel (TypeOfPiece.cannon);
		}

		UpdateMyDockCard ();
	}

	public void SetMyNameVisually(int playersNum) {
		transform.GetChild(0).Find ("Canvas").Find ("Pnl_Name").Find ("text_playersName").gameObject.GetComponent<Text> ().text = "Player " + playersNum.ToString ();
	}

	private void SetNumberForPanel(GameObject thePanel, int number) {
		thePanel.transform.Find ("Text").gameObject.GetComponent<Text> ().text = number.ToString ();
	}

	private void SetPlayersColorVisually() {
		transform.GetChild (0).gameObject.GetComponent<ParticleSystemRenderer> ().material.color = _myColor;
	}

	private void CreateNewPanel(TypeOfPiece typeOfPanel) {
		GameObject tempPanelRef = Instantiate (panelPrefab);
		tempPanelRef.transform.SetParent (OrganizerTransform);
		tempPanelRef.transform.localScale = Vector3.one * 1.5f;
		tempPanelRef.transform.localPosition = Vector3.zero;
		tempPanelRef.transform.localEulerAngles = Vector3.zero;
		tempPanelRef.transform.Find ("Image").gameObject.GetComponent<Image> ().sprite = Resources.Load ("RiskSprites/" + typeOfPanel.ToString (), typeof(Sprite)) as Sprite;

		switch (typeOfPanel) {
		case TypeOfPiece.soldier:
			mySoldiersPanel = tempPanelRef;
			break;
		case TypeOfPiece.cannon:
			myCannonsPanel = tempPanelRef;
			break;
		case TypeOfPiece.cavalry:
			myCavalryPanel = tempPanelRef;
			break;
		default:
			Debug.Log ("tried to create new panel for something that wasn't in risk");
			break;
		}
	}
}
