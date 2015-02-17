using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

public enum ElevationChange_e{
	single,
	up,
	down,
	none
}

public enum Alignment_e{
	orthoganal,
	diagonal,
	both
}

public class Unit : MonoBehaviour, Highlightable {

	public Team_e team;

	public MoveState_e moveState;

	public int x;
	public int y;
	public Tile currentSpace;


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

	bool elevChange; 

	SpriteRenderer stateSymbol;

	MeshRenderer render;

	void Awake(){
		stateSymbol = GetComponentInChildren<SpriteRenderer> ();
		render = GetComponent<MeshRenderer> ();
	}

	// Use this for initialization
	void Start () {
		highlighted = false;
		MoveTo (x, y);
		SetState (moveState);
	}

	// does the actual movement (position-wise)
	void MoveTo (int x, int y){
		if (currentSpace != null) {
						currentSpace.unit = null;
		}
		currentSpace = Map.S.grid [y, x];
		currentSpace.unit = this;
		Vector3 pos = currentSpace.collider.bounds.center;
		pos.y = currentSpace.collider.bounds.max.y;
		transform.position = pos;

	}

	public void MoveTo(Tile tile){
		MoveTo ((int)tile.transform.position.x, (int)tile.transform.position.z);
	}
	
	public void Remove(){
		currentSpace.unit = null;
		currentSpace = null;
		Destroy (this.gameObject);
	}

	public void OnMouseUpAsButton(){
		TurnManager.S.SelectUnit (this);
	}

