using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CategoryPopulator : MonoBehaviour {

	public ObjectCreatorButtons typeToPopulate;
	public GameObject picturePrefab;
	Transform contentTransform;

	// Use this for initialization
	void Awake () {
		contentTransform = transform.Find ("Content");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			Debug.Log ("trying to load a renderTexture");
			Debug.Log("this is the length of the category we're dealing with: " + CategoryInitializer.Instance.allCachedTexturesList[typeToPopulate.ToString()].Count.ToString());
			Debug.Log("this is the name" + CategoryInitializer.Instance.allCachedTexturesList[typeToPopulate.ToString()][0].name);
			MakePicture ();
		}
	}

	//add Dragme, adds buttonName, type to instantiate

	void MakePicture() {
		GameObject rawImage = Instantiate (picturePrefab);
		rawImage.transform.SetParent (this.transform);
		rawImage.transform.localPosition = Vector3.zero;

		rawImage.GetComponent<RawImage> ().texture = CategoryInitializer.Instance.allCachedTexturesList [typeToPopulate.ToString ()] [0];
		Debug.Log ("done with making the picture");

		this.transform.localEulerAngles = new Vector3 (90, 0, 0);
	}

	public void MakeAllChildren() {
		for (int i = 0; i < CategoryInitializer.Instance.allCachedTexturesList[typeToPopulate.ToString()].Count; i++) {
			Debug.Log ("populating: " + CategoryInitializer.Instance.allCachedTexturesList [typeToPopulate.ToString ()] [i].name);
			GameObject theImage = Instantiate (picturePrefab);
			theImage.transform.SetParent (contentTransform);
			theImage.GetComponent<RawImage> ().texture = CategoryInitializer.Instance.allCachedTexturesList [typeToPopulate.ToString ()] [i];

			theImage.GetComponent<DragMe> ().buttonName = typeToPopulate;
			theImage.GetComponent<DragMe> ().typeToInstantiate = TypeOfPiece.attackBot;	//change this
		}
	}
}
