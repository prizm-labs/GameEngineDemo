using UnityEngine;
using System.Collections;

public class DiceMaterialHolder : MonoBehaviour {

	public Material whiteDiceMaterial;
	public Material redDiceMaterial;
	public Material blueDiceMaterial;

	public Texture blueDiceTexture;

	public static DiceMaterialHolder Instance;

	void Awake () {
		Instance = this;
	}
}
