using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadCameraSceneOnClick : MonoBehaviour {

	public void LoadByIndex() {
		SceneManager.LoadScene (SceneManager.sceneCount+1);
	}

}