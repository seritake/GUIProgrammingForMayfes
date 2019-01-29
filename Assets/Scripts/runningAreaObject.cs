using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Shapes;


public class runningAreaObject : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler  {

	public ComponentType type;
	
	public RectTransform m_rectTransform;
	private Vector3 initialPos;

	public MoveWheel.WheelSide wheelSide;

	public GameObject upperDot;
	public GameObject rightDot;
	public GameObject leftDot;
	public GameObject lowerDot;
	public GameObject lowerTrueDot;
	public GameObject lowerFalseDot;

	public Dropdown dropDown1;
	public Dropdown dropDown2;

	public InputField inputField;

	private GameObject gameController;

	public int selfNumber;

	private void Reset()
	{
		m_rectTransform = GetComponent<RectTransform>();
	}

	public void OnBeginDrag( PointerEventData e )
	{
		initialPos = m_rectTransform.position;
		this.transform.SetSiblingIndex (0);
	}

	public void OnDrag( PointerEventData e )
	{
		m_rectTransform.position += new Vector3( e.delta.x, e.delta.y, 0f );
	}

	public void OnEndDrag( PointerEventData e)
	{
		if (isInDeleteArea (e.position)) {
			if (this.type != ComponentType.End) {
				Destroy (this.gameObject);
			}
		}
		if (!isInRunningArea (e.position)) {
			m_rectTransform.position = initialPos;
			return;
		}

	}

	// Use this for initialization
	void Start () {
		gameController = GameObject.Find ("GameController");
		selfNumber = gameController.GetComponent<GameController> ().addComponent(this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	}

	private bool isInRunningArea(Vector3 position) {
		position = Camera.main.ScreenToWorldPoint (position);
		float left = 305; //areaPoint.x - (sizePoint.x / 2);
		float right = 999; //areaPoint.x + (sizePoint.x / 2);
		float top = 743; //areaPoint.y + (sizePoint.y / 2);
		float bottom = 14; //areaPoint.y - (sizePoint.y / 2);
		return position.x < right && position.x > left && position.y > bottom && position.y < top;
	}

	private bool isInDeleteArea(Vector3 position) {
		position = Camera.main.ScreenToWorldPoint (position);
		float left = 16; //areaPoint.x - (sizePoint.x / 2);
		float right = 286; //areaPoint.x + (sizePoint.x / 2);
		float top = 140; //areaPoint.y + (sizePoint.y / 2);
		float bottom = 14; //areaPoint.y - (sizePoint.y / 2);
		return position.x < right && position.x > left && position.y > bottom && position.y < top;
	}
}
