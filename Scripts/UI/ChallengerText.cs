using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengerText : MonoBehaviour {

    public GameObject chalGUI;
    public Text descText;
    public Text nameText;
    public Button attack;
    public Button retreat;
    public Challenger player; //assigned in inspectr
    public Challenger challenger;
	// Use this for initialization
	void Start () {
        // initial values set in editor

	}
	
    public void SetText(Challenger opponent) {

        if (player.interactingWith != null) {
            challenger = player.interactingWith;
            nameText.text = challenger.name;
            descText.text = "I am from faction " + challenger.faction + " and I have " + challenger.GetArmy().Count + " squadrons, totalling " + challenger.GetArmyTroopCount() + " troops."
                        + " \n Weaker than us: " + player.StrongerThanOpponent(challenger.GetArmy()).ToString() + " \n Their faction is " + " at war " + "with yours";
        }
        //try go
        else {
            nameText.text = "null challenger";
            descText.text = "???? description";
        }

    }

    public void ShowChalGUI() {
        chalGUI.SetActive( true);
    }

    // not in use atm 
    public void AttackButton() {

        //challenger = player.interactingWith; // obsolete?
        player.CombatCalculation(challenger);
    }
    public void RetreatButton() {
        //runs away (closes)

    }
}
