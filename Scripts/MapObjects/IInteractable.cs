using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable  {
    Vector3 position { get; set; }
    void OnClick();
    void GUIOnClick();
    IInteractable GetLocation();
    string GetName();
    //menu button functionality here, maybe move to new interface called IMenuButtons
    void Interaction1();
    void Interaction2();
    List<Unit> GetArmyList(); //for GUI
    BuildingType GetBuildingType();
    bool IsInRange();
    void SetInRange(bool b);
    Challenger GetInteracting();
    Faction GetFaction();
    bool Attack(Challenger attacker);
}
