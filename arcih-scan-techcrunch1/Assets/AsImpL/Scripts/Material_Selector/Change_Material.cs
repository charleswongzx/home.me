using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Change_Material : MonoBehaviour {

	private GameObject walls;
	private GameObject floor;

	public Material FMatA;
	public Material FMatB;
	public Material FMatC;
	public Material WMatA;
	public Material WMatB;
	public Material WMatC;

	private void Start(){

	walls = GameObject.Find ("Walls");
	Debug.Log (walls);
	floor = GameObject.Find ("Floor");
	FMatA = Resources.Load ("materials/Walls/1", typeof(Material)) as Material;
	FMatB = Resources.Load ("materials/Walls/2", typeof(Material)) as Material;
	FMatC = Resources.Load ("materials/Walls/3", typeof(Material)) as Material;

	WMatA = Resources.Load ("materials/Floor/1f", typeof(Material)) as Material;
	WMatB = Resources.Load ("materials/Floor/2f", typeof(Material)) as Material;
	WMatC = Resources.Load ("materials/Floor/3f", typeof(Material)) as Material;
	}




	// Use this for initialization
	public void Set_floorA()
	{
		Change_floor_mat (FMatA);
	}

	public void Set_floorB()
	{
		Change_floor_mat (FMatB);
	}

	public void Set_floorC()
	{
		Change_floor_mat (FMatC);
	}

	public void Set_wallA()
	{
		Change_walls_mat (WMatA);
	}

	public void Set_wallB()
	{
		Change_walls_mat (WMatB);
	}

	public void Set_wallC()
	{
		Change_walls_mat (WMatC);
	}



	public void Change_floor_mat(Material mat_to_set){
		Renderer[] children;
		children = floor.GetComponentsInChildren<Renderer> ();
		foreach (Renderer rend in children) {
			var mats = new Material[rend.materials.Length];
			for (var j = 0; j < rend.materials.Length; j++) {
				mats [j] = mat_to_set;
			}
			rend.materials = mats;
		}
	}

	public void Change_walls_mat(Material mat_to_set){
		Renderer[] children;
		children = walls.GetComponentsInChildren<Renderer> ();
		foreach (Renderer rend in children) {
			var mats = new Material[rend.materials.Length];
			for (var j = 0; j < rend.materials.Length; j++) {
				mats [j] = mat_to_set;
			}
			rend.materials = mats;
		}
	}
}
