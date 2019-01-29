using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class velocityDropdown : MonoBehaviour {

	public Dropdown dropdown;

	// Use this for initialization
	void Start () {
		if (dropdown) {
			dropdown.ClearOptions ();
			List<string> list = new List<string> ();
			for (int i = -100; i <= 100; i++) {
				list.Add (i.ToString ());
			}
			dropdown.AddOptions (list);
			dropdown.value = 100;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}


}
