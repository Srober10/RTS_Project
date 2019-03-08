using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "PluggableAI/Decisions/AttackStructure")]
public class AttackStructureDecision : Decision {


    public override bool Decide(StateController controller) {
        return StructureCapturable(controller);

    }

    // Use this for initialization
    void Start () {
		
	}

    // returns true if structure is in range and weaker, else false
	private bool StructureCapturable(StateController controller) {
        return false;

    }

}
