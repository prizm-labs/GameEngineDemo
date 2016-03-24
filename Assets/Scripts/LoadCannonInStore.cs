using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoadCannonInStore : MonoBehaviour {

	void Start () {
		GetComponent<RawImage>().texture = CategoryInitializer.Instance.GetCannonRenderTexture ();
	}
}
