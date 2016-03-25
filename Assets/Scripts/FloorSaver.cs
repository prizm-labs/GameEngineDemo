using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FloorSaver : MonoBehaviour {

	[SerializeField]
	public TypeOfPiece theBoardThatsOpen = TypeOfPiece.dice6;

	[SerializeField]
	public bool previouslyLoaded = false;

	[SerializeField]
	private bool _SerializationChecker;
	public bool SerializationChecker {
		get { return _SerializationChecker; }
		set {
			StartCoroutine (ReloadBoardDelayed ());
		}
	}

	private int counter = 0;

	// Use this for initialization
	void Start () {
		//artCoroutine (ReloadBoardDelayed ());
	}

	void Fuck(int thing) {
		if (counter != 1)
			Debug.Log ("fuck");
		else
			Debug.Log ("no fuck");
	}

	IEnumerator ReloadBoardDelayed() {
		Debug.Log ("loading board delayed");

		yield return new WaitForSeconds (Constants.timeDelayToLoad);
		string backgroundName = GetComponent<MeshRenderer> ().sharedMaterial.name.ToLower ();
		Debug.Log (backgroundName);

			if (backgroundName.Contains ("0"))
				theBoardThatsOpen = TypeOfPiece.RiskBoard0;
			else if (backgroundName.Contains ("1"))
				theBoardThatsOpen = TypeOfPiece.RiskBoard1;
			else if (backgroundName.Contains ("2"))
				theBoardThatsOpen = TypeOfPiece.RiskBoard2;
			else if (backgroundName.Contains ("3"))
				theBoardThatsOpen = TypeOfPiece.RiskBoard3;
			else if (backgroundName.Contains ("4"))
				theBoardThatsOpen = TypeOfPiece.RiskBoard4;
			else if (backgroundName.Contains ("5"))
				theBoardThatsOpen = TypeOfPiece.RiskBoard5;
	
			Material boardMaterial = Resources.Load ("LoadBG/" + theBoardThatsOpen.ToString (), typeof(Material)) as Material;
			GetComponent<MeshRenderer> ().sharedMaterial = boardMaterial;

			List<AudioClip> myAudioClips = new List<AudioClip>(Resources.LoadAll ("LoadBG/LoadBGSounds", typeof(AudioClip)).Cast<AudioClip>().ToArray());
			if (myAudioClips.Count == 0) {
				Debug.LogError ("no sounds for loading BG");
			}
			AudioSource.PlayClipAtPoint (myAudioClips [UnityEngine.Random.Range (0, myAudioClips.Count - 1)], SoundManager.Instance.mainCameraLocation, SoundManager.Instance.globalSFXVolume);


		previouslyLoaded = true;
	}

}
