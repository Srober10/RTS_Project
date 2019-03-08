using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

public class MySelectable : MonoBehaviour, ISelectHandler, IPointerClickHandler, IDeselectHandler
{
    CameraRaycaster cr = new CameraRaycaster(); //for detecting clicks not on the gameobj, so not handled by inbuilt methods.
    public static HashSet<MySelectable> allMySelectables = new HashSet<MySelectable>();
    public static HashSet<MySelectable> currentlySelected = new HashSet<MySelectable>();
    public static bool showUnitGUI = false;
    PlayerMovement playerMovement;

    void Awake()
    {
        allMySelectables.Add(this);
        playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement.currentlyControlled) { //hopefully ?
            currentlySelected.Add(this);
        }
        Debug.Log("This is the list of selected : " + currentlySelected);
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (gameObject.tag == "Selectable" || gameObject.tag == "Untagged")
        {
            OnSelect(eventData);
            Debug.Log("Game object has been clicked and selected.");
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        gameObject.tag = "Selected";
        currentlySelected.Add(this);
        showUnitGUI = true;
        Debug.Log(this.transform.position);
        playerMovement.currentlyControlled = true; //to prevent movement bug
      
    }

    public void OnDeselect(BaseEventData eventData)
    {
        gameObject.tag = "Selectable";
        playerMovement.currentlyControlled = false;
    }

    public static void DeselectAll(BaseEventData eventData)
    {
        foreach (MySelectable selectable in currentlySelected)
        {
            selectable.OnDeselect(eventData);
        }
        currentlySelected.Clear();
    }
}