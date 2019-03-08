using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Hire")]
public class HireAction : StateAction {

    public State backupState; //this broke it??
    private bool isMoving = false;
    private bool arrived = false;

    public override void Act(StateController controller) {
        arrived = false; //needed here to reset, changed to true in hiring if arrived
        SetTownDestination(controller);
        Hiring(controller);
    }
   
    public void Awake() {
        arrived = false;
    }
    private void Hiring(StateController controller) {

        // checking if navmesh agent is at destination      
        if (ArrivedAtLocation(controller)) {
            if (controller.targetLocation != null) {
                try {
                    Town t = (Town)controller.targetLocation;
                    Debug.Log("Hiring action called");
                    t.AIHire(controller.self);
                    //clearing village CDlist if exceeds a random amount
                    if (controller.townCDlist.Count > 2) { //TODO change this later
                        controller.townCDlist.Clear();
                    }
                    controller.townCDlist.Add(t);
                }
                catch (InvalidCastException e) {
                    Debug.Log("Cast exception error here?") ;
                } 
            }
        }    
    }

    public void SetTownDestination(StateController controller) {

        Debug.Log("set town destination called");
        var closestTown= controller.ClosestTownToHire();
        if (closestTown != null && controller.navMeshAgent.SetDestination(closestTown.townPos)) {
            controller.navMeshAgent.destination = closestTown.townPos;
            controller.targetLocation = closestTown;
            controller.navMeshAgent.isStopped = false;
            //destinationSet = true;
        }
        // no valid path to this town !
        else if( ! controller.navMeshAgent.SetDestination(closestTown.townPos)) {
            Debug.Log("nav mesh error, path to target is not valid. moving to different state and removing the destination from list");
            controller.self.faction.towns.Remove(closestTown);
            Debug.Log("town destroyed by environment :o ");
           // SetTownDestination(controller); //recall method causing infinite loop??
            //controller.currentState = backupState;
        }
       else {
            Debug.Log("no close towns to hire from? error? moving to different state");
            controller.currentState = backupState;
        }

    }
    private bool ArrivedAtLocation(StateController controller) {

       // Debug.Log("arrived at location called");
        if (!controller.navMeshAgent.pathPending) { //and ismoving??
            if (controller.navMeshAgent.remainingDistance <= controller.navMeshAgent.stoppingDistance) {
                if (!controller.navMeshAgent.hasPath || controller.navMeshAgent.velocity.sqrMagnitude == 0f) {
                    // collect resources from village, add village to list of cdvillages      
                    Debug.Log("Arrived at locationnn is true");
                    arrived = true;
                    return true;

                }
            }
        }
        return false;
    }
}
