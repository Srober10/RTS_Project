using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TroopUpgradesGUI : MonoBehaviour {

    
    private Unit oldUnitRef;
    private Unit upgradedUnit;
    public int indexOfUnit; //set from method in menumanager from button click on unitbutton 

    //set in inspector
    public MenuManager manager;

    private Challenger player;
    private ArmyScrollList playersList;
    private List<Unit> playerArmy;
    private int upgradeCost;
    //unit comparison and update

    public Color highlightedColour;
    public Color disabledColour;
    public Color upgradableColor;
    public Color disabledUpgradeColor; // adjacent upgrades but not enough resources

    private Button currentButton;
    private Button[] purchasableButtons;
    private Button[] disabledButtons; // everything disabled as default 
    // buttons by type
    public Button recruitButton;
    public Button[] meleeButtons;
    public Button[] rangedButtons;
    public Button[] cavalryButtons;

	// Use this for initialization
	void Start () {
        player = manager.player;
        playersList = player.armyScrollList;
        playerArmy = player.GetArmy();
	}
	
    public void RefreshGUI(Unit unit) {
        DisableAllButtons();
        SetButtons(unit);
        ActivateButtons();
    }

    //sets all buttons to not interactable and disabled colours back to default
    private void DisableAllButtons() {
        recruitButton.interactable = false;
        ColorBlock cl = recruitButton.colors;
        cl.disabledColor = disabledColour;
        recruitButton.colors = cl;
        foreach (Button b in meleeButtons) {
            ColorBlock cb = b.colors;
            cb.disabledColor = disabledColour;
            b.colors = cb;
            b.interactable = false;
        }
        foreach (Button b in rangedButtons) {
            ColorBlock cb = b.colors;
            cb.disabledColor = disabledColour;
            b.colors = cb;
            b.interactable = false;
        }
        foreach (Button b in cavalryButtons) {
            ColorBlock cb = b.colors;
            cb.disabledColor = disabledColour;
            b.colors = cb;
            b.interactable = false;
        }
    }
    //sets colour of all buttons and sets purchasble active
    // todo check if the player has required resources ??? currently this is in button function, onclick it will print debug if not enough
    private void ActivateButtons() {

        // turns disabled colour of button which this unit is a different colour for clarity
        ColorBlock c = currentButton.colors;
        c.disabledColor = highlightedColour;
        currentButton.colors = c;
        
        foreach(Button b in purchasableButtons) {
            ColorBlock cb = b.colors;
            cb.normalColor = upgradableColor;
            b.colors = cb;
            //if(player.gold > )
            b.interactable = true;
           
        }
    }
    
    //call this from inside button() method
    //sets current button, and purchasable buttons
    //also can set the text etc 
    public void SetButtons(Unit unit) {
        oldUnitRef = unit;
       
        purchasableButtons = new Button[2];
        disabledButtons = new Button[8]; // dont need/
        UnitType t = unit.unitType;
        UnitRank r = unit.unitRank;
        Debug.Log(" unit type " + t.ToString() + ", unit rank: " + r.ToString());
        switch (t) {
            case (UnitType.MELEE):
                if (r.Equals(UnitRank.RECRUIT)) {
                    currentButton = recruitButton;
                    purchasableButtons[0] = (meleeButtons[0]);
                    purchasableButtons[1] = (rangedButtons[0]);

                }
                else if (r.Equals(UnitRank.MELEE1)) {
                    currentButton = meleeButtons[0];
                    purchasableButtons = new Button[1];
                    purchasableButtons[0] = (meleeButtons[1]);
                }
                else if (r.Equals(UnitRank.MELEE2)) {

                    currentButton = meleeButtons[1];
                    purchasableButtons[0] = (cavalryButtons[0]);
                    purchasableButtons[1] = (meleeButtons[2]);
                }
                else if (r.Equals(UnitRank.MELEE3)) {
                    currentButton = meleeButtons[2];
                    purchasableButtons = new Button[0];
                }
                    break;
            case (UnitType.RANGED):
                if (r.Equals(UnitRank.RANGED1)) {
                    currentButton = rangedButtons[0];
                    purchasableButtons = new Button[1];
                    purchasableButtons[0] = (rangedButtons[1]);
                }
                if (r.Equals(UnitRank.RANGED2)) {
                    currentButton = rangedButtons[1];
                    purchasableButtons = new Button[1];
                    purchasableButtons[0] = (rangedButtons[2]);
                }
                if (r.Equals(UnitRank.RANGED3)) {
                    currentButton = rangedButtons[2];
                    purchasableButtons = new Button[0];
                    //purchasableButtons[0] = (rangedButtons[3]);
                }
                break;
            case (UnitType.CAVALRY):
                if (r.Equals(UnitRank.CAVALRY1)) {
                    currentButton = cavalryButtons[0];
                    purchasableButtons = new Button[0];
                }
                break;
            
        }

    }

    private void ReplaceUnit(Unit oldUnit, Unit newUnit) {
        int index = playerArmy.IndexOf(oldUnit); //this line is giving -1 which means that its saying the players army doesnt contain the troop.. somewhere reference is broken this should work.
        Debug.Log("rank of unit upgrade : " + newUnit.unitRank.ToString() + " index is :" + index);
        if (index != -1) {
            playerArmy[index] = newUnit;

        } else {
            Debug.Log("Replacing unit failed");
        }
    }

    private void ReplaceUnit2(Unit oldUnit, Unit newUnit) {
        Debug.Log(newUnit.positionInList);
        if(newUnit.positionInList > -1)
        playerArmy[newUnit.positionInList] = newUnit;
    }


    //upgrade buttons : set troops stats / type / rank
    // selected by player from the GU, player army list and scroll list therefore guarenteed to be used
    // cant have any parameters as called from inspector button
    //sets unit to the respective upgrade. replaces old unit in list and armyscroll list with this 
    // !! oldUnitRef is broken ..
    public void UGtoMelee1() {
        //int upgradeCostPerUnit = 2;
        var unit = playerArmy[indexOfUnit];
        int ugCost = unit.currentSize * unit.upgradeCost;
        Debug.Log("upgrade2 cost: " + ugCost + " gold: " + player.gold);
        if (player.gold > ugCost) {
            Debug.Log("this shouldnt be working?");
            player.gold -= ugCost;
            unit.UpgradeToMelee1();
            
            //manager.ShowTroopUpgrades(oldUnitRef, oldUnitRef.positionInList);
            RefreshGUI(unit);
         }
            else {
                Debug.Log("You cant afford to upgrade this!");
            }
        }

    //old method before implemented AI methods
    public void UpgradeToMelee1() {
        
        //change this to per unit cost * current size of unit army .
        int upgradeCostPerUnit = 2;
        int upgradeCost = upgradeCostPerUnit * oldUnitRef.currentSize;
        //replacing unit in army 
        //todo replace ALL units with upgraded version: 
        if (player.gold > upgradeCost) {
            //  public Unit(int currentSize, int maxSize, int power, int maxLife, int damage, UnitType type, UnitRank rank, string name) {
           
            upgradedUnit = new Unit(oldUnitRef.currentSize, 20, 2, 10, 2, UnitType.MELEE, UnitRank.MELEE1, "Melee troop rk 1", indexOfUnit);            
        
            ReplaceUnit2(oldUnitRef, upgradedUnit);
            //replacing unit in ASL not needed hopefully (test)

            // remove resources
            player.gold -= upgradeCost;
            // refresh GUI
            RefreshGUI(upgradedUnit);
        }
        else {
            Debug.Log("you cant afford this.");
        }
    }
    public void UpgradeToMelee2() {
        var unit = playerArmy[indexOfUnit];
        int ugCost = unit.currentSize * unit.upgradeCost;
        Debug.Log("upgrade2 cost: " + ugCost + " gold: " + player.gold);
        if (player.gold > ugCost) {
            
            player.gold -= ugCost;
            unit.UpgradeToMelee2();
            //manager.ShowTroopUpgrades(oldUnitRef, oldUnitRef.positionInList);
            RefreshGUI(unit);
        }
        else {
            Debug.Log("You cant afford to upgrade this!");
        }
    }
    public void UpgradeToMelee3() {
        var unit = playerArmy[indexOfUnit];
        int ugCost = unit.currentSize * unit.upgradeCost;
        Debug.Log("upgrade2 cost: " + ugCost + " gold: " + player.gold);
        if (player.gold > ugCost) {
            unit.UpgradeToMelee3();
            player.gold -= ugCost;
            //manager.ShowTroopUpgrades(oldUnitRef, oldUnitRef.positionInList);
            RefreshGUI(unit);
        }
        else {
            Debug.Log("You cant afford to upgrade this!");
        }
    }
    public void UpgradeToRanged1() {
        var unit = playerArmy[indexOfUnit];
        int ugCost = unit.currentSize * unit.upgradeCost;
        //Debug.Log("upgrade2 cost: " + ugCost + " gold: " + player.gold);
        if (player.gold > ugCost) {
            unit.UpgradeToRanged1();
            player.gold -= ugCost;
            //manager.ShowTroopUpgrades(oldUnitRef, oldUnitRef.positionInList);
            RefreshGUI(unit);
        }
        else {
            Debug.Log("You cant afford to upgrade this!");
        }
    }
    public void UpgradeToRanged2() {
        var unit = playerArmy[indexOfUnit];
        int ugCost = unit.currentSize * unit.upgradeCost;
        //Debug.Log("upgrade2 cost: " + ugCost + " gold: " + player.gold);
        if (player.gold > ugCost) {
            unit.UpgradeToRanged2();
            player.gold -= ugCost;
            //manager.ShowTroopUpgrades(oldUnitRef, oldUnitRef.positionInList);
            RefreshGUI(unit);
        }
        else {
            Debug.Log("You cant afford to upgrade this!");
        }
    }
    public void UpgradeToRanged3() {
        var unit = playerArmy[indexOfUnit];
        int ugCost = unit.currentSize * unit.upgradeCost;
        //Debug.Log("upgrade2 cost: " + ugCost + " gold: " + player.gold);
        if (player.gold > ugCost) {
            unit.UpgradeToRanged3();
            player.gold -= ugCost;
            //manager.ShowTroopUpgrades(oldUnitRef, oldUnitRef.positionInList);
            RefreshGUI(unit);
        }
        else {
            Debug.Log("You cant afford to upgrade this!");
        }
    }
    public void UpgradeToCavalry1() {
        var unit = playerArmy[indexOfUnit];
        int ugCost = unit.currentSize * unit.upgradeCost;
        //Debug.Log("upgrade2 cost: " + ugCost + " gold: " + player.gold);
        if (player.gold > ugCost) {
            unit.UpgradeToCavalry1();
            player.gold -= ugCost;
            //manager.ShowTroopUpgrades(oldUnitRef, oldUnitRef.positionInList);
            RefreshGUI(unit);
        }
        else {
            Debug.Log("You cant afford to upgrade this!");
        }
    }

}
