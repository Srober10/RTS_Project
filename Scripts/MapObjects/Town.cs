using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//building on a map
public class Town : MonoBehaviour, IInteractable, IStructure {
    //this is the menu to display on GUI
    private MenuManager menuManager;
    public MapBuildingText buttonText;
    private GameObject playerGO;
    private Challenger player; // needs ref to player to add troops to
    public List<Unit> playerArmy; //this needs to also set for ai army as well
    // public TUnit citiesUnit; 
    public Unit citiesUnit;
    public Unit citiesTUnit;
    public Sprite unitsSprite; //set in inspector
    public GameObject content; //where the army scroll list script is
    private int unitsHere = 4;
    //ArmyGUIController armyGUI = new ArmyGUIController(); //basic GUI controller ?? can remove?
    //** these are from trying to add a GUI
    public List<Unit> townsArmylist; // update this periodically
    ArmyScrollList playersArmyList; //what we add to
    private int income;
    public string buildingName;
    public Faction owningFaction;
    public Color colourText;
    public SphereCollider radiusToInteract;
    private bool inRange = true;
    public Vector3 townPos;
    public CityText town3Dtext;
    public int recruitsAvailable;

    // public Mesh building;
    //public Unit townRecruit; todo move from obj spawner to individual towns?

    //implementing istructures methods
    public bool isCapital = false;
    public bool isRaided = false; //cooldown on raided building for troop regeneration
    public int raidedCD = 40; //40s
    public float recruitRegainCD = 10f; //find a good cd for this
    BuildingType buildingType = BuildingType.Town;
    bool setup = false;

    //fancy get and set method
    Vector3 IInteractable.position {
        get {
            return townPos;
        }

        set {
            townPos = value;
        }
    }

    public Challenger GetInteracting() {
        return player;
    }
    public bool IsInRange() {
        return inRange;
    }
    public Color GetTextColour() {
        return colourText;
    }
    public string GetBuildingName() {
        return buildingName;
    }

    public Transform GetPosition() {
        return this.transform;
    }
    //old start
    public Faction GetFaction(){
        return owningFaction;
    }
    
    public void SetInRange(bool b) {
        inRange = b;
    }
    void AfterSetup() {
        //menuUI = GameObject.Find("City Menu").GetComponent<Canvas>();
        menuManager = GameObject.Find("Menu Manager").GetComponent<MenuManager>(); // trying to mae this work
        buttonText = menuManager.GetComponent<MapBuildingText>(); // :/
        playerGO = GameObject.FindWithTag("Player");
        playerArmy = playerGO.GetComponent<Challenger>().GetArmy();
        //GUI:
        townsArmylist = new List<Unit>(); 
        playersArmyList = playerGO.GetComponent<ArmyScrollList>(); 
        player = playerGO.GetComponent<Challenger>();
        if (isCapital) {
            buildingType = BuildingType.Capital;
        }
       // citiesUnit = new Unit(unitsSprite);
        setup = true;
        InvokeRepeating("RegainRecruits", 40f, recruitRegainCD);
        //buttonText.TextSet(); too soon?

    }

    public void Setup(bool isCapital, int income, int unitCount, Faction ownedBy, Vector3 pos) {
        this.isCapital = isCapital;
        this.income = income;
        this.unitsHere = unitCount;
        owningFaction = ownedBy;
        colourText = ownedBy.colour;
        townPos = pos;
        AfterSetup();
    }
    public string GetName() {
        return "" + buildingName;
    }

    public IInteractable GetLocation() {
        return this;
    }

    //implementing interactable methods
    public void OnClick() {
        // Debug.Log("On click being called!");
        //eg show animation
        //Disable player movement ? slow time/ stop time?

    }
    public void GUIOnClick() {
        //display city menu GUI from inspector
       /* Debug.Log("test -M: " + menuManager + "setup is : " + setup);
        if (!setup) {
            AfterSetup();
            Debug.Log("ran aftersetup now: " + setup);
        } */
        menuManager.ShowMenu();
        if (isCapital) SetBuildingType(BuildingType.Capital); //late setter
        //buttonText.TextSet(); ?? needed?
    }
    public List<Unit> GetArmyList() {
        return townsArmylist;
    }


