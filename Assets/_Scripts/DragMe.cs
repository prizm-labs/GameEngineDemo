using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class DragMe : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public GameObject cube;
	public bool dragOnSurfaces = true;
	
	private GameObject m_DraggingIcon;
	private RectTransform m_DraggingPlane;


	//set these two fields when in UI mode
	public ObjectCreatorButtons buttonName;
	public TypeOfPiece typeToInstantiate;
	public int numberToInstantiate = 1;


	public void OnBeginDrag(PointerEventData eventData)
	{
		var canvas = FindInParents<Canvas>(gameObject);
		if (canvas == null)
			return;

		// We have clicked something that can be dragged.
		// What we want to do is create an icon for this.
		m_DraggingIcon = new GameObject("icon");

		m_DraggingIcon.transform.SetParent (canvas.transform, false);
		m_DraggingIcon.transform.SetAsLastSibling();
		
		var image = m_DraggingIcon.AddComponent<RawImage>();
		// The icon will be under the cursor.
		// We want it to be ignored by the event system.
		CanvasGroup group = m_DraggingIcon.AddComponent<CanvasGroup>();
		group.blocksRaycasts = false;

		image.texture = GetComponent<RawImage>().texture;
		image.SetNativeSize();

		if (dragOnSurfaces)
			m_DraggingPlane = transform as RectTransform;
		else
			m_DraggingPlane = canvas.transform as RectTransform;
		
		SetDraggedPosition(eventData);
	}

	public void OnDrag(PointerEventData data)
	{
		if (m_DraggingIcon != null)
			SetDraggedPosition(data);
	}

	private void SetDraggedPosition(PointerEventData data)
	{
		if (dragOnSurfaces && data.pointerEnter != null && data.pointerEnter.transform as RectTransform != null)
			m_DraggingPlane = data.pointerEnter.transform as RectTransform;
		
		var rt = m_DraggingIcon.GetComponent<RectTransform>();
		Vector3 globalMousePos;
		if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_DraggingPlane, data.position, data.pressEventCamera, out globalMousePos))
		{
			rt.position = globalMousePos;
			rt.rotation = m_DraggingPlane.rotation;
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (m_DraggingIcon != null)
			Destroy(m_DraggingIcon);

		Vector3 location = Camera.main.ScreenToWorldPoint(new Vector3 (eventData.position.x, eventData.position.y, GameManager.DistanceFromCamera));


		numberToInstantiate = Mathf.FloorToInt(transform.parent.parent.parent.parent.Find ("QuantitySlider").GetComponent<Slider> ().value);
		Debug.Log ("released, instantiating new object, quantity: " + numberToInstantiate.ToString());

		for (int i = 0; i < numberToInstantiate; i++) {
			GameObject newPiece = Instantiate (Resources.Load ("Piece", typeof(GameObject))) as GameObject;
			newPiece.GetComponent<Piece> ().myCategory = buttonName;
			newPiece.GetComponent<Piece> ().myType = typeToInstantiate;
			newPiece.GetComponent<Piece> ().Bootstrap ();
			newPiece.transform.position = location;
		}
	}

	static public T FindInParents<T>(GameObject go) where T : Component
	{
		if (go == null) return null;
		var comp = go.GetComponent<T>();

		if (comp != null)
			return comp;
		
		Transform t = go.transform.parent;
		while (t != null && comp == null)
		{
			comp = t.gameObject.GetComponent<T>();
			t = t.parent;
		}
		return comp;
	}
}
