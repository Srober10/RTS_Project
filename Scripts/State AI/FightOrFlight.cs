using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/FightOrFlight")]
public class FightOrFlight : Decision {

    // Use this for initialization
    private int fleeRadius = 8;

    public override bool Decide(StateController controller) {
        bool fight = FightvFlight(controller);
        Debug.Log(fight + " fight bool");
        return fight;
    }


    // compares nearby enemy strength returns true if weaker enemy nearby, else returns false
    // check nearby enemies, if all are weaker then chase closest
    // if not all are weaker but closest is weakest then chase closest
    // if there is a stronger enemy within (x) distance then flee 
    //using GETARMYPOWER
    //massive overflow to negative idk why?
    public bool FightvFlight(StateController controller) {
        if (controller.AreaLook()) {
            //Debug.Log("calling arealook");
            var enemies = controller.nearbyEnemies;
            List<EnemyInfo> closeEnemies = new List<EnemyInfo>();
            foreach (Challenger e in enemies) {
               // Debug.Log("enemy power is : " + e.GetArmyPower() + " my power is " + controller.self.GetArmyPower() );
                closeEnemies.Add(new EnemyInfo(e.GetArmyPower(), Vector2.Distance(controller.transform.position, e.transform.position), e));
            }
            //runs if theres a stronger enemy within flee radius, otherwise chases
            foreach (EnemyInfo p in closeEnemies) {
                if (p.GetPower() > controller.self.GetArmyPower() && p.GetDistance() < fleeRadius) {
                   // Debug.Log(p.GetPower() + "(player) nearby power! more than mine of: " + controller.self.GetArmyPower());
                    controller.closestEnemyLocation = p.challenger.transform.position; //set position to use in flee function
                    Debug.Log("stronger enemy detected, setting to fleeee");
                    return false;
                }
            }
            return true;
        }
        return false;
    }

}
public class EnemyInfo {
    private int power;
    private float distance;
    public Challenger challenger;

    public EnemyInfo( int p, float d, Challenger c) {
        power = p;
        distance = d;
        challenger = c;
    }
    public int GetPower() {
        return power;
    }
    public float GetDistance() {
        return distance;
    }
}