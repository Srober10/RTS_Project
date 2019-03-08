using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "PluggableAI/Actions/Upgrade")]
public class UpgradeAction : StateAction {

    public State[] moveToState; // move to this state once upgrade is called. could also mkae this a list and pick randomly from list of options.. 

    int rand; // use this to pick which unit to upgrade first ?

    // forced move not being used at the moment, instead a decision on gold is being used
    public override void Act(StateController controller) {
        UpgradeOptions(controller);
        int rand = UnityEngine.Random.Range(0, moveToState.Length);
       // controller.TransitionToState(moveToState[rand]);
    }

    // Use this for initialization
    void Start () {
		
	}
	
    //gets the upgrade options for each unit
    // greedy upgrade system, loops through all units in order and upgrades if possible, picking random upgrade
    //we only want to call this once so as soon as the action is completed we can change state, don't need to return a bool because we swap state regardless of if upgrade was possible
    public void UpgradeOptions(StateController controller) {
        Debug.Log("AI Upgrading troops ");
        List<Unit> army = controller.self.GetArmy();
        bool upgrading = true;
        foreach(Unit u in army) {
            if(upgrading && controller.self.gold > u.upgradeCost * u.currentSize) { //can upgrade randomly
                var ugs = u.PossibleUpgrades();
                int rand = UnityEngine.Random.Range(0, ugs.Count);
                List<Action> upgrade = new List<Action>(); //puts into a list of one to call
                upgrade.Add(ugs[rand]);
                foreach(Action a in upgrade) {
                    a();
                }
                controller.self.gold -= (u.upgradeCost * u.currentSize);
              
            }
            else {
                upgrading = false;
               
            }

        }
        if (!upgrading) {
            if (moveToState.Length > 0) {
                controller.TransitionToState(moveToState[0]); //moves out to different state
            }
        }
    }

   
}
