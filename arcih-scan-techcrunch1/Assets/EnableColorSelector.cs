using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableColorSelector : MonoBehaviour {

	// Use this for initialization
	public GameObject UImanager;
	public GameObject ColorSelectorPanel;

	public void Enable_panel()
	{
		UImanager.SetActive (true);
		ColorSelectorPanel.SetActive (true);
	}
}
