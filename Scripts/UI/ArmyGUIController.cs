using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmyGUIController : MonoBehaviour {

    public Challenger player;
    private List<Unit> playerArmy;
    public GameObject buttonPrefab;
    public Sprite debug;
    public Transform contentPanel; // location to parent the button to
    private bool painted = false;
    int numPainted = 0;

    public Text troops;
    public Text wages;
    public Text morale;
    public Text maxTroops;
    private Transform contentPanelTransform;
    int buttonAdded;

    // Use this for initialization
    void Start () {

       // player = GameObject.FindWithTag("Player").GetComponent<Challenger>(); assigned in inspetor
        //GenerateIcon(0); //generate first unit only as army is empty apart from leader
        playerArmy = player.GetArmy();
       // Debug.Log("army: " + playerArmy);
        // GameObject.Find("Menu Manager").GetComponent<MenuManager>();
        troops.text = "" + playerArmy.Count;
        //contentPanelTransform = 
        buttonAdded = 1;
    }
	
    // repaint basically
    // painting only one at a time...... .TODO FIX
    public void GenerateIcon(int index) {
        playerArmy = player.GetArmy();

       // Debug.Log("PlAYER ARMY " + playerArmy);
        GameObject newButton = Instantiate(buttonPrefab) as GameObject;
        newButton.name = "AnnoyingUnityIs: " + index;
        Debug.Log(newButton + "is this here?? hello" );
        newButton.GetComponent<Image>().sprite = playerArmy[index].icon;
        newButton.SetActive(true);
        newButton.transform.SetParent(contentPanel, false);
        UpdatePlayerGUI();
        //Debug.Log("This is army: " + playerArmy + " With size: " + playerArmy.Count);   
    }

    public void UpdatePlayerGUI() {
        troops.text = "" + playerArmy.Count;
        maxTroops.text = "" + player.maxArmySize;
    }
    
}
