using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
//using LitJson;
using UnityEngine.Networking;
using System.IO;

public class ProcessImage : MonoBehaviour {

	private Image photo;
//	private JsonData itemData;


	public void uploadImage() {
		Debug.Log ("Button Press");
		DontDestroyOnLoad (transform.gameObject);
		
		StartCoroutine (PostFloorplan ());
	}


	IEnumerator PostFloorplan(){
		

		yield return new WaitForEndOfFrame ();
		string path = "Assets/Capture/" + "floor.jpg";

		Texture2D screenImage = new Texture2D (Screen.width, Screen.height);
		//Get Image from screen
		screenImage.ReadPixels (new Rect (0, 0, Screen.width, Screen.height), 0, 0);
		screenImage.Apply ();
		//Convert to png
		byte[] imageBytes = screenImage.EncodeToJPG ();
		Debug.Log ("Picture Taken");

		//Save image to file
		System.IO.File.WriteAllBytes (path, imageBytes);

//		List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
//		formData.Add( new MultipartFormFileSection("file", "Assets/Capture/floor.jpg") );  // IM NOT SURE HOW YOU POST FROM CAMERA

		WWWForm form = new WWWForm();

		form.AddBinaryData("file", imageBytes, "floor.jpg", "image/jpg");

		// Upload to a cgi script
		WWW w = new WWW("http://192.168.43.153:5000/process_img", form);
		yield return w;
		if (!string.IsNullOrEmpty(w.error)) {
			print(w.error);
		}
		else {
			print("Finished Uploading Screenshot");
		}
	}



//		using (UnityWebRequest www = UnityWebRequest.Post("http://192.168.43.153:5000/process_img", formData))
//		{
//			
//			yield return www.Send();
//
//			if (www.isError) {
//				Debug.Log (www.error);
//			}
//			Debug.Log ("5");

//			StartCoroutine (GetWalls ());
//			StartCoroutine (GetFloor ());

			// Show results as text
//			Debug.Log(www.downloadHandler.text);
//			itemData = JsonMapper.ToObject(www.downloadHandler.text);

//			int num_rooms = (int)itemData ["num_rooms"];
//			float area = (float)itemData ["area"];

//			PlayerPrefs.SetInt ("num_rooms", num_rooms);
//			PlayerPrefs.SetFloat ("area", area);

			// Or retrieve results as binary data
			//byte[] results = www.downloadHandler.data;

//		}
//	}


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