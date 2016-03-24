using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ChangeQuantity : MonoBehaviour {
	
	public void ChangeValue(float value){
	    Text	quantity = GetComponent<Text> ();

		if (quantity != null)
			quantity.text = Mathf.RoundToInt (value ).ToString();
	}
}
