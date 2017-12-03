
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.IO;

public class ObjDownloader_Floor : MonoBehaviour {

	private string modelName = "get_floor";

	void Start () {
		StartCoroutine(ImportObject());
	}

	// Update is called once per frame
	IEnumerator ImportObject () {

		Debug.Log ("Floor Download Starting");
		WWW www = new WWW("http://192.168.43.153:5000/"+modelName);
		//yield return www;

		while (!www.isDone) {
			yield return null;
		}

		string fullPath = Application.dataPath + "/Objects/"+"floor.obj";
		File.WriteAllBytes (fullPath, www.bytes);
		Debug.Log ("Floor Done Downloading");
	}
}

