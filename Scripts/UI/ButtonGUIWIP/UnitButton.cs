using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitButton : MonoBehaviour {

    [SerializeField] MenuManager menuManager; // got in code as unit button is being used as a prefab
    public Button buttonComponent;
   // public Text nameLabel; dont need this? 
    public Image iconImage;
    public Text unitName;
    private Unit unit;
    private ArmyScrollList scrollList;
    public Text powerText;
    public Text levelText;
    public int unitIndex;
    // Use this for initialization
    void Start() {
        buttonComponent.onClick.AddListener(HandleClick);
        menuManager = GameObject.FindGameObjectWithTag("MenuManager").GetComponent<MenuManager>();
    }

    public void Setup(Unit currentUnit, int unitIndex, ArmyScrollList currentScrollList) {
        unit = currentUnit;
        unitName.text = currentUnit.name;
        iconImage.sprite = unit.icon;
       // priceText.text = unit.price.ToString();
        scrollList = currentScrollList;
        powerText.text = "" + unit.power;
        levelText.text = "" + unit.level;
        this.unitIndex = unitIndex;
    }

    //on button click display unit specific GUI and inventory to equip them with
    public void HandleClick() {
        //scrollList.TryTransferItemToOtherShop(item);
        menuManager.ShowTroopUpgrades(unit, unitIndex);
    }

}