	public void SetState(MoveState_e newState){
		moveState = newState;
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

	public bool Attacker(){
		switch (moveState) {
		case MoveState_e.attack:
		case MoveState_e.upAttack:
		case MoveState_e.downAttack:
			return true;
		default:
			return false;
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

	public List<Highlightable> GetValidMoves(){
		elevChange = false;
		switch (moveState) {
		case MoveState_e.move:
			return validMoves (currentSpace, 2, ElevationChange_e.single);
			break;
		case MoveState_e.upMove:
			return validMoves (currentSpace, 1, ElevationChange_e.up);
			break;
		case MoveState_e.downMove:
			return validMoves (currentSpace, 1, ElevationChange_e.down);
			break;
		default:
			return null;
		}
	}

	public List<Highlightable> GetValidAttacks(){
		switch (moveState) {
		case MoveState_e.attack:
			return validAttacks (currentSpace, ElevationChange_e.none, Alignment_e.orthoganal);
			break;
		case MoveState_e.upAttack:
			return validAttacks (currentSpace, ElevationChange_e.up, Alignment_e.diagonal);
			break;
		case MoveState_e.downAttack:
			return validAttacks (currentSpace, ElevationChange_e.down, Alignment_e.both);
			break;
		default:
			return null;
		}
	}

	List<Highlightable> validAttacks(Tile center, ElevationChange_e elevOpt, Alignment_e align){
		List<Highlightable> units = new List<Highlightable> ();
		int centerX = (int)center.transform.position.x;
		int centerZ = (int)center.transform.position.z;

		if (align == Alignment_e.orthoganal || align == Alignment_e.both) {
			// north
			if (centerZ + 1 < Map.gridHeight && 
			    validElevationChange(center, Map.S.grid[centerZ + 1, centerX], elevOpt) &&
			    	enemyPresent(Map.S.grid[centerZ + 1, centerX])) {
				units.Add (Map.S.grid[centerZ + 1, centerX].unit);
			}
			// south
			if (centerZ - 1 >= 0 &&
			    validElevationChange(center, Map.S.grid[centerZ - 1, centerX], elevOpt) && 
			    	enemyPresent(Map.S.grid[centerZ - 1, centerX])){
				units.Add (Map.S.grid[centerZ - 1, centerX].unit);
			}
			// East
			if (centerX + 1 < Map.gridWidth &&
			    validElevationChange(center, Map.S.grid[centerZ, centerX + 1], elevOpt) && 
			    	enemyPresent(Map.S.grid[centerZ, centerX + 1])){
				units.Add (Map.S.grid[centerZ, centerX + 1].unit);
			}
			// West
			if (centerX - 1 >= 0 &&
			    validElevationChange(center, Map.S.grid[centerZ, centerX - 1], elevOpt) && 
			    	enemyPresent(Map.S.grid[centerZ, centerX - 1])){
				units.Add (Map.S.grid[centerZ, centerX - 1].unit);
			}
		}
		if (align == Alignment_e.diagonal || align == Alignment_e.both) {
			// NE
			if (centerZ + 1 < Map.gridHeight && centerX + 1 < Map.gridWidth &&
			    validElevationChange(center, Map.S.grid[centerZ + 1, centerX + 1], elevOpt) &&
			    	enemyPresent(Map.S.grid[centerZ + 1, centerX + 1])){
				units.Add (Map.S.grid[centerZ + 1, centerX + 1].unit);
			}
			// SE
			if (centerZ - 1 >= 0 && centerX + 1 < Map.gridWidth &&
			    validElevationChange(center, Map.S.grid[centerZ - 1, centerX + 1], elevOpt) && 
			    	enemyPresent(Map.S.grid[centerZ - 1, centerX + 1])){
				units.Add (Map.S.grid[centerZ - 1, centerX + 1].unit);
			}
			// SW
			if (centerZ - 1 >= 0 && centerX - 1 >= 0 &&
			    validElevationChange(center, Map.S.grid[centerZ - 1, centerX - 1], elevOpt) &&
			    	enemyPresent(Map.S.grid[centerZ - 1, centerX - 1])){
				units.Add (Map.S.grid[centerZ - 1, centerX - 1].unit);
			}
			// NW
			if (centerZ + 1 < Map.gridHeight && centerX - 1 >= 0 &&
			    validElevationChange(center, Map.S.grid[centerZ + 1, centerX - 1], elevOpt) &&
			    	enemyPresent(Map.S.grid[centerZ + 1, centerX - 1])){
				units.Add (Map.S.grid[centerZ + 1, centerX - 1].unit);
			}
		}
		if (units.Count == 0)
						return null;
		return units;
	}

	List<Highlightable> validMoves(Tile center, int radius, ElevationChange_e elevOpt){
		List<Highlightable> tiles = new List<Highlightable>();
		// north
		// unconventional for loop because we are counting moves
		int centerX = (int)center.transform.position.x;
		int centerZ = (int)center.transform.position.z;
		elevChange = false;
		for (int i = 1; i <= radius; ++i) {
			if (centerZ + i >= Map.gridHeight) break;
			print ("looking north");
			// now that we know we *could* move there, get the elevation difference
			if (!validElevationChange(Map.S.grid[centerZ + i - 1, centerX], Map.S.grid[centerZ + i, centerX],
			                          elevOpt) && !HomeFlag(Map.S.grid[centerZ + i, centerX])) break;
			// everything's cool, add tile to list
			tiles.Add(Map.S.grid[centerZ + i, centerX]);
		}
		// south
		elevChange = false;
		for (int i = 1; i <= radius; ++i) {
			if (centerZ - i < 0) break;
			print ("looking south");
			if (!validElevationChange(Map.S.grid[centerZ - i + 1, centerX], Map.S.grid[centerZ - i, centerX],
			                          elevOpt) && !HomeFlag (Map.S.grid[centerZ - i, centerX])) break;
			tiles.Add (Map.S.grid[centerZ - i, centerX]);
		}
		// east
		elevChange = false;
		for (int i = 1; i <= radius; ++i) {
			if (centerX + i >= Map.gridWidth) break;
			print ("looking east");
			if (!validElevationChange(Map.S.grid[centerZ, centerX + i - 1], Map.S.grid[centerZ, centerX + i],
			                          elevOpt) && !HomeFlag(Map.S.grid[centerZ, centerX + i])) break;
			tiles.Add (Map.S.grid[centerZ, centerX + i]);
		}
		// west
		elevChange = false;
		for (int i = 1; i <= radius; ++i) {
			if (centerX - i < 0) break;
			print ("looking west");
			if (!validElevationChange(Map.S.grid[centerZ, centerX - i + 1], Map.S.grid[centerZ, centerX - i],
			                          elevOpt) && !HomeFlag(Map.S.grid[centerZ, centerX - i])) break;
			tiles.Add (Map.S.grid[centerZ, centerX - i]);
		}
		if (tiles.Count == 0)
						return null;
		return tiles;
	}

	bool validElevationChange(Tile start, Tile target, ElevationChange_e elevOpt){
		int eleDiff = (int)target.terrType - (int)start.terrType;
		if (elevOpt == ElevationChange_e.none && eleDiff != 0) return false;
		if (elevOpt == ElevationChange_e.single){
			if (elevChange && eleDiff != 0) return false; // if we have already changed elevation and this is also a change
			// if the difference is greater than one or you're trying to climb a mountain, it's not ok.
			else if (eleDiff > 1 || eleDiff < -1 ) return false;
			else if ((eleDiff == 1 || eleDiff == -1) && 
			         (target.terrType == TerrainType_e.mountain || start.terrType == TerrainType_e.mountain)) return false;
			elevChange = true; // only allow one elevation change!
		}
		if (elevOpt == ElevationChange_e.up && eleDiff <= 0) return false;
		if (elevOpt == ElevationChange_e.down && eleDiff >= 0) return false;
		return true;
	} 


	bool enemyPresent(Tile tile){
		if (tile.unit == null)
						return false;
		if (tile.unit.team == team)
						return false;
		return true;
	}

	bool HomeFlag(Tile tile){
		if (team == Team_e.red && tile.flag == Flag_e.red)
						return true;
		else if (team == Team_e.blue && tile.flag == Flag_e.blue)
						return true;
				
		return false;
	}
//
//	public List<Highlightable> GetValidTargets(){
//		
//	}

}
