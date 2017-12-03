using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using LitJson;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;


public class QueryAddressOnClick : MonoBehaviour {

	public InputField addressField;
	public Text finalAddress;
	private string call;
	private string jsonString;
	private JsonData itemData;

	public void QueryAddress() {

		DontDestroyOnLoad (transform.gameObject);

		call = "http://geoenrich.arcgis.com/arcgis/rest/services/World/geoenrichmentserver/GeoEnrichment/enrich?StudyAreas=[{\"address\":{\"text\":\"Manhattan, New York, USA\"}}]&dataCollections=[\"KeyGlobalFacts\"]&f=pjson&token=ZuHwnqED6Zrtl4MAj591SrIw_ByityCcP3kNrIMIRVNMj0waC8DiDEHwyvrVzUceNDCm187MUZ7FIkZ3ybo3HAMiSh1Lu34mYZwSuEsHgBM6QJGg_HWTumqfInDfnvrZ&analysisVariables=[\"homevalue.AVGVAL_CY\", \"DIVINDX_CY\", \"MEDAGE_CY\"]";

		print (call);

		LoadNextScene (1);

		StartCoroutine(GetText());


		// query arcgis API
		// get housing type, average cost of housing, average housing psf
	}

	public void LoadNextScene(int sceneIndex) {
		SceneManager.LoadScene (sceneIndex);
	}

	void LoadErisJSON() {
		//Debug.Log (itemData ["results"] [0] ["value"] ["FeatureSet"] [0] ["features"] [0] ["attributes"] ["AVGVAL_CY"]);
		float avgHousingCost = (float)(int)itemData ["results"][0]["value"]["FeatureSet"][0]["features"][0]["attributes"]["AVGVAL_CY"];
		float diversityIndex = (float)itemData ["results"][0]["value"]["FeatureSet"][0]["features"][0]["attributes"]["DIVINDX_CY"];
		float medianAge = (float)itemData ["results"][0]["value"]["FeatureSet"][0]["features"][0]["attributes"]["MEDAGE_CY"];
		float futureHousingCost = (float)itemData ["results"][0]["value"]["FeatureSet"][0]["features"][0]["attributes"]["AVGVAL_FY"];

		PlayerPrefs.SetFloat ("diversityIndex", diversityIndex);
		PlayerPrefs.SetFloat ("medianAge", medianAge);
		PlayerPrefs.SetFloat ("futureHousingCost", futureHousingCost);
		//PlayerPrefs.SetFloat ("avgHousingCost", avgHousingCost);

		//print(PlayerPrefs.GetFloat("avgHousingCost"));

		Destroy (transform.gameObject);


	
	}

	IEnumerator GetText()
	{
			
		using (UnityWebRequest www = UnityWebRequest.Get(call))
		{
			
			yield return www.Send();

			if (www.isError) {
				Debug.Log (www.error);
			}

			// Show results as text
			Debug.Log(www.downloadHandler.text);
			itemData = JsonMapper.ToObject(www.downloadHandler.text);
			LoadErisJSON();

			// Or retrieve results as binary data
			//byte[] results = www.downloadHandler.data;

		}
	}
		
}
