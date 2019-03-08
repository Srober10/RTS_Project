using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    //used for keyboard pressed events and showing related menus, eg ESC and I 
    public static bool menuShown = false;

    public bool playerFrozenInMenu = false;
    public GameObject playerGUI; //army gui
    public GameObject buildingMenu;
    public GameObject challengerDialogue;
    public ChallengerText chalText;
    public GameObject battleRecap;
    public GameObject battleGUI;
    private ArmyGUIController armyGUI;
    private BattleGUI battleGUIObject;
    public GameObject upgradesGUI;
    public TroopUpgradesGUI troopUgGUI;
    public Challenger player;
    public BattleResultsUI resultsUI;
    public GameObject battleResultsObj;
    // Use this for initialization

    private void Awake() {
        armyGUI = GetComponent<ArmyGUIController>();
        playerGUI.SetActive(false);
        chalText = GetComponent<ChallengerText>();
        battleGUIObject = GetComponent<BattleGUI>();
    }

    //on pressing the i key, shows player info and army stats
    private void Update() {
        if (Input.GetKeyDown("i") && !menuShown) {
            playerGUI.SetActive(true);
           // armyGUI.GenerateIcons();
            menuShown = true;
        } else if (Input.GetKeyDown("i") && menuShown) {
            playerGUI.SetActive(false);
            menuShown = false;
        }
    }

    public void BattleClick() {
        battleGUI.SetActive(false);
        //calculate battle
        //
        battleGUIObject.Battle();
       // battleRecap.SetActive(true);
    }
   
    public void SetPlayerFrozen (bool freeze) {
        playerFrozenInMenu = freeze;
    }

    public void ClosePlayerGUI() {
        playerGUI.SetActive(false);
        //menuShown = false;
    }

    public void HideMenu() {
        buildingMenu.SetActive(false);
        menuShown = false;
        playerFrozenInMenu = false;
    }

    public void HideBattleRecap() {
        battleRecap.SetActive(false);
        playerFrozenInMenu = false;

    }

    public void ShowBattleResults() {
        resultsUI.SetResults();
        BattleClick();
        battleResultsObj.SetActive(true);
    }
    public void HideBattleResults() {
        battleResultsObj.SetActive(false);
        HideChallengerUI();
        HideMenu();
    }
    //sets unit index
    public void ShowTroopUpgrades(Unit unit, int unitIndex) {
        upgradesGUI.SetActive(true);
        troopUgGUI.RefreshGUI(unit);
        playerFrozenInMenu = true;
        troopUgGUI.indexOfUnit = unitIndex;
    }


    public void HideTroopUpgrades() {
        upgradesGUI.SetActive(false);
        playerFrozenInMenu = false;
    }

    public void ShowMenu() {        
        buildingMenu.SetActive(true);
        playerFrozenInMenu = true;
        // menuShown = true;
    }
    public void ShowChallengerUI() {
        challengerDialogue.SetActive(true);
        playerFrozenInMenu = true;
    }
    public void HideChallengerUI() {
        challengerDialogue.SetActive(false);
        playerFrozenInMenu = false;
    }
}
