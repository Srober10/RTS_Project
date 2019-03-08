using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "PluggableAI/Decisions/TroopCountHigherThan")]
public class TroopCountDecision : Decision {

    public int minTroopCount =20;

    public override bool Decide(StateController controller) {

        return ChangeState(controller);
    }

    // Use this for initialization
    void Start () {
		
	}

    //changes state if troop count higher
    private bool ChangeState(StateController controller) {
        if( controller.self.GetArmyTroopCount() >= minTroopCount) {
            return true;
        }
        else {
            return false;
        }
    }
	
}
