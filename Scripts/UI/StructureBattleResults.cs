using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StructureBattleResults : MonoBehaviour {

    //assign in inspector
    public MenuManager manager;
    public GameObject resultsGUI;
    Challenger player;
    IInteractable enemy;
    public bool playerWon = false;

    public Text victory;
    public Text infoText;

    // Use this for initialization
    void Start() {
        player = manager.player;
    }

    public void HideResults() {
        resultsGUI.SetActive(false);
    }

    public void SetResults() {
        resultsGUI.SetActive(true);
        enemy = player.location;
        Debug.Log("enemy army?? " + enemy.GetArmyList());
        playerWon = player.StrongerThanOpponent(enemy.GetArmyList());
        if (playerWon) {
            victory.text = "Victory!";
            infoText.text = "This " + enemy.ToString() + "is now yours!";
            if (enemy.GetBuildingType().Equals(BuildingType.Capital)) {
                infoText.text = "Congratulations, you have taken the enemies capital and won the game!";
            }
        }
        else {
            victory.text = "Defeat!";
            infoText.text = "The enemy successfully defended.";
        }
        //player.RemoveTroopCasualties(enemy); already done this?
    }
}
