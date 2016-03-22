using UnityEngine;
using System.Collections;

public class RenderObject : MonoBehaviour {

	public Camera renderCamera;
	public RenderTexture myRenderTexture;

	public GameObject Model;

	void Awake(){
		getMyRenderCamera ();
		SetNewRenderTexture ();


	}

	private void getMyRenderCamera(){
		renderCamera = GetComponent<Camera>();
	}


	private void SetNewRenderTexture(){
		myRenderTexture = new RenderTexture (256, 256, 16, RenderTextureFormat.ARGB32);
		myRenderTexture.Create ();
		renderCamera.targetTexture = myRenderTexture;
	}
}
