using UnityEngine;
using System;
using UnityEngine.UI;

public class SaveLoad : MonoBehaviour {
	[SerializeField]
	private bool paused = false;
	[SerializeField]
	private GUITexture pausedGUI;
	[SerializeField]
	private string gameName = "Your Game";
	[SerializeField]
	private static bool logProgress = false;

	private GameObject LoadButtonPrefab;
	private GameObject SaveLoadContent;

	void Awake(){
		LoadButtonPrefab = Resources.Load ("GameStateButton/Pnl_GameState", typeof(GameObject)) as GameObject;
		SaveLoadContent = GameObject.Find ("MainCanvas/Pnl_SaveLoad/ScrollView/ScrollRect/Content");
	}


	public void RefreshLoadGameButtons(){
		foreach (Transform t in SaveLoadContent.transform) {
			Destroy (t.gameObject);
		}
		for (int i = 0; i < LevelSerializer.SavedGames [LevelSerializer.PlayerName].Count; i++ ){
			CreateNewButton (LevelSerializer.SavedGames [LevelSerializer.PlayerName] [i], LevelSerializer.SavedGames [LevelSerializer.PlayerName].Count -1 - i);

		}/*
		foreach (LevelSerializer.SaveEntry sg in LevelSerializer.SavedGames[LevelSerializer.PlayerName]) {
			CreateNewButton (sg);
		}*/
	}



	private void CreateNewButton(LevelSerializer.SaveEntry sg, int index){
		GameObject newButton = Instantiate (LoadButtonPrefab) as GameObject;
		SaveLoadButton newButtonScript = newButton.GetComponent<SaveLoadButton> ();
		newButton.SetParent (SaveLoadContent);
		newButtonScript.SetSavedGameCaption (sg.Caption);
		newButtonScript.setSavedGameNumber (index);
		newButtonScript.LoadGameButton.onClick.AddListener(() => LoadGame(sg));
		newButtonScript.DeleteGameSavedButton.onClick.AddListener (() => DeleteGameState(sg));
	}


	public void LoadGame(LevelSerializer.SaveEntry sg){
		LevelSerializer.LoadNow (sg.Data);
	}

	public void DeleteGameState(LevelSerializer.SaveEntry sg){
		sg.Delete ();
		//LevelSerializer.SavedGames [LevelSerializer.PlayerName].Remove (sg);
		RefreshLoadGameButtons();
	}


	public void SaveCurrentGame(){
		LevelSerializer.SaveGame(gameName);
		RefreshLoadGameButtons ();
	}


}
