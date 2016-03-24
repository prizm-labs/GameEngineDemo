using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SwapSprites : MonoBehaviour {

	public Sprite spriteToSwapWith;
	private Sprite theOriginal;

	private bool swapped = false;
	public void SwapTheSprites() {
		if (!swapped) {
			theOriginal = GetComponent<Image> ().sprite;
			GetComponent<Image> ().sprite = spriteToSwapWith;
		} else {
			GetComponent<Image> ().sprite = theOriginal;
		}

		swapped = !swapped;
	}
}
