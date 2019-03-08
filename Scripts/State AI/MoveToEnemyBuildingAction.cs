using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//decisions handled in code as requires less code and is cleaner
[CreateAssetMenu(menuName = "PluggableAI/Actions/MoveToBuilding")]
public class MoveToEnemyBuildingAction : StateAction {

    private Faction enemyFac;
    //public Decision 
    public State failureState;
    public bool arrived;
    //private bool destinationSet = false;
    public override void Act(StateController controller) {
        MoveToBuilding(controller);
        Attack(controller);
    }
    
    // gets the players faction and list of cities and villages from this 
    void Start() {
       // enemyFac = GameObject.FindGameObjectWithTag("Player").GetComponent<Challenger>().faction;
    }
    
    private void Attack( StateController controller) {
        if (ArrivedAtLocation(controller) && controller.targetLocation != null) {
            Debug.Log("Controller null?? " + controller);
            Debug.Log("Controller self null?? " + controller.self);
            var aiPlayer = controller.self;
            //if successful recall method
            aiPlayer.location = controller.targetLocation;
            if (aiPlayer.location.GetFaction().Equals(controller.self.faction)) {
                Debug.Log("trying to take own city??");
                return ;
            }
            if (aiPlayer.location.Attack(aiPlayer)) { //if wins then we stay in this state
                Debug.Log("Enemy AI took a building!");
                //MoveToBuilding(controller);
            }
            else {
                Debug.Log("Enemy AI failed to take a building!"); //if we lose then we move to another state
                Debug.Log("Does this work?");
                controller.TransitionToState (failureState); //this owrks?
            }

        }
    }
    private void MoveToBuilding(StateController controller) {
        //if (enemyFac == null)
            // enemyFac = controller.player.faction;
        var destination = controller.ClosestEnemyBuilding();
        if (destination == null) {//this owrks?
            controller.TransitionToState(failureState);
            return;
        }
        if (destination != null && controller.navMeshAgent.SetDestination(destination.position)) {
            controller.navMeshAgent.destination = destination.position;
            Debug.Log(" this is enemy structures name: " + controller.ClosestEnemyBuilding().GetName());
            controller.targetLocation = destination;
            controller.navMeshAgent.isStopped = false;
        }
        else {
            //Debug.Log("building is : " + destination + "path valid: " + controller.navMeshAgent.SetDestination(destination.position));
            if(!controller.navMeshAgent.SetDestination(destination.position)) { //path is null so remove from list of posible destinations
                controller.invalidPathTo.Add(destination);
            }
        }
    }

    private bool ArrivedAtLocation(StateController controller) {
        if (!controller.navMeshAgent.pathPending) { //and ismoving??
            if (controller.navMeshAgent.remainingDistance <= controller.navMeshAgent.stoppingDistance) {
                if (!controller.navMeshAgent.hasPath || controller.navMeshAgent.velocity.sqrMagnitude == 0f) {
                    // collect resources from village, add village to list of cdvillages      
                    Debug.Log("Arrived at locationnn");
                    //destinationSet = false;
                    arrived = true;
                    return true;

                }
            }
        }
        return false;
    }
}
