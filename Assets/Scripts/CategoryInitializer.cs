using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CategoryInitializer : MonoBehaviour {

	GameObject cachedStorage;
	GameObject rotatingPicturePrefab;

	public Color storeObjectColor;

	private int numberOfCachedObjects = 0;
	private float seperationDistance = 50.0f;	//how far apart each one is from another

	public Dictionary<string, List<RenderTexture>> allCachedTexturesList = new Dictionary<string, List<RenderTexture>> ();
	public Dictionary<string, List<TypeOfPiece>> allCachedObjectsTypes = new Dictionary<string, List<TypeOfPiece>> ();

	public static CategoryInitializer Instance;

	void Awake () {
		Instance = this;

		cachedStorage = GameObject.Find ("CachedObjectStorage");
		rotatingPicturePrefab = Resources.Load ("ObjectCamera3D", typeof(GameObject)) as GameObject;
		BootstrapDictionaryList ();

		ReloadAllObjects ();
	}

	void Start() {
		
	}

	private GameObject LoadNewRotatingPic() {
		GameObject rotatingPic = Instantiate (rotatingPicturePrefab);
		rotatingPic.transform.SetParent (cachedStorage.transform);
		rotatingPic.transform.localPosition = Vector3.right * seperationDistance * numberOfCachedObjects++;


		GameObject rotatorObj = rotatingPic.transform.GetChild (0).gameObject;
		return rotatorObj;
	}

	public void ReloadAllObjects() {
		cachedStorage.transform.DestroyChildren ();
		numberOfCachedObjects = 0;

		foreach (string category in System.Enum.GetNames(typeof(ObjectCreatorButtons))) {
			List<GameObject> MeshObjects;
			MeshObjects = new List<GameObject>(Resources.LoadAll (category, typeof(GameObject)).Cast<GameObject>().ToArray());

			foreach (GameObject meshObjPrefab in MeshObjects) {
				//Debug.Log ("found meshObject: " + meshObjPrefab.name);

				try {
					TypeOfPiece thisGuysType = (TypeOfPiece) System.Enum.Parse (typeof(TypeOfPiece), meshObjPrefab.name);

					GameObject meshObj = Instantiate (meshObjPrefab);
					meshObj.transform.SetParent (LoadNewRotatingPic().transform);
					meshObj.transform.localPosition = Vector3.zero;
					meshObj.transform.localEulerAngles = new Vector3 (10, 10, 30);

					if (category != ObjectCreatorButtons.Dice.ToString () && category != ObjectCreatorButtons.Cards.ToString()) {
						if (meshObj.GetComponent<MeshRenderer> () != null) {
							MeshRenderer tempMesh = meshObj.GetComponent<MeshRenderer> ();
							tempMesh.materials [0] = new Material (Shader.Find ("Standard"));
							tempMesh.materials [0].name = "storeMaterial_" + meshObj.name;
							tempMesh.materials [0].color = storeObjectColor;		//set the unowned objects color to gray
							tempMesh.materials[0].shader = Shader.Find("Standard");
						}
						for (int i = 0; i < meshObj.transform.childCount; i++) {
							if (meshObj.transform.GetChild(i).GetComponent<MeshRenderer> () != null) {
								MeshRenderer tempMesh = meshObj.transform.GetChild(i).GetComponent<MeshRenderer> ();
								tempMesh.materials [0] = new Material (Shader.Find ("Standard"));
								tempMesh.materials [0].name = "storeMaterial_" + meshObj.name;
								tempMesh.materials [0].color = storeObjectColor;		//set the unowned objects color to gray
								tempMesh.materials[0].shader = Shader.Find("Standard");
							}
						}
					}


					//take care of instanced textures
					RenderTexture instancedRenderTexture = new RenderTexture(320, 240, 1);
					instancedRenderTexture.name = "instancedRenderTexture" + thisGuysType.ToString();
					meshObj.transform.parent.parent.gameObject.GetComponent<Camera> ().targetTexture = instancedRenderTexture;


					allCachedTexturesList [category].Add (instancedRenderTexture);
					allCachedObjectsTypes [category].Add (thisGuysType);

					//Debug.Log ("added object:  " + meshObj.name + " to category: " + category + "with rendered texture: " + instancedRenderTexture.name + " which a type: " + instancedRenderTexture.ToString());
				} catch (System.ArgumentException e) {
					//Debug.LogWarning (meshObjPrefab.name + " wasn't in the types enum, going to try to skip");
					//Debug.LogWarning (e);
				}
			}
		}

	}

	private void BootstrapDictionaryList() {
		foreach (string category in System.Enum.GetNames(typeof(ObjectCreatorButtons))) {
			allCachedTexturesList.Add (category, new List<RenderTexture> ());
			allCachedObjectsTypes.Add (category, new List<TypeOfPiece> ());
		}
	}
}
