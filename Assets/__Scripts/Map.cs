using UnityEngine;
using System.Collections;

public class Map : MonoBehaviour {
	static public Map S;


	const int gridHeight = 6;
	const int gridWidth = 10;
	public GameObject[,] grid = new GameObject[gridHeight, gridWidth];



	// Use this for initialization
	void Awake () {
		S = this;
		GameObject[] objs = GameObject.FindGameObjectsWithTag("Tile");
		foreach (GameObject go in objs) {
			//*** THIS ONLY WORKS BECAUSE THE GRID ELEMENTS ARE LAID OUT ON INTEGER SPACES!
			grid[(int)go.transform.position.z, (int)go.transform.position.x] = go;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
