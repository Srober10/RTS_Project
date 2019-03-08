using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



//moved this into citty text class
public class Hoverable : MonoBehaviour {

    private TextMeshPro tm;
    public Color startColor = Color.white;
    public Color endColor = Color.red;
	// Use this for initialization
    Color facColour;

    private void Awake() {
        tm = GetComponentInChildren<TextMeshPro>();
    }
    void Start () {
        facColour = GetComponent<IStructure>().GetTextColour();
        
	}

    private void OnMouseEnter() {
       tm.outlineColor = Color.black;
    }

    private void OnMouseExit() {
        tm.outlineColor = Color.white;
    }
}
