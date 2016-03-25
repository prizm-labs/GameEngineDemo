using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour {

	public static SoundManager Instance;


	[System.NonSerialized]
	public Vector3 mainCameraLocation = new Vector3();
	[System.NonSerialized]
	public float globalSFXVolume = 1.0f;
	[System.NonSerialized]
	private float _BGMVolume = 1.0f;
	public float BGMVolume {
		get { return _BGMVolume; }
		set {
			_BGMVolume = value;
			BGMPlayer.volume = _BGMVolume;
		}
	}

	[System.NonSerialized]
	public List<AudioClip> backgroundMusicClips = new List<AudioClip>();

	private Text trackNameText;

	private bool BGMuted = false;
	private bool SFXMuted = false;

	private AudioSource BGMPlayer;
	private int trackIndex = 0;

	void Awake () {
		Instance = this;
		mainCameraLocation = GameObject.Find ("Main Camera").transform.position;
		BGMPlayer = GetComponent<AudioSource> ();
		trackNameText = GameObject.Find ("MainCanvas/Pnl_Sound/Pnl_Items/Pnl_BGM/Text_NameOfTrack").GetComponent<Text> ();
	}

	void Start() {
		LoadBGMusics ();
		PlayTheTrack ();
	}

	void LoadBGMusics() {
		backgroundMusicClips = new List<AudioClip>(Resources.LoadAll ("BGMusic", typeof(AudioClip)).Cast<AudioClip>().ToArray());
		ShuffleSoundTracks ();
	}

	void ShuffleSoundTracks() {
		for (int i = 0; i < backgroundMusicClips.Count; i++) {
			AudioClip temp = backgroundMusicClips [i];
			int randomIndex = UnityEngine.Random.Range (i, backgroundMusicClips.Count);
			backgroundMusicClips [i] = backgroundMusicClips [randomIndex];
			backgroundMusicClips [randomIndex] = temp;
		}
	}

	public void PlayNextTrack() {
		trackIndex++;
		if (trackIndex > backgroundMusicClips.Count - 1) {
			trackIndex = 0;
		}

		PlayTheTrack ();
	}
		
	public void PlayPreviousTrack() {
		trackIndex--;
		if (trackIndex < 0) {
			trackIndex = backgroundMusicClips.Count - 1;
		}
		PlayTheTrack ();
	}

	public void PlayTheTrack() {
		BGMPlayer.Stop ();
		BGMPlayer.volume = BGMVolume;
		BGMPlayer.clip = backgroundMusicClips [trackIndex];
		trackNameText.text = backgroundMusicClips [trackIndex].name;
		BGMPlayer.Play ();
	}

	public void MuteSFX() {
		if (SFXMuted) {
			//unmute it
			globalSFXVolume = 1.0f;
		} else {
			//mute it
			globalSFXVolume = 0.0f;
		}

		SFXMuted = !SFXMuted;
	}

	public void MuteBG() {
		if (BGMuted) {
			//unmute it
			BGMVolume = 1.0f;
			PlayTheTrack ();
		} else {
			//mute it
			BGMVolume = 0.0f;
			BGMPlayer.Stop ();
		}

		BGMuted = !BGMuted;
	}

	public void ChangeVolume_BG(float value) {
		BGMVolume = value / 100.0f;
	}

	public void ChangeVolume_SFX(float value) {
		globalSFXVolume = value / 100.0f;
	}
}