    public int NetIncome() {
        return income;
    }
    public void SetBuildingType(BuildingType type) {
        buildingType = type;
    }
    public BuildingType GetBuildingType() {
        return buildingType;
    }
    public void AddTroops(Unit u, int a) {
        u.SetCurrentSize(a);
        u.positionInList = townsArmylist.Count;
        townsArmylist.Add(u);
    }

    //invoked repeatingly, will add 5 to existing stack of recruits if exists or will create new unit with army 1 if empty
    //we dont want more than 1 stack of recruits in a city (2 for capital), so dont regen if so
    //merge uneven troops in capital into one stack if possible: TODO
    public void RegainRecruits() {
        buttonText.TextSet();
        if (!isRaided) {
            if (townsArmylist.Count == 0 || townsArmylist.Count < 2 && isCapital) {
                townsArmylist.Add(new Unit(null, 5));
                buttonText.TextSet();
            }
            else if (townsArmylist.Count > 1 && !isCapital) {
                return;
            }
            else if (townsArmylist.Count > 0 && townsArmylist.Count < 3) {
                int recruitsToAdd = 5;
                foreach(Unit u in townsArmylist ) {
                    if(u.currentSize < u.maxSize) {
                        int difToMax = u.maxSize - u.currentSize;
                        if( difToMax >= recruitsToAdd) { //if there is more space than we are adding, just add all and return
                            u.SetCurrentSize(u.currentSize + recruitsToAdd);
                            return;
                        } else { //this unit is at max after adding less than total recruits so add to max and contiune
                            recruitsToAdd -= difToMax;
                            u.SetCurrentSize(u.currentSize + recruitsToAdd);
                        }
                    }
                }
                buttonText.TextSet();
            }
        }
    }

    public void Interaction1() {
        Debug.Log("attempting to takeover town");
        Attack(player);

    }


    private void RecruitHire( Challenger chall) {
        if (chall.HasRefillableRecruits()) {
            Debug.Log("Has refillable troops : current size for first unit in list : " + chall.GetArmy()[0].currentSize + " / max size: " + chall.GetArmy()[0].maxSize);
            RefillPlayerRecruits(chall);
            buttonText.UpdateRecruitCount();
        }
        else {
            Debug.Log("HIRE NEW RECRUITS called. Trying to add new unit stack to list");
            HireNewRecruits(chall);
            buttonText.UpdateRecruitCount();
        }
    }

    public void AIHire(Challenger chall) {
        RecruitHire(chall);
    }
    //if player has unfilled army option to refill army here: refills only recruits level 1 
    public void Interaction2() {
        RecruitHire(player);
        //buttonText.UpdateRecruitCount();
    }

    // players armies are not fully refilled and so dont need to add buttons to the army GUI, just increment current size of recruits in player army
    /*this doesnt terminate ever when trying to add new unit to AI army.. occasionally infinite looped, replaced with below
    private void RefillPlayerRecruits1(Challenger chall) {

       Debug.Log("Attempting to a unit from...: " + buildingType);
        //fancy gui: troop count is the list, dont need an int check. 
        //refilling players recruits
        playerArmy = chall.GetArmy();
        playersArmyList = chall.armyScrollList;
        int failsafe = 0;
        if (townsArmylist.unitList.Count > 0 && chall.gold >= townsArmylist.unitList[0].hirePrice && playerArmy.Count > 0) {
                foreach (Unit playerU in playerArmy) {
                    foreach (Unit u in townsArmylist.unitList) {
                                                                                    // changed from playerU, fixed??,
                        while (playerU.currentSize < playerU.maxSize && chall.gold >= u.hirePrice && u.currentSize > 0) {
                            failsafe++;
                            u.currentSize -= 1;
                            playerU.currentSize += 1;
                            chall.gold -= u.hirePrice;
                            if (failsafe > 10000 || playerU.currentSize >= playerU.maxSize)
                                break;
                       // Debug.Log("looping still");
                        }
                    }

                }
             }      
    } */

