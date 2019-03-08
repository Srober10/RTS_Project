using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {

    private Camera camera;
	// Use this for initialization
	void Start () {
        camera = Camera.main;
	}
	
	// rotates the gameobject to be facing the camera every frame
	void Update () {
        transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward,
                           camera.transform.rotation * Vector3.up);
	}
}
