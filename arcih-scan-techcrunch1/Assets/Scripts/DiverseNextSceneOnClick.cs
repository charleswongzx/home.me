using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DiverseNextSceneOnClick : MonoBehaviour {

	public void LoadNextScene() {
		print (SceneManager.sceneCount);
		SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex+1);
	}
}
