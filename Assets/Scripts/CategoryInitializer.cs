using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CategoryInitializer : MonoBehaviour {

	GameObject cachedStorage;
	GameObject rotatingPicturePrefab;

	private int numberOfCachedObjects = 0;
	private float seperationDistance = 50.0f;	//how far apart each one is from another

	public Dictionary<string, List<RenderTexture>> allCachedTexturesList = new Dictionary<string, List<RenderTexture>> ();

	public static CategoryInitializer Instance;

	void Awake () {
		Instance = this;

		cachedStorage = GameObject.Find ("CachedObjectStorage");
		rotatingPicturePrefab = Resources.Load ("ObjectCamera3D", typeof(GameObject)) as GameObject;
		BootstrapDictionaryList ();
	}

	void Start() {
		ReloadAllObjects ();
	}

	private GameObject LoadNewRotatingPic() {
		GameObject rotatingPic = Instantiate (rotatingPicturePrefab);
		rotatingPic.transform.SetParent (cachedStorage.transform);
		rotatingPic.transform.localPosition = Vector3.right * seperationDistance * numberOfCachedObjects++;


		GameObject rotatorObj = rotatingPic.transform.GetChild (0).gameObject;
		return rotatorObj;
	}

	public void ReloadAllObjects() {
		Debug.Log ("reloading all objects in CategoryPopulator");

		cachedStorage.transform.DestroyChildren ();
		numberOfCachedObjects = 0;

		foreach (string category in System.Enum.GetNames(typeof(ObjectCreatorButtons))) {
			Debug.Log ("trying to load resources from: " + category);

			List<GameObject> MeshObjects;
			MeshObjects = new List<GameObject>(Resources.LoadAll (category, typeof(GameObject)).Cast<GameObject>().ToArray());

			foreach (GameObject meshObjPrefab in MeshObjects) {
				Debug.Log ("found meshObject: " + meshObjPrefab.name);

				GameObject meshObj = Instantiate (meshObjPrefab);
				meshObj.transform.SetParent (LoadNewRotatingPic().transform);
				meshObj.transform.localPosition = Vector3.zero;

				//take care of instanced textures
				RenderTexture instancedRenderTexture = new RenderTexture(320, 240, 1);
				Debug.Log ("created new instance of render texture, here it is before naming: " + instancedRenderTexture.ToString ());
				instancedRenderTexture.name = "fuck";
				Debug.Log ("here is the render texture after naming: " + instancedRenderTexture.name.ToString ());

				meshObj.transform.parent.parent.gameObject.GetComponent<Camera> ().targetTexture = instancedRenderTexture;


				allCachedTexturesList [category].Add (instancedRenderTexture);
				Debug.Log ("added object:  " + meshObj.name + " to category: " + category + "with rendered texture: " + instancedRenderTexture.name + " which a type: " + instancedRenderTexture.ToString());

			}
		}

	}

	private void BootstrapDictionaryList() {
		foreach (string category in System.Enum.GetNames(typeof(ObjectCreatorButtons))) {
			allCachedTexturesList.Add (category, new List<RenderTexture> ());
		}
	}
}
