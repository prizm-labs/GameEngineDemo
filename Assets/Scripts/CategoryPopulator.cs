using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CategoryPopulator : MonoBehaviour {

	public ObjectCreatorButtons typeToPopulate;

	GameObject picturePrefab;
	Transform contentTransform;

	void Awake () {
		contentTransform = transform.Find ("ScrollView").Find("ScrollRect").Find("Content");
		picturePrefab = Resources.Load ("rawImage", typeof(GameObject)) as GameObject;
	}

	void Start() {
		MakeAllChildren ();
	}

	public void MakeAllChildren() {
		for (int i = 0; i < CategoryInitializer.Instance.allCachedTexturesList[typeToPopulate.ToString()].Count; i++) {
			Debug.Log ("populating: " + CategoryInitializer.Instance.allCachedTexturesList [typeToPopulate.ToString ()] [i].name);
			GameObject theImage = Instantiate (picturePrefab);
			//
			Debug.Log ("set parent to: " + contentTransform.name);
			theImage.transform.SetParent (contentTransform);
			theImage.GetComponent<RawImage> ().texture = CategoryInitializer.Instance.allCachedTexturesList [typeToPopulate.ToString ()] [i];

			theImage.GetComponent<DragMe> ().buttonName = typeToPopulate;
			Debug.Log ("setting type from: " + CategoryInitializer.Instance.allCachedObjectsTypes.ToString ());
			Debug.Log ("setting type to: " + CategoryInitializer.Instance.allCachedObjectsTypes [typeToPopulate.ToString ()] [i]);
			theImage.GetComponent<DragMe> ().typeToInstantiate = CategoryInitializer.Instance.allCachedObjectsTypes [typeToPopulate.ToString ()] [i];
			theImage.transform.localScale = Vector3.one;
		}
	}
}