    private void RefillPlayerRecruits(Challenger chall) {

       // Debug.Log("Refill player rec called");
        //fancy gui: troop count is the list, dont need an int check. 
        //refilling players recruits
        playerArmy = chall.GetArmy();
        playersArmyList = chall.armyScrollList; //can uncomment out thses later
        var townsArmy = townsArmylist;
        foreach (Unit p in playerArmy) {
            int recruits = (p.maxSize - p.currentSize); //amount needed to hire             
            Debug.Log(recruits + " recruits needed for full refill");
            foreach (Unit t in townsArmy) { // line below should assure all units are recruits
                if (recruits <= 0) break; //skip to next unit
                int townRecruits = t.currentSize; //amount hireable
                Debug.Log(townRecruits + " recruits in town");
                if (townRecruits > 0) {
                    //this unit for the player is now full, so add all recruits
                    int cost = recruits * p.hireCost;
                    //case 1, player unit will be filled to max and can afford 
                    if(recruits <= t.currentSize  && chall.gold >= cost) {
                        Debug.Log("case 1");
                        chall.gold -= cost;
                        recruits = 0;
                        p.currentSize = p.maxSize;
                        if (t.currentSize - recruits >= 0) {
                            t.currentSize -= recruits;
                           // playersArmyList.RefreshDisplay();
                           
                        }
                        else { //should never hit this
                            t.currentSize = 0;
                            //playersArmyList.RefreshDisplay();
                        }
                        break; // go to next player unit
                    }
                    //case 2, town unit emptied before player unit to max and can afford
                    else if (recruits > t.currentSize && chall.gold >= cost) {
                        Debug.Log("case 2");
                        chall.gold -= (t.currentSize * t.hireCost);
                        p.currentSize += t.currentSize;
                        recruits -= t.currentSize;
                        t.currentSize = 0;
                       // playersArmyList.RefreshDisplay();
                    }
                    //case 3, town unit emptied before player unit to max but cant afford
                   else if (t.currentSize - recruits < 0 && chall.gold < cost) {
                        Debug.Log("case 3");
                        int maxHireable = Mathf.RoundToInt((float)chall.gold / (float)t.hireCost);
                        Debug.Log(maxHireable + " max hirable,,");
                        chall.gold -= (maxHireable * t.hireCost);
                        p.currentSize += maxHireable;
                        t.currentSize -= maxHireable;
                       // playersArmyList.RefreshDisplay();
                        return; //player is out of gold so break out
                    }
                    //case 4, player unit would be filled to max but cant afford, acting as default fallthrough case as same as case 3
                   else if (recruits - t.currentSize <= 0 && chall.gold < cost) {
                        Debug.Log("case 4");
                        int maxHireable = Mathf.RoundToInt((float)chall.gold / (float)t.hireCost);
                        chall.gold -= (maxHireable * t.hireCost);
                        p.currentSize += maxHireable;
                        t.currentSize -= maxHireable;
                        //playersArmyList.RefreshDisplay();
                        return;//player is out of gold so break out
                    }

                }
                else {
                    Debug.Log("town is empty??");
                    return;
                }
            }

        }

        
    }

