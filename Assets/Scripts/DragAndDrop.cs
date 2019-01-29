using UnityEngine;
using UnityEngine.EventSystems;

using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
	public RectTransform m_rectTransform;

	private Vector3 initialPos;

	public GameObject prefab;

	public GameObject runningArea;

	void Start() {
		initialPos = m_rectTransform.position;
	}

	private void Reset()
	{
		m_rectTransform = GetComponent<RectTransform>();
	}

	public void OnBeginDrag( PointerEventData e) {
		this.transform.SetSiblingIndex (10);
	}

	public void OnDrag( PointerEventData e )
	{
		m_rectTransform.position += new Vector3( e.delta.x, e.delta.y, 0f );
	}

	public void OnEndDrag( PointerEventData e )
	{
		Vector3 currentPos = m_rectTransform.position;
		m_rectTransform.position = initialPos;
		if (!isInRunningArea (e.position)) { return; }
		m_rectTransform.position = initialPos;
		GameObject newObj = Instantiate (prefab);
		newObj.transform.position = currentPos;
		newObj.transform.parent = runningArea.transform;
	}

	public void OnDrop() {

	}

	private bool isInRunningArea(Vector3 position) {
		position = Camera.main.ScreenToWorldPoint (position);
		Vector3 areaPoint = runningArea.transform.position;
		Vector3 sizePoint = runningArea.GetComponent<RectTransform> ().sizeDelta;
		float left = 305; //areaPoint.x - (sizePoint.x / 2);
		float right = 999; //areaPoint.x + (sizePoint.x / 2);
		float top = 743; //areaPoint.y + (sizePoint.y / 2);
		float bottom = 14; //areaPoint.y - (sizePoint.y / 2);
		return position.x < right && position.x > left && position.y > bottom && position.y < top;
	}
}