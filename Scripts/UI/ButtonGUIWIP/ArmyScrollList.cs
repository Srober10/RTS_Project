using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public enum UnitType {  MELEE, RANGED, CAVALRY}
public enum UnitRank { RECRUIT, MELEE1, MELEE2, MELEE3, RANGED1, RANGED2, RANGED3, CAVALRY1 }

[System.Serializable]
public class Unit {
   public Challenger leader;
   public string name;
   public Sprite icon;
   public int positionInList;
   public Unit(Sprite icon, int size) {
        this.icon = icon;
        name = "Recruit";
        power = 1;
        maxLife = 30;
        currentLife = maxLife;
        damage = 1;
        unitType = UnitType.MELEE;
        hireCost = 1;
        upkeep = 0;
        maxSize = 20;
        SetCurrentSize(size);
        level = 1;
        unitRank = UnitRank.RECRUIT;
   }
    public Unit(int currentSize, int maxSize, int power, int maxLife, int damage, UnitType type, UnitRank rank, string name, int pos) {
        this.maxSize = maxSize;
        this.currentSize = currentSize;
        this.power = power;
        this.maxLife = maxLife;
        this.damage = damage;
        this.unitType = type;
        this.unitRank = rank;
        this.name = name;
        this.positionInList = pos;
    }

    public int power;
    public int currentLife;
    public int maxLife;
    public int damage;
    public UnitType unitType;
    public int upkeep;
    public int maxSize;
    public int currentSize;
    public int level;
    public UnitRank unitRank;
    public int upgradeCost =2;
    public int hireCost = 1; // incase in the future adding in that can hire more than just recruits

    public void SetCurrentSize(int size) {
        if (currentSize <= maxSize)
            currentSize = size;
        else currentSize = maxSize;
    }

    //returns number of recruits upgradable for this unit
    public int RecruitsUpgradable() {
        if( unitRank == UnitRank.RECRUIT) {
            return maxSize - currentSize;
        }
        return 0;
    }
    //getting possible upgrades for each unit
    //returns null if there is no possible upgrade remaining
    public List<Action> PossibleUpgrades() {
        List<Action> upgradesAvailable = new List<Action>();
        switch (unitType) {
            case (UnitType.MELEE):
                if (unitRank.Equals(UnitRank.RECRUIT)) {
                    upgradesAvailable.Add(UpgradeToMelee1);
                    upgradesAvailable.Add(UpgradeToRanged1);
                }
                else if (unitRank.Equals(UnitRank.MELEE1)) {
                    upgradesAvailable.Add(UpgradeToMelee2);
                }
                else if (unitRank.Equals(UnitRank.MELEE2)) {
                    upgradesAvailable.Add(UpgradeToMelee3);
                    upgradesAvailable.Add(UpgradeToCavalry1);
                }
                else if (unitRank.Equals(UnitRank.MELEE3)) {
                    //end up the line
                }
                break;
            case (UnitType.RANGED):
                if (unitRank.Equals(UnitRank.RANGED1)) {
                    upgradesAvailable.Add(UpgradeToRanged2);
                }
                if (unitRank.Equals(UnitRank.RANGED2)) {
                    upgradesAvailable.Add(UpgradeToRanged3);
                }
                if (unitRank.Equals(UnitRank.RANGED3)) {
                   //end of the line
                }
                break;
            case (UnitType.CAVALRY):
                if (unitRank.Equals(UnitRank.CAVALRY1)) {
                    //end of the line
                }
                break;

        }
        return upgradesAvailable;
    }

    #region AI UPGRADES TODO 
    //upgrades unit (called by ai ) TODO

