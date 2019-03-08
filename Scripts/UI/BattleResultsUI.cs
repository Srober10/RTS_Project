using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleResultsUI : MonoBehaviour {

    //assign in inspector
    public MenuManager manager;
    public GameObject resultsGUI;
    Challenger player;
    Challenger enemy;
    public bool playerWon = false;

    public Text victory;

	// Use this for initialization
	void Start () {
        player = manager.player;
	}
	
	public void HideResults() {
        resultsGUI.SetActive(false);
    }

    public void SetResults() {

     enemy = player.interactingWith;
     Debug.Log("enemy army?? " + enemy.GetArmy());
     playerWon = player.StrongerThanOpponent(enemy.GetArmy());
        if (playerWon)
            victory.text = "Victory!";
        else {
            victory.text = "Defeat!";
        }
     //player.RemoveTroopCasualties(enemy); already done this?
    }
}