    //adds a new stack to the army list and removes it from the town as player has no recruits refillable   
    private void HireNewRecruits(Challenger chall) {
        Debug.Log("hire recruits in town method called");
        playerArmy = chall.GetArmy();
        playersArmyList = chall.armyScrollList;
        List<int> iToRemove = new List<int>(); //complains about removing from list while iterating through
        int counter = 0;
        foreach (Unit u in townsArmylist) {
            //if player can afford the whole stack, add it
            int cost = u.hireCost * u.currentSize;
            if (chall.gold >= cost) {//if can afford whole stack
                int oldPos = u.positionInList;
                u.positionInList = playerArmy.Count;
                // playersArmyList.GetUnit(u); //adds unit to players army from town not working for enemy AI whyyy??
                playerArmy.Add(u);
                iToRemove.Add(counter);
                chall.gold -= cost;
                counter++;
                //playersArmyList.RefreshDisplay();
            }
            else { //dont need to delete stack, only need to add stack to chall
                Debug.Log("You cant afford the whole recruit stack, attempting to add new partial stack ");
                counter++;
                int amount = Mathf.RoundToInt ( (float) player.gold / (float) u.hireCost);
                int newAmount = u.currentSize - amount;
                if (newAmount >= 0 && newAmount <= u.maxSize) {
                    u.currentSize = amount;
                    u.positionInList = playerArmy.Count;
                    playerArmy.Add(u);
                    u.currentSize = newAmount;
                   // playersArmyList.RefreshDisplay();

                } else {
                    Debug.Log("adding new stack failed, shouldnt hit this ??");
                }
                break;
            }
        }
        try {
            if (townsArmylist.Count > 0) {
                foreach (int i in iToRemove) {
                    townsArmylist.RemoveAt(i);
                }
            }
        } catch {
            Debug.Log("catchededed");
        }
        //update player army GUI buttons
        playersArmyList.RefreshDisplay();
        //refresh town text
        //playersArmyList.RefreshDisplay();
    }

    //returns bool if won or lost and removes casualties
    public bool Attack(Challenger attacker) {
        bool won = attacker.StrongerThanOpponent(townsArmylist);
        if (won) {
            if (isCapital) {
                Debug.Log("Capital taken, game over!");
                //todo add lose screen ui
            }
            owningFaction.towns.Remove(this);
            attacker.faction.towns.Add(this);
            isRaided = true;
            owningFaction = attacker.faction;
            town3Dtext.Recolour(owningFaction.colour);
        }
        attacker.RemoveTroopCasualties(GetArmyList());
        return won;
    }
    

    //take over town from menu 
    public void Interaction3() {
        Attack(player);
        //close this menu
        //display victory menu
        Debug.Log("Town Taken");
    }

    /*
    public void Interaction99() {

        if (player.HasRefillableRecruits()) {
            Debug.Log("Attempting to a unit from...: " + buildingType);
            //fancy gui: troop count is the list, dont need an int check. 
            //adds unit from town to player i hHoPpEe 
            if (townsArmylist.unitList.Count > 0 && player.gold >= townsArmylist.unitList[0].hirePrice) {
                int recruitsToAdd = player.RefillableRecruits();
                List<int> recruits = player.RecruitRefillList(playerArmy);
                int playerGold = player.gold; // keep track of gold player has and break if player runs out
                if (playerGold < recruitsToAdd) {
                    Debug.Log("You dont have enough gold to fully refill troops :( ");
                }
                int counter = Mathf.Min(recruitsToAdd, playerGold);
                foreach (Unit u in townsArmylist.unitList) {
                    if (u.currentSize >=  recruits[0]) {
                        int difToMax = u.maxSize - u.currentSize;
                        if (difToMax >= counter) { //if there is more space than we are adding, just add all and return
                            u.SetCurrentSize(u.currentSize + counter);
                            return;
                        }
                        else { //this unit is at max after adding less than total recruits so add to max and contiune
                            counter -= difToMax;
                            u.SetCurrentSize(u.currentSize + counter);
                        }
                    }
                }
                playersArmyList.GetUnit(townsArmylist.unitList[0]); //pops first unit from the list (all meant to be same)
                player.gold -= townsArmylist.unitList[0].hirePrice;
                // Debug.Log("Here we are !!!!!!!!");
            }
            else {
                Debug.Log("No Troops will join you.");
            }


        }
        buttonText.TextSet();
    } */

    //public void Interaction2() {
    //  Debug.Log("is this a problem ssssssssss");
    //}

    public void ExitButton() {
     // menuManager.HideMenu();
    }



}
