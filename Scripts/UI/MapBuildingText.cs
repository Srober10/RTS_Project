using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapBuildingText : MonoBehaviour {
    //general methods needed for all the buildings in the game, methods called from buttons
    //used to set the text for the menu gui to whichever Interactable object menu the player is currently in. 

    public Text resourceAmount;
    public Text type;
    public Text buildingName;
    public Text buildingInfo;
    public Text CDTitle;
    //these are set as gameobjects so the menu adjusts the layout group when the button is disabled
    public GameObject buttonObj1;
    public GameObject buttonObj2;
    private Button b1;
    private Button b2;
    private Challenger player;
    public GameObject menu;
    private bool menuShown = false;
    public MenuManager manager;
    IInteractable location;
    Town townRef;
    Village villageRef;
    BuildingType b;
    public GameObject cdImage;
    // Use this for initialization
    void Start () {
        player = GameObject.FindWithTag("Player").GetComponent<Challenger>();
        // type.text = player.location.toString(); //gets type
        //this is too soon, causes null errors
        //location = player.location;
        //TextSet();
        //try to cast IInteractable to town
        location = player.location.GetLocation();
        b1 = buttonObj1.GetComponent<Button>();
        b2 = buttonObj2.GetComponent<Button>();
        resourceAmount.text = player.gold + " gold"; //set resource
    }

    //todo remove this from update and call on click only.
    // Update is called once per frame

    public void UpdateRecruitCount() {
        if (player.location != null && player.location.GetArmyList() != null && player.location.GetArmyList().Count> 0) {
            var p = player.location.GetArmyList();       
            int count = 0;
            foreach (Unit u in p) {
                count += u.currentSize;
            }
            count = Mathf.Clamp(count, 0, 100); //testing
            buildingInfo.text = p.Count + " squadrons recruitable. \n " + count + " recruits in total for 1 each";
        }
    }

    public void TextSet() {

        location = player.location.GetLocation();
        if (location != null) {
            b = location.GetBuildingType();
           // Debug.Log("location faction is " + location.GetFaction().factionName);
            //Debug.Log("player faction is " + player.faction.factionName);
            type.text = b.ToString();
            buildingName.text = location.GetName();
            bool enemyStructure = !player.faction.Equals(location.GetFaction()); // true if the faction of the player is different to the faction of interacted town etc. TODO compare with list of enemy factions for more than 1v1
          
            switch (b) {
                case (BuildingType.Capital):
                    var capital = (Town)location;
                   //UpdateRecruitCount();
                    cdImage.SetActive(false);
                    if (enemyStructure) {
                        b1.GetComponentInChildren<Text>().text = "Attack Enemy Capital";
                        buttonObj1.SetActive(true);
                        buttonObj2.SetActive(false);
                        b2.GetComponentInChildren<Text>().text = "This is an enemy capital";

                    }
                    else {
                        b1.GetComponentInChildren<Text>().text = "This is an allied Capital ";
                        buttonObj1.SetActive(false);
                        buttonObj2.SetActive(true);
                        CDTitle.text = "";
                        if (player.gold > 0) {
                            if (player.HasRefillableRecruits()) {
                                b2.GetComponentInChildren<Text>().text = "Replenish recruits";
                            }
                            else {
                                b2.GetComponentInChildren<Text>().text = "Hire new Recruits";
                            }
                        }
                        else if (player.gold <= 0) {
                            b2.GetComponentInChildren<Text>().text = "You have no gold";
                        }
                        else {
                            OutOfUnitsText();
                        }

                    }
                  
                    break;

                case (BuildingType.Town):

                    cdImage.SetActive(false);
                   // UpdateRecruitCount();
                    if (enemyStructure) {

                        buttonObj1.SetActive(true);
                        b1.GetComponentInChildren<Text>().text = "Attack Enemy Town";
                        buttonObj2.SetActive(false);
                        b2.GetComponentInChildren<Text>().text = "This is an enemy town";
                    }
                    else {
                        buttonObj1.SetActive(false);
                        b1.GetComponentInChildren<Text>().text = "This is an allied Town ";
                        buttonObj2.SetActive(true);
                        CDTitle.text = "";
                        if (player.gold > 0) {
                            if (player.HasRefillableRecruits()) {
                                b2.GetComponentInChildren<Text>().text = "Replenish recruits";
                            }
                            else {
                                b2.GetComponentInChildren<Text>().text = "Hire new Recruits";
                            }
                        }
                        else if (player.gold <= 0) {
                            b2.GetComponentInChildren<Text>().text = "You have no gold";
                        }
                        else {
                            OutOfUnitsText();
                        }
                    }
                  
                    break;

                case (BuildingType.Outpost):

                    cdImage.SetActive(false);
                    b1.GetComponentInChildren<Text>().text = "Visit Outpost";
                    break;
                default:
                    b1.GetComponentInChildren<Text>().text = "Visit Area";
                    break;
                    //village interaction 1 is gather, 2 is attack
                case (BuildingType.Village):
                    var vref = (Village)location.GetLocation();
                    cdImage.SetActive(true);
                    if (enemyStructure) {
                        buttonObj2.SetActive(true);
                        buttonObj1.SetActive(false);
                        b2.GetComponentInChildren<Text>().text = "Attack Enemy Village";
                        b1.GetComponentInChildren<Text>().text = "This is an enemy Village ";

                    }
                    else {
                        buttonObj2.SetActive(false);
                        buttonObj1.SetActive(true);
                        if (vref.resourceCount >= 1) {
                            b1.GetComponent<Button>().interactable = true;
                            vref.resourceTimer.fillAmount = vref.GetPercentageCD();
                            if (vref.GetPercentageCD() > 0)
                                CDTitle.text = "Collect CD";
                        }
                        else {
                            b1.GetComponent<Button>().interactable = false;
                            vref.resourceTimer.fillAmount = vref.GetPercentageCD();
                        }
                        b1.GetComponentInChildren<Text>().text = "Collect Resources";                        
                        b2.GetComponentInChildren<Text>().text = "This is an allied Village ";
                    }                  
                  
                    break;
            }
        }
    }

    
    public void OutOfUnitsText() {
        b2.GetComponentInChildren<Text>().text = "No units in town";
    }
    //TODO remove this later to a listener thing?
	void Update () {
        // type.text = player.location.toString(); //gets type
        TextSet();
        resourceAmount.text = player.gold + "";
    }

    public void ExitButton() {
        HideMenu();
        manager.SetPlayerFrozen(false);
    }

    public void HideMenu() {
        menu.SetActive(false);
        menuShown = false;
        manager.SetPlayerFrozen(false);
    }
    // todo probally remove this
    public void ShowMenu() {
        menu.SetActive(true);
        // menuShown = true;
        manager.SetPlayerFrozen(true);
    }

    //gets the button method implemented by the interface of the object and calls it 
    public void Button1() {
        player.location.Interaction1();     

    }
    public void Button2() {
        Debug.Log("called from mbt + " + player.location);
        player.location.Interaction2();
      
    }
}
