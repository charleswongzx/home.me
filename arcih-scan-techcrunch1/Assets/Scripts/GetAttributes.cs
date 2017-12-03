using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetAttributes : MonoBehaviour {

	public Text num_room_text;
	public Text area_text;
	public Text avgHousingCost_text;
	public Text estCost_text;
	public Text dbApproved_text;

	private int num_rooms;
	private float area;

	private float diversityIndex;
	private float medianAge;
	private float futureHousingCost;
	private float avgHousingCost;
	private float monthlySalary;

	private int userAge;

	private float estCost;
	private bool dbApproved;


	private void estimateCost () {
		int houseAvgSize = 30;
		float pixel2m = (float)5163.25;

		estCost = (float)((area / pixel2m) / houseAvgSize) * avgHousingCost;  // not added options yet
	}


	private void checkDb(){
		float monthlyInstallments = estCost / 12 / 8;
		if (monthlyInstallments > monthlySalary) {
			dbApproved = true;
		} else {
			dbApproved = false;
		}

	}

	// Use this for initialization
	void Start () {

		num_rooms = PlayerPrefs.GetInt("num_rooms");
		area = PlayerPrefs.GetFloat("area");

		diversityIndex = PlayerPrefs.GetFloat ("diversityIndex");
		medianAge = PlayerPrefs.GetFloat ("medianAge");
		futureHousingCost = PlayerPrefs.GetFloat ("futureHousingCost");
		avgHousingCost = PlayerPrefs.GetFloat ("avgHousingCost");

		userAge = PlayerPrefs.GetInt ("userAge");

		estimateCost ();
		checkDb ();
		
	}

	public void refreshAttributes() {
		estimateCost ();
		checkDb(); 
		 
	}
	
	// Update is called once per frame
	void Update () {
		num_room_text.text = num_rooms.ToString();
		area_text.text = area.ToString();
		avgHousingCost_text.text = avgHousingCost.ToString();
		estCost_text.text = estCost.ToString();
		if (dbApproved) {
			dbApproved_text.text = "APPROVED";
		} else {
			dbApproved_text.text = "DENIED";
		}
		refreshAttributes ();
}
}
