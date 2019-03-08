using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCursors : MonoBehaviour {

    [SerializeField] Texture2D walkCursor = null;
    [SerializeField] Texture2D invalidCursor = null;
    [SerializeField] Texture2D attackCursor = null;
    [SerializeField] Vector2 cursorHotspot = new Vector2(0, 0); //dependent on size of the icon
    [SerializeField] const int environment = 9;
    [SerializeField] const int enemy = 10;
    [SerializeField] const int walkable = 11;


    CameraRaycaster cr;
	// Use this for initialization
	void Start () {
        cr = GetComponent<CameraRaycaster>();
        cr.notifyLayerChangeObservers += OnLayerChange;

    }

    void OnLayerChange(int newLayer) { //cursor only changes on layer change.
       Debug.Log("Cursor Icon handler. new layer: " + newLayer);
        switch (newLayer) {
            case (environment):
                Cursor.SetCursor(walkCursor, cursorHotspot, CursorMode.Auto);
                break;
            case (walkable):
                Cursor.SetCursor(walkCursor, cursorHotspot, CursorMode.Auto);
                break;
            case (enemy):
                Cursor.SetCursor(attackCursor, cursorHotspot, CursorMode.Auto);
                break;
            default:
                Cursor.SetCursor(walkCursor, cursorHotspot, CursorMode.Auto);
                return;
        }
    }
	
}
