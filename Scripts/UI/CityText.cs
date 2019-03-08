using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CityText : MonoBehaviour {

    private TextMeshPro tm;
    private IStructure structure;
    public Color factionColour;

    // Use this for initialization
    void Start () {
        tm = gameObject.GetComponent<TextMeshPro>();
        //tm.text = "this";
        structure = GetComponentInParent<IStructure>();
        tm.text = structure.GetBuildingName();
        factionColour = (Color32) structure.GetTextColour();
        tm.color = factionColour;
    }
	
    public void Recolour(Color newColor) {
        tm.color = newColor;
    }
	// Update is called once per frame
	void Update () {
		
	}

}
