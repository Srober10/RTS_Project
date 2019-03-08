using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/GoldHigherThan")]
public class GoldHigherThanDecision : Decision {

    public int minGold = 20; //can change this for other 
    public int maxGold = 100;
    private int rand = 30;
    bool started = false;
    public int counter =0;
    public int counterThreshold = 300;

    // cant invoke repeating as inheriting from decision and not monobehaviour
    public override bool Decide(StateController controller) {

        return ChangeState(controller);
    }

    // Use this for initialization
    void Start () {
        //InvokeRepeating("SetRandValue", 0, 1.0);  //sets this every 5s cant invoke repeating as inheriting from decision and not monobehaviour
        SetRandValue();
    }
    

    //todo this random number checked every update so will tend towards as soon as over 20 resources ... fix this once statemachine working 
    // >> > >> > iddkk linear relationship of p to move on to resources 
    // only hire if above minimum of 21


    //removed randomness for now
    private bool ChangeState(StateController controller) {
        if (!started) {
            SetRandValue();
            started = true;
        }
        if (controller.self.gold >= rand) {
            counter++;
            if(counter > counterThreshold) {
                SetRandValue();
                counter = 0;
            }
            return true;
        }
        else {
            counter++;
            if (counter > counterThreshold) {
                SetRandValue();
                counter = 0;
            }
            return false;
        }
        
    }

    // only call this per x seconds / function call
    public void SetRandValue() {
        rand =  UnityEngine.Random.Range(minGold, maxGold); 
    }
}
