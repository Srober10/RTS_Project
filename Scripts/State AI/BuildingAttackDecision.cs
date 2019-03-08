using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "PluggableAI/Actions/BuildingAttack")]
public class BuildingAttackAction : StateAction {

    private int rand;
    public int attackChance = 50;
    public float refreshRNGrate = 10f;

    bool started = false;
    public int counter = 0;
    public int counterThreshold = 1000;


    public override void Act(StateController controller) {
        throw new NotImplementedException();
    }
    
    // Use this for initialization
    void Start () {
        SetRandValue();
	}
    
    

    public void SetRandValue() {
        rand = UnityEngine.Random.Range(0, 100);
    }

}
