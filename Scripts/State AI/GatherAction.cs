using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Gather")]
public class GatherAction : StateAction {
    
    private bool destinationSet = false;

    public override void Act(StateController controller) {
       Gathering( controller);
    }

    void Start() {
        destinationSet = false;
    }
    //sets destination to nearest village
    //when destination is reached gather resources
    
    private void Gathering(StateController controller) {
        //Debug.Log("GatherAction ?? " + destinationSet + "helo");
        //if(!destinationSet)
        SetVillageDestination(controller);
        // checking if navmesh agent is at destination
        if (ArrivedAtLocation(controller)) {
            if (controller.targetLocation != null) {
                destinationSet = false;
                Village v = (Village)controller.targetLocation;
                v.AIGather(controller.self);
               // Debug.Log("enemy at targte destination of : " + controller.targetLocation.ToString());
                //clearing village CDlist if exceeds a random amount
                int rand = UnityEngine.Random.Range(1, controller.villageList.Count);
                if (controller.villageCDlist.Count > rand) {
                    controller.villageCDlist.Clear();
                }
                controller.villageCDlist.Add(v);                
               
            }
        }
    }  
    public void SetVillageDestination(StateController controller) {
        
           //Debug.Log("Start of gathering action");
            var closestVil = controller.ClosestVillageToGather();
            if (closestVil != null) {
                controller.navMeshAgent.destination = closestVil.villagePosition;
                controller.targetLocation = closestVil;
                controller.navMeshAgent.isStopped = false;
                destinationSet = true;
            }
            else {
                Debug.Log("no close villages to gather from? error?");
            }
        
    }
    private bool ArrivedAtLocation(StateController controller) {
        if (!controller.navMeshAgent.pathPending) { //and ismoving??
            if (controller.navMeshAgent.remainingDistance <= controller.navMeshAgent.stoppingDistance) {
                if (!controller.navMeshAgent.hasPath || controller.navMeshAgent.velocity.sqrMagnitude == 0f) {
                    // collect resources from village, add village to list of cdvillages                    
                    return true;
                    
                }
            }
        }
        return false;
    }
}
