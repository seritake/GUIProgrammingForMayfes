using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Http : MonoBehaviour {
	private string command = "empty";
	private string sensor = "0";

	// Use this for initialization
	void Start () {
		StartCoroutine("Upload");
	}



	IEnumerator Upload() {
		while (true) {
            if (command != "empty") {
                print(command);
            }
		UnityWebRequest www = UnityWebRequest.Get ("http://127.0.0.1:5000/?data=" + command);
			yield return www.SendWebRequest ();


			if (www.isNetworkError || www.isHttpError) {
				Debug.Log (www.error);
			} else {
				this.sensor = www.downloadHandler.text;
                //print(this.sensor);
				this.command = "empty";
			}
			yield return new WaitForSeconds (0.05f);
		}
	}
		
	
	public void setCommand(string s) {
		command = s;
	}

	public float getSensor() {
		//print (sensor);
		return float.Parse (sensor);
	}
}
