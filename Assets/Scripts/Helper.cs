using UnityEngine;
using System.Collections;

public static class Helper {

	//hack around to get a primitive quad mesh
	public static Mesh GetQuadMesh() {
		GameObject go = GameObject.CreatePrimitive (PrimitiveType.Quad);
		MeshFilter mf = go.GetComponent<MeshFilter> ();
		Mesh mesh = mf.mesh;

		GameObject.Destroy (go);
		return mesh;
	}

}
