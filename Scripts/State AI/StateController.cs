using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class StateController : MonoBehaviour {   

    public State currentState;
   // public EnemyStats enemyStats; // get from enemy or create more easily accessable Scriptable obj to hold??
    public Transform eyes;
    public State remainState; //dummy state to cover the case of no state change.
    public Faction faction;
    public Challenger self;
    public Challenger player; // todo remove this is more than 1v1 is implemented 
    public Faction enemyFaction; // as above
    public List<Village> villageList, villageCDlist;    

    public List<Town> townList, townCDlist;

    public int triggerRadius = 15;
    public List<Challenger> allies;
    public List<Challenger> nearbyEnemies;
    public Vector3 closestEnemyLocation;
    public Vector3 closestVillage;
    public IInteractable targetLocation;
    [HideInInspector] public NavMeshAgent navMeshAgent;
   // [HideInInspector] public Complete.TankShooting tankShooting;
    [SerializeField] public List<Transform> wayPointList;
    [HideInInspector] public int nextWayPoint;
    [HideInInspector] public Transform chaseTarget;
    [HideInInspector] public float stateTimeElapsed;
    public MenuManager manager; // show menu when attacking 
    public Button retreatButton; //set to inactive on being attacked
    public List<IInteractable> invalidPathTo; 

    //
    List<Village> enemyVilList;
    List<Town> enemyTownList;

    private bool aiActive;


    void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        self = GetComponent<Challenger>();
        faction = self.faction;
        allies = faction.challengers; //TODO these arent set up in inspector yet as its 1 v 1 atm
        navMeshAgent.enabled = true;       
        villageCDlist = new List<Village>();
        townCDlist = new List<Town>();
        invalidPathTo = new List<IInteractable>();
        aiActive = true;
    }

    private void Start() {

        villageList = faction.villages; //error: this is length of 8 but with entries being null....

        //TODO change for more than 1v1
        enemyFaction = player.faction;
        enemyVilList = enemyFaction.villages;
        enemyTownList = enemyFaction.towns;
        InvokeRepeating("SlowerUpdate", 0f, 3f); //repeats update every second *!!( dont want to scale this with time for ML though..) !***
    }


    //updates and returns the closest village not in CD village list
    // null also needs to be handled when this is called
    public Village ClosestVillageToGather() {
        //gets village with minimum distance and sets as next location to gather from
       // Debug.Log("is this null. ? " + self.faction.villages[1]);
        villageList = self.faction.villages;
        float minDistance = 10000;
        Village closestVil = null;
       // Debug.Log(villageList.Count + " : list of village list");
        foreach( Village v in villageList) {
            if( villageCDlist.Count == 0 || ! villageCDlist.Contains(v) && v !=null ) {
                float dist = Vector3.Distance(this.transform.position, v.villagePosition);
                if ( dist < minDistance) {
                    minDistance = dist;
                    closestVil = v;
                }
            }
        }
        if (closestVil != null) {
          //  Debug.Log("closest village is " + closestVil.buildingName);

            return closestVil;
        }
        else return null;
    }

    //updates and returns the closest town not in CD town list
    //gets town with minimum distance and sets as next location to hire from
    public Town ClosestTownToHire() {
        townList = self.faction.towns;
        float minDistance = 10000;
        Town closestTown = null;
        //Debug.Log(townList.Count + " : n of town list");
        foreach (Town t in townList) {
            if (townCDlist.Count == 0 || !townCDlist.Contains(t)) {
                float dist = Vector3.Distance(this.transform.position, t.townPos);
                if (dist < minDistance) {
                    minDistance = dist;
                    closestTown = t;
                }
            }
        }
        if (closestTown != null) {
           //Debug.Log("closest town is " + closestTown.buildingName);

            return closestTown;
        }
        else return null;
    }

    public IInteractable ClosestEnemyBuilding() {
        //looks through enemy capital/town/village and returns closest one 

        float capitalDist;
        float townDistance;
        float villageDistance;
        float minDistance = 10000;
        //incase towns have changed etc
        enemyVilList = enemyFaction.villages;
        enemyTownList = enemyFaction.towns;

        if (enemyFaction.capital != null) {
            capitalDist = Vector3.Distance(self.transform.position, enemyFaction.capital.GetPosition().position);
        }
        else {
            capitalDist = minDistance;
            Debug.Log("enemy capital is null>> erroor");
        }
        Town closestTown = null;
        //Debug.Log(townList.Count + " : n of town list");
        foreach (Town t in enemyTownList) {
            if (! invalidPathTo.Contains(t)) {
                float dist = Vector3.Distance(this.transform.position, t.townPos);
                if (dist < minDistance) {
                    minDistance = dist;
                    closestTown = t;
                }
           }
        }
        Village closestVil = null;
        //Debug.Log(townList.Count + " : n of town list");
        foreach (Village v in enemyVilList) {
            if (!invalidPathTo.Contains(v)) {
                float dist = Vector3.Distance(this.transform.position, v.villagePosition);
                if (dist < minDistance) {
                    minDistance = dist;
                    closestVil = v;
                }
            }
        }

        if (closestTown != null ) {
             townDistance = Vector3.Distance(self.transform.position, closestTown.GetPosition().position);
        }
        else {
            townDistance = minDistance;
        }
        if (closestVil != null) {
            villageDistance = Vector3.Distance(self.transform.position, closestVil.GetPosition().position);
        }
        else {
            villageDistance = minDistance;
        }

        // finds smallest of thses values and returns associated object
        if (villageDistance <= townDistance && villageDistance <= capitalDist) {
            return closestVil;
        }
        else if (townDistance <= villageDistance && townDistance <= capitalDist) {
            return closestTown;
        }
        else return enemyFaction.capital;


    }

    //everyone not an ally is an enemy,
    //faction initialisation may not be working 
    //thse 2 methods in awake and start functions now
    private void SetupAllies() {
       Debug.Log("Ally count: " + allies.Count);
    }
    //currently assumes only 1 enemy faction
    //loops through all objects in the game with IStructure tag, and if owned by the faction then adds them to list to patrol between
    public void SetupAI(bool activateAI) {
        SetupAllies();
        //this isnt initiallising..
        /*var allStructures = FindObjectsOfType<MonoBehaviour>().OfType<IStructure>();
        foreach (IStructure s in allStructures) {
            if (s.GetFaction().Equals(faction)){
                wayPointList.Add(s.GetPosition());
            }
        } */ 
       
        //Debug.Log("waypoint length is :" + wayPointList.Count);
    }

    //returns true if an enemy is near
    //assigns nearby enemy list
    public bool AreaLook() {
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, triggerRadius);
        //making new lists to jsut add to avoid having to remove (check efficiency vs removing and maintaining a list)
        nearbyEnemies = new List<Challenger>();
        bool enemynear = false;
        // Debug.Log( hitColliders.Count()); this is returning some numbers 
        foreach (Collider c in hitColliders) {
            //if collider is a challenger then check if its an enemy and add it to 
            Challenger chall = c.GetComponent<Challenger>();
            if (chall) {
                // Debug.Log("at least one not null!");
                if (!chall.faction.factionName.Equals(faction.factionName)) {
                    // 
                   // Debug.Log("found nearby enemy;");
                    nearbyEnemies.Add(c.GetComponent<Challenger>());
                    enemynear = true;
                }
                
            }
        }
        return enemynear;
    }

    public void UpdateClosestEnemy() {
        AreaLook();
        //only one enemy in 1v1 .. TODO SORT LIST WHEN THERES MORE THAN 1 PRIORITY TO DISTANCE / POWER
        if(nearbyEnemies.Count != 0)
        chaseTarget = nearbyEnemies[0].transform;
        else {
            chaseTarget = null; //worth??
        }
        
    }
    /*void Update() {
        if (!aiActive)
            return;
        if (currentState != null) {
            currentState.UpdateState(this);
        }
        else {
            Debug.Log("state destryoed somehow wajdasdasdcaskdasld wtf");

        }
    } */

    void SlowerUpdate() {
        if (!aiActive)
            return;
        if (currentState != null) {
            currentState.UpdateState(this);
        }
        else {
            Debug.Log("state destryoed somehow wajdasdasdcaskdasld wtf");

        }
    }
    void OnDrawGizmos() {
        if (currentState != null && eyes != null) {
            Gizmos.color = currentState.sceneGizmoColor;
           // Gizmos.DrawWireSphere(eyes.position, enemyStats.lookSphereCastRadius);
        }
    }

    public void TransitionToState(State nextState) {
        if (nextState != remainState) {
            currentState = nextState;
            OnExitState();
        }
    }

    public bool CheckIfCountDownElapsed(float duration) {
        stateTimeElapsed += Time.deltaTime;
        return (stateTimeElapsed >= duration);
    }

    private void OnExitState() {
        stateTimeElapsed = 0;
    }
}
