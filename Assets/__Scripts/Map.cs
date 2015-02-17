using UnityEngine;
using System.Collections;

public class Map : MonoBehaviour {
	static public Map S;


	public const int gridHeight = 6;
	public const int gridWidth = 10;
	public Tile[,] grid = new Tile[gridHeight, gridWidth];



	// Use this for initialization
	void Awake () {
		S = this;
		GameObject[] objs = GameObject.FindGameObjectsWithTag("Tile");
		foreach (GameObject go in objs) {
			//*** THIS ONLY WORKS BECAUSE THE GRID ELEMENTS ARE LAID OUT ON INTEGER SPACES!
			grid[(int)go.transform.position.z, (int)go.transform.position.x] = go.GetComponent<Tile>();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