    public void UpgradeToMelee1() {
        // Unit(int currentSize, int maxSize, int power, int maxLife, int damage, UnitType type, UnitRank rank, string name, int pos)
       // new Unit(currentSize, 20, 2, 10, 2, UnitType.MELEE, UnitRank.MELEE1, "Melee troop rk 1", positionInList);
        power = 2;
        damage = 2;
        unitRank = UnitRank.MELEE1;
        name = "Melee troop rank 1";
        upgradeCost = 3;
        
    }
    public void UpgradeToMelee2() {
        new Unit(currentSize, 20, 4, 15, 3, UnitType.MELEE, UnitRank.MELEE2, "Melee troop rk 2", positionInList);
        power = 4;
        maxLife = 15;
        damage = 3;
        unitRank = UnitRank.MELEE2;
        name = "Melee troop rank 2";
        upgradeCost = 4;
    }
    public void UpgradeToMelee3() {
        new Unit(currentSize, 20, 6, 20, 4, UnitType.MELEE, UnitRank.MELEE3, "Melee troop rk 3", positionInList);
        power = 6;
        maxLife = 20;
        damage = 4;
        maxSize = 30;
        unitRank = UnitRank.MELEE3;
        name = "Melee troop rank 3";
        upgradeCost = 999;
    }
    public void UpgradeToRanged1() {
        new Unit(currentSize, 20, 2, 8, 3, UnitType.RANGED, UnitRank.RANGED1, "Ranged troop rk 1", positionInList);
        power = 2;
        maxLife = 8;
        damage = 3;
        unitRank = UnitRank.RANGED1;
        unitType = UnitType.RANGED;
        name = "ranged troop rank 1";
        upgradeCost = 3;
    }
    public void UpgradeToRanged2() {
        new Unit(currentSize, 20, 4, 10, 4, UnitType.RANGED, UnitRank.RANGED2, "Ranged troop rk 2", positionInList);
        power = 4;
        maxLife = 10;
        damage = 4;
        unitRank = UnitRank.RANGED2;
        name = "ranged troop rank 2";
        upgradeCost = 4;
    }
    public void UpgradeToRanged3() {
        new Unit(currentSize, 20, 7, 10, 6, UnitType.RANGED, UnitRank.RANGED3, "Ranged troop rk 3", positionInList);
        power = 7;
        maxLife = 12;
        damage = 6;
        unitRank = UnitRank.RANGED3;
        name = "ranged troop rank 3";
        upgradeCost = 999;
    }
    public void UpgradeToCavalry1() {
        new Unit(currentSize, 20, 7, 10, 6, UnitType.CAVALRY, UnitRank.CAVALRY1, "Cavalry troop rk 3", positionInList);
        power = 10;
        maxLife = 25;
        damage = 6;
        unitRank = UnitRank.CAVALRY1;
        unitType = UnitType.CAVALRY;
        name = "cavalry troop rank 1";
        upgradeCost = 999;
    }
      #endregion
}

//probably attach this to whichever object has an army 
public class ArmyScrollList : MonoBehaviour {

    //private Challenger player; 
    public List<Unit> unitList;
    public Transform contentPanel; //this is currently assigned in inspector : is always the player GUI for challeneger
    //  public Text myGoldDisplay;
    public SimpleObjPool buttonObjectPool;
    //this will need to be updated to the location on click as well.
    public ArmyScrollList otherList; // assign this in inspector , this is where you transfer units from to your army
    Challenger challReference;

    //currently only will work with closest town 
    // Use this for initialization
    void Start() {
       //add buttons
       AddButtons();

        // initialise other list, if this is attached to any object other than a 'challenger', we set
        //transfer list to be the players army.  //set unit list to unit list in attached challenger script
        challReference = this.GetComponent<Challenger>();
        if (challReference) {
            // do something specific ... (?) TODO think
            //set to current interactable object
           // otherList = challReference.location.GetArmyList();
            unitList = challReference.GetArmy();

        } else { //must be attached a town or interactable etc, so we set other list to players list for transfers
            otherList = GameObject.FindWithTag("Player").GetComponent<ArmyScrollList>();
        }

        
    }

