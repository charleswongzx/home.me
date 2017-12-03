using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.IO;

public class ObjDownloader_walls : MonoBehaviour {

	private string modelName = "get_walls";

	void Start () {
		StartCoroutine(ImportObject());
	}

	// Update is called once per frame
	IEnumerator ImportObject () {

		Debug.Log ("Walls Download Starting");
		WWW www = new WWW("http://192.168.43.153:5000/"+modelName);
		//yield return www;

		while (!www.isDone) {
			yield return null;
		}

		string fullPath = Application.dataPath + "/Objects/"+"walls.obj";
		File.WriteAllBytes (fullPath, www.bytes);
		Debug.Log ("Walls Done Downloading");
	}
}

