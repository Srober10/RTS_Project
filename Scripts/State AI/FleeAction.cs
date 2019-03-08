using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Flee")]
public class FleeAction : StateAction {
    public override void Act(StateController controller) {
        Flee(controller);
    }

    private void Flee(StateController controller) {

        Debug.Log("FLEEEEEEEE!");
        Vector3 dirToPlayer = controller.transform.position - controller.closestEnemyLocation;
        Vector3 newPosition = controller.transform.position + dirToPlayer;

        controller.navMeshAgent.destination = newPosition; //works?
        controller.navMeshAgent.isStopped = false;
    }
}