using UnityEngine;
using System.Collections;
using TouchScript;
using TouchScript.Gestures;

public class ApplyTransform : MonoBehaviour {
	TransformGesture myTransformGesture;

	void Awake() { 
		myTransformGesture = GetComponent<TransformGesture> ();
	}
	void OnEnable() {
		myTransformGesture.Transformed += MyTransformGesture_Transformed;
	}
	void OnDisable() {
		myTransformGesture.Transformed -= MyTransformGesture_Transformed;
	}

	void MyTransformGesture_Transformed (object sender, System.EventArgs e)
	{
		myTransformGesture.ApplyTransform (this.transform);
	}

}
