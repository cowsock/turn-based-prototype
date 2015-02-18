using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public enum Team_e{
	red,
	blue
}

public enum turnState_e{
	movement,
	change,
	end
}



public class TurnManager : MonoBehaviour {
	static public TurnManager S;
	public List<Unit> redTeam;
	public List<Unit> blueTeam;

	public int moves_per_turn;
	public int changes_per_turn;

	public Team_e activePlayer; 
	public turnState_e turnPhase;

	int receivedMoveOrders;
	int receivedChangeOrders;

	List<Highlightable> playerOptions;

	// gui stuff
	public Text curPlayerText;
	public Text curPhaseText;
	public Text changesRemaining;
	public GameObject gearChangeCanvas;
	public Text movesRemaining;


	public Unit selectedUnit;

	void Awake(){
		S = this;
		selectedUnit = null;
	}

	public void TileClicked(Tile tile){

	}

	public void UnitClicked(Unit unit){
		selectedUnit = unit;
	}

	public void OptionClicked(int moveState){
		// convert from int to enum and try not to break it!
		if (turnPhase != turnState_e.change)
						return;
		if (selectedUnit == null)
						return;
		selectedUnit.SetState ((MoveState_e)moveState);
		//selectedUnit.UnSelect ();
		UnSelectUnit ();
		++receivedChangeOrders;
	}

	// Use this for initialization
	void Start () {
		receivedMoveOrders = 0;
		receivedChangeOrders = 0;
		//changesRemaining.enabled = false;
		gearChangeCanvas.SetActive(false);
		movesRemaining.enabled = false;
		StartCoroutine (MovePhase ());
		playerOptions = null;
		
	}
	
	// Update is called once per frame
	void Update () {
		// just for stuff that is always active.
	}



	IEnumerator MovePhase(){
		receivedMoveOrders = 0;
		movesRemaining.enabled = true;
		if (activePlayer == Team_e.red) {
			while ((moves_per_turn - receivedMoveOrders) > redTeam.Count){
				++receivedMoveOrders;
			}
		} else {
			while ((moves_per_turn - receivedMoveOrders) > blueTeam.Count){
				++receivedMoveOrders;
			}
		}
		while (receivedMoveOrders < moves_per_turn) {
			if (selectedUnit && playerOptions == null){
				if (!selectedUnit.Attacker()){
					playerOptions = selectedUnit.GetValidMoves();
				}
				else {
					playerOptions = selectedUnit.GetValidAttacks();
				}
				if (playerOptions != null){
					foreach(Highlightable h in playerOptions){
						h.Highlight();
					}
				}
			}
			yield return null;
		}
		movesRemaining.enabled = false;
		turnPhase = turnState_e.change;
		StartCoroutine (ChangePhase ());
	}

	IEnumerator ChangePhase(){
		receivedChangeOrders = 0;
		if (activePlayer == Team_e.red) {
			foreach(Unit u in redTeam){
				if (u.moved)
					++receivedChangeOrders;
			}
			while ((changes_per_turn - receivedChangeOrders) > redTeam.Count)	
				++receivedChangeOrders;
		} else {
			foreach(Unit u in blueTeam){
				if (u.moved)
					++receivedChangeOrders;
			}
			while ((changes_per_turn - receivedChangeOrders) > blueTeam.Count)
				++receivedChangeOrders;
		}
		//changesRemaining.enabled = true;
		gearChangeCanvas.SetActive(true);
		while (receivedChangeOrders < changes_per_turn) {
			yield return null;		
		}
		// set everyone to not moved
		if (activePlayer == Team_e.red) {
			foreach(Unit u in redTeam)
				u.moved = false;
		} else {
			foreach(Unit u in blueTeam)
				u.moved = false;
		}
		// switch active player
		if (activePlayer == Team_e.red) {
			activePlayer = Team_e.blue;		
		} else if (activePlayer == Team_e.blue) {
			activePlayer = Team_e.red;
		}
		turnPhase = turnState_e.movement;
		//changesRemaining.enabled = false;
	
		gearChangeCanvas.SetActive(false);
		StartCoroutine (MovePhase ());
	}

	public void OnGUI(){
		movesRemaining.text = "Moves Remaining: " + (moves_per_turn - receivedMoveOrders);
		changesRemaining.text = "Gear Changes Remaining: " + (changes_per_turn - receivedChangeOrders);
		if (activePlayer == Team_e.blue) {
						curPlayerText.color = Color.blue;
						curPlayerText.text = "Current Player: Blue";
		} else {
			curPlayerText.color = Color.red;
			curPlayerText.text = "Current Player: Red";
		}
		if (turnPhase == turnState_e.movement) {
						curPhaseText.text = "Current Phase: Movement";		
		} else {
			curPhaseText.text = "Current Phase: Gear Change";		
		}
	}

	public void SelectUnit(Unit unit){
		if (unit.moved)
						return;
		if (activePlayer != unit.team) {
			if (turnPhase == turnState_e.movement){
				SelectEnemy (unit);
				return;
			}	
			else return;
		} 
		// if the unit belongs to the current player, we want to select it.
		UnSelectUnit ();
		selectedUnit = unit;
		selectedUnit.Highlight ();
	}

	public void UnSelectUnit(){
		if (selectedUnit != null)
			selectedUnit.Highlight (); // unhighlight this unit
			if (playerOptions != null) {
				foreach(Highlightable h in playerOptions){
					h.Highlight();
				}	
				playerOptions.Clear ();
				playerOptions = null;
			}
		selectedUnit = null;

	}

	// called by a tile that is clicked, used to facilitate movement
	// if that tile is a destination option
	public void SelectTile(Tile tile){
		if (turnPhase != turnState_e.movement)
						return;
		if (tile.unit != null)
						return;
		if (playerOptions == null)
						return;
		if (playerOptions.Contains (tile)) {
			// tile is a vaild location to move to!
			selectedUnit.MoveTo(tile);
			CheckForGameEnd(selectedUnit);
			++receivedMoveOrders;
			UnSelectUnit();
		}
	}

	// called by a unit that is clicked, used to facilitate
	// attacks against that unit
	void SelectEnemy(Unit unit){
		if (playerOptions == null)
						return;
		if (playerOptions.Contains (unit)) {

			if (selectedUnit.moveState == MoveState_e.attack){ // special attack rules, includes a move
				Tile targetTile = unit.currentSpace;
				unit.Remove ();
				selectedUnit.MoveTo(targetTile);
			}	
			else {
				// just remove the unit
				selectedUnit.moved = true;
				unit.Remove ();
			}
			++receivedMoveOrders;
			UnSelectUnit();
		}
	}

	public void EndTurn(){
		UnSelectUnit ();
		receivedMoveOrders = moves_per_turn;
		receivedChangeOrders = changes_per_turn;
	}

	// check if this unit is on its enemy's flag, if so, cue fireworks
	void CheckForGameEnd(Unit unit){
		if (unit.team == Team_e.red && unit.currentSpace.flag == Flag_e.blue){
			EditorUtility.DisplayDialog("Winner", "Red wins!", "nice");
			GameEnd(Team_e.blue);
		}
		else if (unit.team == Team_e.blue && unit.currentSpace.flag == Flag_e.red){ 
			EditorUtility.DisplayDialog("Winner", "Blue wins!", "nice");
			GameEnd(Team_e.red);
		}
	}

	void GameEnd(Team_e victor){
		print ("Game over, yo");
		StopAllCoroutines ();
		// display some awesome message indicating who won
	}
}