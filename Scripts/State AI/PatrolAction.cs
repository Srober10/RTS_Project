using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Patrol")]
public class PatrolAction : StateAction {
    public override void Act(StateController controller) {

        Debug.Log("called patrol action");
        Patrol(controller);

    }

    private void Patrol(StateController controller) {
        Debug.Log("waypoint list" + controller.wayPointList + "and count: " + controller.wayPointList.Count);
        controller.navMeshAgent.destination = controller.wayPointList[controller.nextWayPoint].position;
        controller.navMeshAgent.isStopped = false;

        if (controller.navMeshAgent.remainingDistance <= controller.navMeshAgent.stoppingDistance && !controller.navMeshAgent.pathPending) {
            controller.nextWayPoint = (controller.nextWayPoint + 1) % controller.wayPointList.Count;
        }
    }
}
