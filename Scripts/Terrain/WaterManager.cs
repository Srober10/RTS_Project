using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterManager : MonoBehaviour {

    public float WATERHEIGHT = 1.2f;
    public GameObject waterGO = null;
    public GameObject terrainPrefab;
    private Terrain terrain;
    private TerrainData terrainData;
    public bool hasWater = false; // dont add water if causing performance issues. ATM using optimised water as this doesnt drop frames at all :) does look a bit bland.
    //private TerrainCollider collider;

    // Use this for initialization
    void Start () {
        //this might break with multiple terrains.
        if (hasWater) {
            terrain = terrainPrefab.GetComponent<Terrain>();
            terrainData = terrainPrefab.GetComponent<TerrainCollider>().terrainData;
            SetWaterHeight(WATERHEIGHT);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (!hasWater)
            waterGO.SetActive(false);

	}
    //
    public void SetWaterHeight( float newHeight) {
        if (hasWater) {
            GameObject water = GameObject.Find("water");
            WATERHEIGHT = newHeight;
            if (!water) {

                water = Instantiate(waterGO, terrain.transform.position + new Vector3(0, WATERHEIGHT, 0), terrain.transform.rotation);
                water.name = "water";
            }
            //WATERHEIGHT * terrainData.size.y
            water.transform.position = terrain.transform.position + new Vector3(terrainData.size.x / 2,
                                                                                 WATERHEIGHT,
                                                                                terrainData.size.z / 2);
        }
    }
}
