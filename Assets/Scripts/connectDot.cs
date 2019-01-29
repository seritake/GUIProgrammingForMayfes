using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Shapes;
using UnityEngine.UI;

public class connectDot : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler {

	public bool drawFlag = false;
	public GameObject connected;
	public DotType type;

	public enum DotType {
		START,
		END
	}

	public LineInfo info = new Shapes.LineInfo {
		forward = new Vector3(0, 0, 10),
		fillColor = Color.blue,
		arrowWidth = 20f,
		arrowLength = 20f,
		width = 7f
	};

	public void OnBeginDrag( PointerEventData e) {
		if (connected != null) {
			connected.GetComponent<connectDot> ().clearConnected ();
		} 
		connected = null;
		drawFlag = true;
		this.info.endPos = e.position;
	}

	public void OnDrag( PointerEventData e) {
		this.info.endPos = e.position;
	}

	public void OnEndDrag( PointerEventData e) {
		if (connected == null) {
			drawFlag = false;
		} else {
			drawFlag = true;
		}
	}

	public void OnDrop( PointerEventData e) {
		connectDot script;
		script = e.pointerDrag.GetComponent<connectDot> ();
		if (this.type == script.type) {
			return;
		}
		script.addConnectedDot (this.gameObject);
		this.connected = e.pointerDrag;
	}

	public void addConnectedDot(GameObject o) {
		this.connected = o;
		info.endPos = o.transform.position;
		drawFlag = true;
	}

	public void clearConnected() {
		this.connected = null;
		this.drawFlag = false;
	}
		

	// Use this for initialization
	void Start () {
		if (type == DotType.END) {
			this.info.startArrow = true;
			this.info.endArrow = false;
		} else if (type == DotType.START) {
			this.info.endArrow = true;
			this.info.startArrow = false;
            this.gameObject.GetComponent<Image>().color = new Color(0, 57F/255, 245F/255, 255F/255);
		}

		info.startPos = this.gameObject.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (drawFlag) {
			info.startPos = this.transform.position;
			if (connected != null) {
				info.endPos = this.connected.transform.position;
			}
			LineSegment.Draw (info);
		}

	}


}
