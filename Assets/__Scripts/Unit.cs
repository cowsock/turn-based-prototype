using UnityEngine;
using System.Collections;

// BE SUPER CAREFUL ABOUT CHANGING THESE
// Their linkage to the GUI buttons is NOT SYMBOLIC
// you can add to the list, but don't change the order!
public enum MoveState_e {
	downMove,
	downAttack,
	move,
	attack,
	upMove,
	upAttack,
}



public class Unit : MonoBehaviour {

	public Team_e team;

	public MoveState_e moveState;

	public int x;
	public int y;
	Tile currentSpace;

	public int health;

	public bool alive;

	public bool highlighted;

	public bool moved; // has it been moved this turn

	public Material normalMat;
	public Material highlightMat;
	public Sprite normalMoveSprite;
	public Sprite attakSprite;
	public Sprite upMoveSprite;
	public Sprite upAttackSprite;
	public Sprite downMoveSprite;
	public Sprite downAttackSprite;

	SpriteRenderer stateSymbol;

	MeshRenderer render;

	void Awake(){
		stateSymbol = GetComponentInChildren<SpriteRenderer> ();
		render = GetComponent<MeshRenderer> ();
	}

	// Use this for initialization
	void Start () {
		health = 5;
		alive = true;
		highlighted = false;
		MoveTo (x, y);
		SetState (moveState);
	}

	// does the actual movement (position-wise)
	void MoveTo (int x, int y){
		Vector3 pos = Map.S.grid [y, x].collider.bounds.center;
		pos.y = Map.S.grid [y, x].collider.bounds.max.y;
		transform.position = pos;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void TakeHit(int dmg){
		health -= dmg;
		if (health <= 0)
						alive = false;
	}

	public void OnMouseUpAsButton(){
		TurnManager.S.SelectUnit (this);
	
	
	}

	public void SetState(MoveState_e newState){
		switch (newState) {
		case MoveState_e.move:
			stateSymbol.sprite = normalMoveSprite;
			break;
		case MoveState_e.attack:
			stateSymbol.sprite = attakSprite;
			break;
		case MoveState_e.downMove:
			break;
		case MoveState_e.downAttack:
			break;
		case MoveState_e.upMove:
			break;
		case MoveState_e.upAttack:
			break;
		}
	}

	public void Highlight(){
		if (highlighted) {
			highlighted = false;
			render.material = normalMat;
			
		} else {
			highlighted = true;
			render.material = highlightMat;
		}
	}

}
