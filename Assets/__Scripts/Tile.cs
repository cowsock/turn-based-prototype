using UnityEngine;
using System.Collections;

public enum TerrainType_e{
	river,
	plain,
	plateau,
	mountain,
	flagR,
	flagB
}

public class Tile : MonoBehaviour {

	public TerrainType_e terrType;

	public Material normalMat;
	public Material highlightMat;
	
	public bool highlighted;

	public bool selectable;

	public bool occupied;

	MeshRenderer render;

	void Awake(){
		occupied = false;
		selectable = false;
		highlighted = false;
		render = GetComponent<MeshRenderer> ();
	}
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// toggles highlight material for this tile.
	public void Highlight(){
		if (highlighted) {
			highlighted = false;
			render.material = normalMat;

		} else {
			highlighted = true;
			render.material = highlightMat;
		}
	}

	public void OnMouseUpAsButton(){
		print ("clicked this mug");
	}
}
