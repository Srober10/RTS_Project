using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour {

    public bool panningOn = false;
    public float panSpeed = 20f;
    public float panBorderThickness = 20f;
    public Vector2 panLimit;
    public float scrollSpeed = 30f;
    public float minY = 20f; //todo set these dynamically so it works on all terrains 
    public float maxY = 400;

    public bool followPlayer = false;
    public Transform target; // this is the default camera setting relative to the player
    public Vector3 offset; //offset of camera from player
    public float pitch = 2f; //height of playable char
    public float currentZoom = 10f;
    public Vector3 rotationValue;

	// Use this for initialization
	void Start () {
        if(target == null)
        target = GameObject.FindWithTag("Player").transform;
        
    }
	
	// Update is called once per frame
	void Update () {

        Vector3 pos = transform.position;
        if (panningOn) {            
            if (Input.GetKey("w") || Input.mousePosition.y >= (Screen.height - panBorderThickness)) {
                pos.z += panSpeed * Time.deltaTime;
            }
            if (Input.GetKey("s") || Input.mousePosition.y <= (panBorderThickness)) {
                pos.z -= panSpeed * Time.deltaTime;
            }
            if (Input.GetKey("d") || Input.mousePosition.x >= (Screen.width + panBorderThickness)) {
                pos.x += panSpeed * Time.deltaTime;
            }
            if (Input.GetKey("a") || Input.mousePosition.x <= (panBorderThickness)) {
                pos.x -= panSpeed * Time.deltaTime;
            }
        }
        // much better for testing
         {
            float inputX = Input.GetAxis("Horizontal") * 100f * Time.deltaTime;
          if (Input.GetKey("w")) {
                pos.z += panSpeed * Time.deltaTime;
            }
            if (Input.GetKey("s") ) {
                pos.z -= panSpeed * Time.deltaTime;
            }
            if (Input.GetKey("d") ) {
                pos.x += panSpeed * Time.deltaTime;
            }
            if (Input.GetKey("a") ) {
                pos.x -= panSpeed * Time.deltaTime;
            }

        }
        //on mouse scroll update position of camera and zoom var
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        float scrollMultiplier = 10f * Time.deltaTime;
        currentZoom-= scroll * scrollSpeed * scrollMultiplier;
        //pos.y -= scroll * scrollSpeed * scrollMultiplier;

        //get rotation values and rotate if q or e are pressed
        if (Input.GetKey("q") || Input.GetKey("e")) {
            float y = Input.GetAxis("Mouse X");
            float x = Input.GetAxis("Mouse Y");
            rotationValue = new Vector3(x, y * -1, 0);
            transform.eulerAngles = transform.eulerAngles - rotationValue;
        }

        //clamp values into sensible ranges
        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        pos.z = Mathf.Clamp(pos.z, -panLimit.y, panLimit.y);
        
	}

    public void LateUpdate() {
        //follows player automatically if toggled
        if (followPlayer) {
            transform.position = target.position - offset * currentZoom;
            transform.LookAt(target.position + Vector3.up * pitch);
        }
    }
}
