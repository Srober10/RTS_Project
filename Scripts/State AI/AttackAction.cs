using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Attack")]
public class AttackAction : StateAction {

    [SerializeField] public Challenger playerRef;
    [SerializeField] public ChallengerText text;
    //todo move these 
    int attackRange = 2;

    public override void Act(StateController controller) {
        Attack(controller);
    }

    private void Start() {
        playerRef = GameObject.FindWithTag("Player").GetComponent<Challenger>();
       
    }

    //TODO fleeing? timer for fleeing?
    private void Attack(StateController controller) {
        RaycastHit hit;
        Debug.Log("attack being called");
        Debug.DrawRay(controller.eyes.position, controller.eyes.forward.normalized * attackRange, Color.red);
        ChallengerText chaltext = controller.manager.GetComponent<ChallengerText>();

        try {
            if (Physics.SphereCast(controller.eyes.position, attackRange, controller.eyes.forward, out hit, attackRange)
                && !hit.collider.GetComponent<Challenger>().faction.Equals(controller.faction)) {   //TODO vfirst few frames this second part is null ?? not sure why ?

                Challenger target = hit.collider.GetComponent<Challenger>();
                BattleGUI gui = controller.manager.GetComponent<BattleGUI>();
                gui.enemy = controller.self; //sets battle gui enemy to this enemy
                                             //setting an showing challenger interaction GUI
                controller.retreatButton.interactable = false; // turn off retreat options btton
                                                               // needed if more than 1v1:
                target.interactingWith = controller.self; //sets player to be interacting with this attacking unit
                controller.self.interactingWith = target; //sets self to interacting with player
                controller.navMeshAgent.isStopped = true; //stops?
                chaltext.SetText(controller.self); //sets text
                chaltext.ShowChalGUI();

            }
        } catch (NullReferenceException e) {
            Debug.Log("First x frames seem to throw null ref??");
        }
    }
}