    //call this method from other classes to refresh GUI
    public void RefreshDisplay() {
        // myGoldDisplay.text = "Gold: " + gold.ToString();
        //sets asl to player army again
        unitList = challReference.GetArmy();
        RemoveButtons();
        AddButtons();
       
    }

    public void RefreshList() {
        unitList = challReference.GetArmy();
    }
    
    //for towns only as the list needs to be homogeneous
    public int GetUnitCost() {
       return unitList[0].hireCost;
    }
    //returns number of units in towns list
    public int GetNumberRecruits() {
        int count = 0;      
        foreach (Unit u in unitList) {
            count += u.RecruitsUpgradable();
        }
        return count;
    }
    //unit is a parameter, populates unitList with units 
    public void SpawnUnit(Unit unit) {
            unitList.Add(unit);            
    }

    private void RemoveButtons() {
        for (int i = (contentPanel.childCount - 1); i >= 0; i--) {
            GameObject go = contentPanel.GetChild(i).gameObject;
            //Debug.Log(contentPanel.GetChild(0) + "is 0th element");
             if (go != null) {
                 GameObject toRemove = contentPanel.GetChild(i).gameObject;
                 buttonObjectPool.ReturnObject(toRemove);
             }
         }
        //Debug.Log("buttons remaining : " + contentPanel.childCount);
        //int numOfChildren = contentPanel.childCount -1;
        /* Debug.Log("this is the increment max: " + numOfChildren);
          for (int i = numOfChildren; i >= 0; i--) {
              GameObject toRemove = transform.GetChild(i).gameObject;
              buttonObjectPool.ReturnObject(toRemove);
          }*/
        // Debug.Log("buttons remaining: " + contentPanel.childCount);
        //attempt number 42:
        /* Debug.Log(contentPanel.childCount);
         int i = 0;
         //Array to hold all child obj
         GameObject[] allChildren = new GameObject[contentPanel.childCount];

         //Find all child obj and store to that array
         foreach (Transform child in transform) {
             allChildren[i] = child.gameObject;
             i += 1;
         }

         //Now destroy them
         foreach (GameObject child in allChildren) {
             GameObject toRemove = transform.GetChild(i).gameObject;
             buttonObjectPool.ReturnObject(toRemove);
         }

         Debug.Log(transform.childCount); */
    }
    
    private void AddButtons() {
        //  Debug.Log("these are army list length: " + unitList.Count);
        //Debug.Log("Unit list len: " + unitList.Count);
        for (int i = 0; i < unitList.Count; i++) {
            Unit unit = unitList[i];
            GameObject newButton = buttonObjectPool.GetObject();
            newButton.transform.SetParent(contentPanel);

            UnitButton sampleButton = newButton.GetComponent<UnitButton>();
            sampleButton.Setup(unit, i, this);
       }
    }
 

    //adds unit to the current troopList and removes it from the other eg
    // armyList.GetUnit(unit) adds unit to the armyList called from and removes from the class called in.

    public void GetUnit(Unit unit) {
        //can add conditional checks here
       // Debug.Log("We have " + unitList.Count + "Numberof troops (scroll list)");
        //add unit from city etc to player
        // gold transfer
        //adds unit to this armyList
        AddUnit(unit, this);
        //removes unit from cities army list. TODO NOT REMOVING?
        RemoveUnit(unit, otherList);
        //refresh displays of both places (atm jsut do ours as not sure want to display cities like this/)
        RefreshDisplay();

    }

    private void AddUnit(Unit unitToAdd, ArmyScrollList list) {
        list.unitList.Add(unitToAdd);
    }

    private void RemoveUnit(Unit unitToRemove, ArmyScrollList list) {
        for (int i = list.unitList.Count - 1; i >= 0; i--) {
            if (list.unitList[i] == unitToRemove) {
                list.unitList.RemoveAt(i);
            }
        }
    }
}