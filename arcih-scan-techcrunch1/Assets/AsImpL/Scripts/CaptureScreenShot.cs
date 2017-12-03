using UnityEngine;
using System.Collections;
using System.IO;

public class CaptureScreenShot : MonoBehaviour {

	public KeyCode takeScreenshotKey = KeyCode.S;
	public int screenshotCount = 0;



	void Update () {

		if (Input.GetKeyDown(takeScreenshotKey))
		{

			StartCoroutine(captureScreenshot());
		}
	}

	IEnumerator captureScreenshot()
	{
		yield return new WaitForEndOfFrame();
		string path = "Assets/Capture/"
			+ "_" + screenshotCount + "_" + Screen.width + "X" + Screen.height + "" + ".jpeg";
		screenshotCount++;
		Texture2D screenImage = new Texture2D(Screen.width, Screen.height);
		//Get Image from screen
		screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
		screenImage.Apply();
		//Convert to png
		byte[] imageBytes = screenImage.EncodeToPNG();

		//Save image to file
		System.IO.File.WriteAllBytes(path, imageBytes);
	}


}