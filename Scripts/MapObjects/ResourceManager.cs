using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour {

    public int resourcesLeft = 1000;
    [SerializeField] string resourceName = "Ore";
    public bool beingUsed = false;
    public bool active = true;
    public int level = 1; // level increased by mines etc

    CameraRaycaster cr;
    // Use this for initialization
  

   
}
