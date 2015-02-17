using UnityEngine;
using System.Collections;

public enum TerrainType_e{
	river,
	plain,
	plateau,
	mountain
}

public enum Flag_e{
	red,
	blue,
	none
}

public class Tile : MonoBehaviour, Highlightable {

	public TerrainType_e terrType;
	public Flag_e flag;

	public Material normalMat;
	public Material highlightMat;
	
	public bool highlighted;

	public bool selectable;

	public Unit unit;


	MeshRenderer render;

	void Awake(){
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
			if (unit != null) return;
			highlighted = true;
			render.material = highlightMat;
		}
	}

	public void OnMouseUpAsButton(){
		TurnManager.S.SelectTile (this);
	}
}
