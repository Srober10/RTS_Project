using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickListener : MonoBehaviour {
    Challenger player;
    GameObject playerGO;
    PlayerMovement pmove; //find playermovement script on the player
    public MenuManager menuManager; //set in inspector
    MapBuildingText textButtons;
    public float interactRadius = 20;
    // Use this for initialization
    void Start () {
        pmove = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        player = GameObject.FindWithTag("Player").GetComponent<Challenger>();
        playerGO = GameObject.FindWithTag("Player");
        textButtons = menuManager.GetComponent<MapBuildingText>();
    }
	
	// Raycast if player clicks
	void FixedUpdate () {

        // have to click on top of object for raycast to hit from camera.. aim to click in middle
        if (Input.GetMouseButtonDown(0)) {
            if (EventSystem.current.IsPointerOverGameObject()) {
                //mouse is over UI element so clicking on UI and not game world.
                return;
            }
            if (!InteractionCheck() && menuManager.playerFrozenInMenu == false) {
                //Debug.Log("moving to location");
                pmove.MoveHere();
            }
        }
        if (Input.GetMouseButtonDown(1)) {
            //gets the game object of clicked item 
                // game object.onClick()                            
        }
    }

    //rename to map interact
    public bool InteractionCheck() {
        //using mask to filter movement from objects 
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
       // Debug.Log("Location of player: " + player.location);
        //ignoring triggers around interactables
        if (Physics.Raycast(ray, out hit, 1000, -5, QueryTriggerInteraction.Ignore)) {
            IInteractable i = hit.collider.GetComponent<IInteractable>();
            Challenger c = hit.collider.GetComponent<Challenger>();
            //call i's method and update player position to equal this new interactable object
            //if statements in order of priority to handle
            if( c != null) {
               // Debug.Log("Click on Challenger recognized");
                //get distance to check if in range
                float distance = Vector3.Distance(playerGO.transform.position, c.transform.position);
                if (distance <= interactRadius) {
                    player.interactingWith = c;
                    menuManager.chalText.SetText(c);
                    menuManager.ShowChallengerUI();
                }
                else {
                   // Debug.Log("not in range of challenger !");
                }
                // player.CombatCalculation(c); //this works but might be better to show the GUI first
                return true;
            }
            else if (i != null) {

                //Debug.Log("Location of player before: " + player.location);
                //sets the players location to the game object and updates which armyList to transfer to and from
                //Debug.Log("Hit interactable...");
                //get distance to check if in range
                float distance = Vector3.Distance(playerGO.transform.position, i.GetLocation().position);
                if (distance <= interactRadius) {
                    player.location = i.GetLocation();

                    //Debug.Log("Location of player after: " + player.location);
                    textButtons.TextSet(); //sets buttons on GUI to relevant location
                    textButtons.UpdateRecruitCount();
                    //player.ChangeOtherArmyList(i.GetArmyList()); not using army lists components in towns using Unit lists
                    i.OnClick();
                    i.GUIOnClick();
                }
                else {
                   // Debug.Log("not in range of interactabel! " + distance);

                }
          
                return true;
            }
            //and for other interactable scripts 
            return false;
        }
        return false;
    }
}
