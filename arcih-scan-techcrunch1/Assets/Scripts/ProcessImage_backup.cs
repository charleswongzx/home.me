using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using LitJson;
using UnityEngine.Networking;
using System.IO;

public class ProcessImageBackup : MonoBehaviour {

	private Image photo;
	private JsonData itemData;
	

	public void uploadImage(Image phonePhoto) {
		DontDestroyOnLoad (transform.gameObject);

		photo = phonePhoto;  // MAYBE SAVE THE PHOTO?
		StartCoroutine (PostFloorplan ());
	}
		

	IEnumerator PostFloorplan(){
		
		List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
		formData.Add( new MultipartFormFileSection("file", "photo.jpg") );  // IM NOT SURE HOW YOU POST FROM CAMERA


		using (UnityWebRequest www = UnityWebRequest.Post("192.168.43.153:5000/process_img", formData))
		{

			yield return www.Send();

			if (www.isError) {
				Debug.Log (www.error);
			}

			StartCoroutine (GetWalls ());
			StartCoroutine (GetFloor ());

			// Show results as text
			Debug.Log(www.downloadHandler.text);
			itemData = JsonMapper.ToObject(www.downloadHandler.text);

			int num_rooms = (int)itemData ["num_rooms"];
			float area = (float)itemData ["area"];

			PlayerPrefs.SetInt ("num_rooms", num_rooms);
			PlayerPrefs.SetFloat ("area", area);
				
			// Or retrieve results as binary data
			//byte[] results = www.downloadHandler.data;

		}
	}


	IEnumerator GetFloor () {

		Debug.Log ("Floor Download Starting");
		WWW www = new WWW("http://192.168.43.153:5000/get_floor");
		//yield return www;

		while (!www.isDone) {
			yield return null;
		}

		string fullPath = Application.dataPath + "/Objects/"+"floor.obj";
		File.WriteAllBytes (fullPath, www.bytes);
		Debug.Log ("Floor Done Downloading");
	}

	IEnumerator GetWalls () {

		Debug.Log ("Walls Download Starting");
		WWW www = new WWW("http://192.168.43.153:5000/get_walls");
		//yield return www;

		while (!www.isDone) {
			yield return null;
		}

		string fullPath = Application.dataPath + "/Objects/"+"walls.obj";
		File.WriteAllBytes (fullPath, www.bytes);
		Debug.Log ("Walls Done Downloading");
	}
		
}
