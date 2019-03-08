using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Decisions/Look")]
public class LookDecision : Decision {
    //todo move theses to different class
    int lookRange = 20;
    int lookRadius = 5;
    //
    List<Challenger> allies;
    public override bool Decide(StateController controller) {
        bool targetVisible = Look(controller);
        return targetVisible;
    }


    public void SetUp() {
    }

    //only looks in a straight line from the front of the player - swapped for the radius method below as player can see around themselves so AI should too
    private bool OldLook(StateController controller) {
        allies = controller.allies;
        RaycastHit hit;
        Debug.DrawRay(controller.eyes.position, controller.eyes.forward.normalized * lookRange, Color.green);

        // if in range and the faction is not hte sa me as the hit faction
        if (Physics.SphereCast(controller.eyes.position, lookRadius, controller.eyes.forward, out hit, lookRange)
            && !hit.collider.GetComponent<Challenger>().faction.Equals(controller.faction)) {
            controller.chaseTarget = hit.transform;
            return true;
        }
        return false;
    }
    
    // looking in a radius around the player
    private bool Look(StateController controller) {
        controller.UpdateClosestEnemy();
        if(controller.chaseTarget != null) {
            return true;
        }
        return false;
    }
}
