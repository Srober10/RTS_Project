using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//building on a map
public class Village : MonoBehaviour, IInteractable, IStructure {
    //this is the menu to display on GUI
    private MenuManager menuManager;

    // public Mesh building;
    // Use this for initialization

    //implementing istructures methods
    BuildingType buildingType = BuildingType.Village;
    public string buildingName;
    public Faction owningFaction;
    public Color colourText;
    public bool isRaided = false;
    public float raidedCD = 60f;
    private bool collected = false;
    private float collectedCD = 20f;
    public int resourceCount;
    public int collectableResources; // at the moment setting this to 1 as minimum collectable res
    public int maxResources;
    private float generateTimeGap = 1f;
    private float timePassedRaided = 0;
    private float timePassedCollected = 0;
    public Image resourceTimer;
    private Challenger player;
    private bool inRange = true;
    public SphereCollider radiusToInteract;
    public Vector3 villagePosition;
    public CityText village3Dtext;

    //olf start method
    void AfterSetup () {
        //menuUI = GameObject.Find("City Menu").GetComponent<Canvas>();
        player = GameObject.FindWithTag("Player").GetComponent<Challenger>();
        menuManager = GameObject.Find("Menu Manager").GetComponent<MenuManager>(); // trying to mae this work
        
        collectableResources = CalculateMaxResourcesCollectable();
        maxResources = collectableResources;
        //method/startsat/repeatsevery
        InvokeRepeating("GenerateResources", 1f, generateTimeGap);
    }
    //todo change ratio to correlate with fame etc
    private int CalculateMaxResourcesCollectable() {
        return 20;
    }
    Vector3 IInteractable.position {
        get {
            return villagePosition;
        }

        set {
            villagePosition = value;
        }
    }
    public Challenger GetInteracting() {
        return player;
    }
    public Transform GetPosition() {
        return this.transform;
    }

    public void Setup(Faction faction, Vector3 pos) {
        owningFaction = faction;
        colourText = owningFaction.colour;
        villagePosition = pos;
        AfterSetup();
    }
    public Faction GetFaction() {
        return owningFaction;
    }
    public Color GetTextColour() {
        return colourText;
    }
    public string GetName() {
        return "" + buildingName;
    }
    public bool IsInRange() {
        return inRange;
    }
    public void SetInRange(bool b) {
        inRange = b;
    }
    public string GetBuildingName() {
        return buildingName;
    }
    public IInteractable GetLocation() {
        return this;
    }
    //implementing interactable methods
    public void OnClick() {
       // Debug.Log("On click being called!");
        //eg show animation
       // Destroy(gameObject);

    }
    public void GUIOnClick() {
        //display city menu GUI from inspector
      menuManager.ShowMenu();
     
    }

    //method should be called repeatedly to get
   
    private void GenerateResources() {
        int resGenerated = 1;  //todo expand for local economy of nearby allied cities to extend this
        if (!isRaided && resourceCount < maxResources) {
            resourceCount += resGenerated;
        }
        if(timePassedRaided >= raidedCD) {
            isRaided = false;
            timePassedRaided = 0;
        }
        if (timePassedCollected >= collectedCD) {
            collected = false;
            timePassedCollected = 0;
        }
        if(collected) {
            timePassedCollected += generateTimeGap;
        }

        if (isRaided) {
            timePassedRaided += generateTimeGap;
        }
    }

    //returns clamped value between 0 and 1 for fill
    public float GetPercentageCD() {
        float c = Mathf.Clamp(timePassedCollected / collectedCD, 0, 1);
        float r = Mathf.Clamp(timePassedRaided / raidedCD, 0, 1);
        //Debug.Log("collected amount : " + c + " |||  raided amount : " + r);
        return Mathf.Max(c,r);
        
    }


    //returns bool if won or lost and removes casualties
    public bool Attack(Challenger attacker) {
        bool won = attacker.StrongerThanOpponent(GetArmyList());
        if (won) {
            owningFaction.villages.Remove(this);
            attacker.faction.villages.Add(this);
            isRaided = true;
            owningFaction = attacker.faction;
            village3Dtext.Recolour(owningFaction.colour);
        }
        attacker.RemoveTroopCasualties(GetArmyList());
        return won;
    }

    // on collection goes into a cooldown period
    //on cd period increment timePassed so CD is shown
    public void CollectResources(Challenger chal) {
        //takes resources from this village        
        if (resourceCount >= 1 && !collected) {
            //Debug.Log("Collect all resources called, " + resourceCount + "removed");
            chal.gold += resourceCount;
            resourceCount = 0;
            collected = true;
        }
    }
    //todo implement
    public List<Unit> GetArmyList() {
        return null;
    }


    public int NetIncome() {
        return -1;
    }
    public void SetBuildingType(BuildingType type) {
        buildingType = type;
    }
    public BuildingType GetBuildingType() {
        return buildingType;
    }

    public void AIGather(Challenger chal) {
        CollectResources(chal);
    }

    //button methods called from menu gui
    public void Interaction1() {
        //Debug.Log("This button has been clicked");
       // Debug.Log("This is a " + buildingType);
        if (inRange) {
            CollectResources(player);
        }
        else {
            Debug.Log("move closer to interact with this object");
        }
    }

    public void Interaction2() {
        Debug.Log("This button has been clicked");
        Debug.Log("This is a " + buildingType);
    }






}
