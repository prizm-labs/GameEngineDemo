using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SaveLoadButton : MonoBehaviour {
	public Text SavedGameNumber;

	public Text SavedGameCaption;
	public Button LoadGameButton;
	public Button DeleteGameSavedButton;

	// Use this for initialization
	void Awake () {
		FindMyStuff ();
	}

	private void FindMyStuff(){
		var allChildren = gameObject.GetComponentsInChildren<Transform> (true);
		foreach (Transform t in allChildren) {
			if (t.name == "Number")
				SavedGameNumber = t.GetComponent<Text>();
			if (t.name == "Caption")
				SavedGameCaption = t.GetComponent<Text>();
			if (t.name == "Btn_Gamestate")
				LoadGameButton = t.GetComponent<Button> ();
			if (t.name == "Btn_DeleteGameState")
				DeleteGameSavedButton = t.GetComponent<Button> ();
		}
	}

	public void setSavedGameNumber(int index){
		SavedGameNumber.text = index + "";
	}

	public void SetSavedGameCaption(string caption){
		SavedGameCaption.text = caption;
	}
}
