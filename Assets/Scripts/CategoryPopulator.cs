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

	public void ReloadThisArea() {
		//kill all children 
		for (int i = 0; i < contentTransform.childCount; i++) {
			Destroy (contentTransform.GetChild (i).gameObject);
		}

		//and then reload all children
		MakeAllChildren();
	}

	public void MakeAllChildren() {
		for (int i = 0; i < CategoryInitializer.Instance.allCachedTexturesList[typeToPopulate.ToString()].Count; i++) {
			GameObject theImage = Instantiate (picturePrefab);
			theImage.transform.SetParent (contentTransform);
			theImage.GetComponent<RawImage> ().texture = CategoryInitializer.Instance.allCachedTexturesList [typeToPopulate.ToString ()] [i];
			theImage.GetComponent<DragMe> ().buttonName = typeToPopulate;
			theImage.GetComponent<DragMe> ().typeToInstantiate = CategoryInitializer.Instance.allCachedObjectsTypes [typeToPopulate.ToString ()] [i];
			theImage.transform.localScale = Vector3.one;
		}
	}


}
