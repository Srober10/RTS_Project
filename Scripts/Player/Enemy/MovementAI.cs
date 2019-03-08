using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;



//depreciated, using statemachine with this behaviour as a state
public class MovementAI : MonoBehaviour {

    [SerializeField] float maxHealthPoints = 100;
    [SerializeField] float triggerRadius = 50;

    float currentHealth = 100f;
    [SerializeField]  AICharacterControl aiCharacter = null;
    //GameObject[] playerPos  ; // array of all player controlled armies
    Challenger[] challengerArray;
    int estimatedPower = 1;
    [SerializeField] Challenger self;
    public float stoppingDistance = 2f;
    private Transform startTransform; //used to calculate flee
    [SerializeField] private NavMeshAgent NMAgent;

    //enemy only neeeds to keep track of list of closeby enemies/friends
    private List<Challenger> allies;
    private List<Challenger> enemies;

    private void Start() {
      
        List<Challenger> allies = new List<Challenger>();
        List<Challenger> enemies = new List<Challenger>();
        
        self = GetComponent<Challenger>(); //not setting whttdxl as;a
    }

    //spherecast from centre of this gameobject to radius distance away. 
    public void GetCloseChallengers() {
        RaycastHit[] hits;
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, triggerRadius);
        //making new lists to jsut add to avoid having to remove (check efficiency vs removing and maintaining a list)
        allies = new List<Challenger>();
        enemies = new List<Challenger>();
        // Debug.Log( hitColliders.Count()); this is returning some numbers 
        foreach (Collider c in hitColliders) {
            //if collider is a challenger then check if its an enemy and add it to 
            Challenger chall = c.GetComponent<Challenger>();          
            if (chall) {
               // Debug.Log("at least one not null!");
                if (chall.faction.factionName .Equals( self.faction.factionName)) {
                    if (allies.Contains(chall) ) {
                        return;
                    }
                    allies.Add(c.GetComponent<Challenger>());                    
                } 
                else {
                    if (enemies.Contains(chall) ) {
                        return;
                    }
                    enemies.Add(c.GetComponent<Challenger>());
                }
            }
        }
    }


    public float HealthAsPercentage {
        get {
            return currentHealth / maxHealthPoints;
        }
    }

    //TODO move out of update and swap to something less frequent ?? maybe not 
    //Controls chase / flee behaviour
    private void Update() {
        //if not currently busy or interacting etc
      //  Debug.Log(self.faction.factionName);
        //Debug.Log(self.name + " enemies: " + enemies.Count + "allies: " + allies.Count);
        GetCloseChallengers();
        //only calcuate AI move if enemies isnt null or it throws null pointer 
        if (enemies != null) {
           // Movement();
        } else {
            //do movement related to goals
        }
    }


    //finds closest killable enemy and if is within stopping radius, interacts. If the closest enemy is stronger it tries to flee
    /* private void Movement() {
        //Debug.Log("Trigger radius: " + triggerRadius);
        //Debug.Log("closest: " + findClosestEnemyDist());
        //Debug.Log(findClosestEnemyDist() + ": closest enemy dist!");
                if (FindClosestEnemyDist() <= stoppingDistance) {
                    aiCharacter.SetTarget(this.transform);
                    //Debug.Log("Engaging enemy!");
                    //engage enemy code below:::: TODO

                }
                else {
                    //pick to chase or flee
                    var closestEnemy = FindClosestEnemy();
                    if (closestEnemy) {
                        //if (closestEnemy.GetComponent<Challenger>().EstimatedArmyPower() > self.EstimatedArmyPower()) {
                            RunFrom(closestEnemy);
                      //  }
                        else {
                            aiCharacter.SetTarget(FindClosestEnemy());
                            //Debug.Log("Attempting to reach enemy!");
                        }
                    }
                }
           
        

    } */

    public void RunFrom(Transform enemyLocation) {
       // Debug.Log("should be fleeing");
        Vector3 dirToPlayer = transform.position - enemyLocation.position;
        Vector3 newPosition = transform.position + dirToPlayer;
        //Debug.Log(self + "moving from :" + transform.position + "To : " + newPosition);
        // And get it to head towards the found NavMesh position
       // Debug.Log("path is " + NMAgent.SetDestination(newPosition));
        //NMAgent.SetDestination ( newPosition );  //Debug.Log(NMAgent.SetDestination(newPosition)); this doesnt move them at all.... 

        // NMAgent.Move(newPosition); //this moves them instantly away :/ >???
        NMAgent.destination = newPosition; //yesss
        NMAgent.isStopped = false;

    }

    // current closest enemy 
    //if the current distance is further than the new distance then we update the current distance
    private float FindClosestEnemyDist() {
        float currentClosest = triggerRadius*1000;
         foreach(Challenger e in enemies) {
            if (currentClosest > Vector3.Distance(e.transform.position, transform.position)) {
                currentClosest = Vector3.Distance(e.transform.position, transform.position);
            }

        }
        //Debug.Log("Distance : " + currentClosest);
        return currentClosest;
    }

    //chase behaviour
    private Transform FindClosestEnemy() {
        float currentClosest = triggerRadius * 1000;
        Transform currentTransform = null;
        foreach (Challenger e in enemies) {
            if (currentClosest > Vector3.Distance(e.transform.position, transform.position)) {
                currentClosest = Vector3.Distance(e.transform.position, transform.position);
                currentTransform = e.transform;
            }

        }
       // Debug.Log("Closest enemy: " + currentTransform);
        return currentTransform;
    }


}
