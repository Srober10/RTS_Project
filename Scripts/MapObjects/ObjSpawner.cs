using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class ObjSpawner : MonoBehaviour {
    public Terrain terrain;
    public WaterManager waterMan;
    [SerializeField]
    public List<Faction> factions;
    public int numberOfObjects; // number of objects to place
    private int currentObjects; // number of placed objects
    public GameObject[] objectToPlace; // GameObject to place
    public GameObject[] enemyPrefabs;
    public GameObject[] playerPrefabs;
    public Vector3[] spawnPoints; //maybe randomise this

    private int terrainWidth; // terrain size (x)
    private int terrainLength; // terrain size (z)
    private int terrainPosX; // terrain position x
    private int terrainPosZ; // terrain position z
    private bool generation = false;
    public float cubeOffset;
    private float waterHeight;
    //image for recruits
    public Sprite troopPic;

    //faction stuff
    private int[][] factionMap;
    // helpful
    public float targetPoints = 10; //target number of buildings in each faction
    public int buildingSpacing = 50; //space betweeen buildings
    public Transform pointAPos; //to visualise/place manually
    public Transform pointBPos; //to visualise/place manually
    public Vector2 pointA; //starting point for faction A
    public Vector2 pointB; //starting point for faction B 
    float[] rates;
    [SerializeField]
    public List<Transform> capitalsPos;
    List <Vector2> freePoints;
    List<Vector2> factionAPoints;
    List<Vector2> possibleFAPoints;
    List<Vector2> factionBPoints;
    List<Vector2> possibleFBPoints;
    //list of transforms as a list per faction
    List<List<Vector2>> factionBuildings;

    private GameObject holderObject; // to hold all the dup child objs so inspector isnt cluttered
    void Start() {
        // gets terrain dimensions and positions
        terrainWidth = (int)terrain.terrainData.size.x;       
        terrainLength = (int)terrain.terrainData.size.z;       
        terrainPosX = (int)terrain.transform.position.x;        
        terrainPosZ = (int)terrain.transform.position.z;
        generation = false;
        holderObject = new GameObject("objectHolder");
        waterHeight = waterMan.WATERHEIGHT;

        factionAPoints = new List<Vector2>();
        factionBPoints = new List<Vector2>();

        //setting pointA/B to inspectors transform
        pointA = pointAPos.position;
        pointB = pointBPos.position;
        //this is public for debugg
        // factions = GenerateFactions();
        GenerateFactionMap2(); //80%of map should be factions
        GenerateFactionsObjects();
        //SpawnAttempt2();
    }
   
    void Update() {
        // generate objects if not at max objects ---> might want to put this in start or delay the refresh rate.
       // if(!generation)
        //GenerateObjects(); old random generation
    }

    //currently using inspector to set these
    /* public List<Faction> GenerateFactions() {
        List<IStructure> unoCityList = new List<IStructure>();
        List<IStructure> twoCityList = new List<IStructure>();
        //hardcoded factions
        Faction faction1 = new Faction("Faction uno",unoCityList, 10);
        Faction faction2 = new Faction("Faction 2", twoCityList, 100);
        List<Faction> temp = new List<Faction>();
        temp.Add(faction1);
        temp.Add(faction2);
        return temp;
     } */

    private void GenerateRates() {
        float totalFacPower = 0;
        rates = new float[factions.Count];
        foreach (Faction f in factions) {
            totalFacPower += f.startingStrength;
        }
        for (int i = 0; i < factions.Count; i++) {
            //round should make it so theres no over / underfilling of array i hope ?? dont need to dividide by total ?
            //clamped and rounded to get a number between 1 and 10 
            rates[i] = (int)(factions[i].startingStrength / totalFacPower * 10);

        }
    }

    public VectorTuple SplitPointsUp(int xSplit, int ySplit, List<Vector2> points) {
        List<Vector2> fA = new List<Vector2>();
        List<Vector2> fB = new List<Vector2>();
        if (xSplit > Mathf.Sqrt(points.Count) || ySplit > Mathf.Sqrt(points.Count)) {
          //  Debug.Log("split is too uneven??");
        }
        foreach(Vector2 p in points) {
            if(p.x < xSplit && p.y < ySplit) {
                fA.Add(p);
            } else {
                fB.Add(p);
            }
        }
       //Debug.Log("fA len: " + fA.Count);
       // Debug.Log("fB len: " + fB.Count);
        return new VectorTuple(fA, fB);
    }
    public void GenerateFactionMap(float spaceProportion) {
        //ignore y for generating faction boundries
        int xStart = terrainPosX;
        int zStart = terrainPosZ;
        int xEnd = terrainPosX + terrainWidth;
        int zEnd = terrainPosZ + terrainLength;
        freePoints = MapPoints(xEnd, zEnd, 10);
       // Debug.Log("free points: " + freePoints.Count);
        //work out how much of the map will be each faction
        int mapArea = xEnd * zEnd;
        int targetFactionsArea = (int) Mathf.Floor (mapArea * spaceProportion);       
        //rate is proportional to the ratio of the faction strengths 
        //sorting faction by strength
        List<Faction> sortedfactions = factions.OrderBy(factions => factions.startingStrength).ToList();
        GenerateRates();
        //Debug.Log("rates are: " + rates[0] + " " + rates[1]);
        //maybe make these relative? currently assigned in inspector
      


    }

    //method without using spread factions 
    //sets faction A and Bs points 
    public void GenerateFactionMap2() {
        //ignore y for generating faction boundries
        int xStart = terrainPosX;
        int zStart = terrainPosZ;
        int xEnd = terrainPosX + terrainWidth;
        int zEnd = terrainPosZ + terrainLength;
        //create list of all valid points on map with increment of 10
        freePoints = MapPoints(xEnd, zEnd, 10);
        //Debug.Log("free points: " + freePoints.Count);
        //rate is proportional to the ratio of the faction strengths 
        GenerateRates();
        float targetAPoints = rates[0] * 5;
        float targetBPoints = rates[1] * 5;
        //split map points into faction A and B so no overlap where x < 200 && y <200 are in A otherwise B
        possibleFAPoints = SplitPointsUp(200, 200, freePoints).GetFirst();
       // Debug.Log("FaP len here: " + possibleFAPoints.Count);
        possibleFBPoints = SplitPointsUp(200, 200, freePoints).GetSecond();
        bool generate = true;
        while (generate) {
            while (factionAPoints.Count < targetAPoints) {
                if (targetAPoints <= 0) break;
                int r = (int)UnityEngine.Random.Range(0, possibleFAPoints.Count - 1);
                factionAPoints.Add(possibleFAPoints[r]);
                //Debug.Log("int r is: " + r + "Fac a points len is: " + possibleFAPoints.Count);
                possibleFAPoints.RemoveAt(r);
            }
            while (factionBPoints.Count < targetBPoints) {
                if (targetBPoints == 0) break;
                int r = (int)UnityEngine.Random.Range(0, possibleFBPoints.Count - 1);
                factionBPoints.Add(possibleFBPoints[r]);
                possibleFBPoints.RemoveAt(r);
            }
            generate = false;
        }


    }

    //given starting point/list of current points of faction, generate rate amount of neighbours from random elements of
    //factionPoints
    // modifies the global cariable freePoints
    public List<Vector2> SpreadFaction(Vector2 start, List<Vector2> factionPoints, int rate) {
       // Debug.Log("Spread faction called! ");
        List<Vector2> newFactionPoints = factionPoints;
        List<Vector2> possiblePoints = freePoints;
        List<Vector2> neighbours = new List<Vector2>();
        if (newFactionPoints.Count == 0) {
            newFactionPoints.Add(start);
        }
        //else {
            //generate neighbours for all points in factionPoints and get intersection with freePoints
            //pick rate number of points randomly, remove thse from free points and add to newFactionPoints and return        
          

                foreach( Vector2 v in newFactionPoints) {
            //this method assumes terrain is at 0,0
            //concats the old list of points to new //UnityEngine.Random.Range(buildingSpacing/1.2f, buildingSpacing*1.2f )
                    neighbours.AddRange( Utils.GenerateNeighbours_M(v, terrainPosX + terrainWidth, 
                                                                        terrainPosZ + terrainLength,(int)buildingSpacing));
               // Debug.Log("In spread faction: " + neighbours.Count);
                }
                                                                     
                var newPossiblePoints = freePoints.Intersect(neighbours).ToList();
            //pick points randomly
            for (int i = 0; i < rate; i++) {
                if (newPossiblePoints.Count > 0) {
                    int index = UnityEngine.Random.Range(0, newPossiblePoints.Count);
                    Debug.Log("pos points coiunt: " + newPossiblePoints.Count);
                    newFactionPoints.Add(newPossiblePoints[index]);
                    freePoints.Remove(newPossiblePoints[index]);
                }
            }
        return newFactionPoints;
    
    }
    // maybe see if dupes arent a bad thing, this will weight factions to expand less which will probably be a good idea
    public List<Vector2> GenerateNeighbours2 (List<Vector2> list, int xMin, int xMax, int yMin, int yMax, int increment) {
        //fo each point in list, gives adjacent points to the given point
        List<Vector2> neighbours = new List<Vector2>();
        foreach( Vector2 point in list) {
            if( point.x + increment < xMax && point.x > xMin && point.y > yMin && point.y < yMax
                            && ! neighbours.Contains(point)) {
                neighbours.Add(point);
            } // else out of range or a dupe so dont add
        }
        return neighbours;
    }

    //given width and height gets all integer points on a grid. 
    // avoids placing them on water
    //maybe chunk this in 10 or 100 etc with increment 
    public List<Vector2> MapPoints(int width, int length, int increment) {
        List<Vector2> points = new List<Vector2>();
        int maxTerrainSlope = 30;
        int maxHeight = 50;
        for( int i =0; i< width; i+=increment) {
            for(int j =0; j < length; j+=increment) {
                float posy = Terrain.activeTerrain.SampleHeight(new Vector3(i, 0, j)); //finds the y given x and z 
                if (posy > waterHeight && posy < maxHeight && terrain.terrainData.GetSteepness(i,j) < maxTerrainSlope) {
                    points.Add(new Vector2(i, j));
                } //else this point is not valid as its in water, so dont add to list of valid points.
            }
        }
        return points;

    }
    //pair of List<Vector2> as unity doesnt support tuple class from c# 
    //TODO move to utils
    public class VectorTuple {
        List<Vector2> first;
        List<Vector2> second;
        public VectorTuple(List<Vector2> a, List<Vector2> b) {
            first = a;
            second = b;
        }
        public List<Vector2> GetFirst() {
            return first;
        }
        public List<Vector2> GetSecond() {
            return second;
        }
    }
    //pick n adjacent points in a list and return list of those points and old list w.o the,
    private VectorTuple PickNPoints (List<Vector2> list, int n) {
        List<Vector2> temp = new List<Vector2>();
      //  list.OrderBy<>;
        if (list.Count == n) {
            return new VectorTuple(list,temp);
        }
        if (list.Count < n) { 
            //error 
            Debug.Log("Tried to pick " + n + "points from a " + list.Count + "length list!");
            return new VectorTuple(list, temp);
        }
        else { //(list.Count > n) {
            //pick all n to the right of the starting point
            //int index = UnityEngine.Random.Range(0, list.Count - n-1); removing the randomness because its not really adding anything visually??
           
            for (int i = 0; i < n; i++) { //remove/add n items
                temp.Add(list[i]);
                list.RemoveAt(i);
            }
            return new VectorTuple(temp, list);
        }
        //
      

    }

    private void SpawnAttempt2() {

        GameObject capital = objectToPlace[0]; //one per faction
        GameObject city = objectToPlace[1]; //bigger town, has some towns associated with it
        GameObject village = objectToPlace[2]; //placed near asociated city

        //split map into quarters
        int xboundry = terrainWidth / 2;
        int zboundry = terrainLength / 2;
        List<Vector2> citiesToPlace;
        List<Vector2> villagesToPlace;
        //radius / square length around capital based on relative strength. same with number of cities
        //between 2-5 cities based on relative starting strength
        var capitals = capitalsPos;
        int count = 0;
        GenerateRates();
        foreach(var c in capitals) {
            //place city
            float posy = Terrain.activeTerrain.SampleHeight(new Vector3(c.position.x, 0, c.position.z)); //finds the y given x and z 
            GameObject newObject = (GameObject)Instantiate(capital, new Vector3(c.position.x, posy, c.position.z), Quaternion.identity);
            //get associated cities and place these
            citiesToPlace = RaycastAttempt(c.position, rates[count], terrainWidth / 2 - 4);
            foreach(Vector2 v in citiesToPlace) {

                float posY = Terrain.activeTerrain.SampleHeight(new Vector3(c.position.x, 0, c.position.y)); //finds the y given x and z 
                GameObject cityObj = (GameObject)Instantiate(city, new Vector3(c.position.x, posY, c.position.y), Quaternion.identity);
                villagesToPlace = RaycastAttempt(c.position, 2, 5); //always 2 villages per city

                //foreach(Vector2 vil in villagesToPlace) {
                    float posY2 = Terrain.activeTerrain.SampleHeight(new Vector3(v.x, 0, v.y)); //finds the y given x and z 
                    GameObject villageObj = (GameObject)Instantiate(village, new Vector3(v.x, posY2, v.y), Quaternion.identity);
               // }
            }
        }

    }


    //at the moment this is in the inspector as this isnt working as intended, might be better to hardcode.
    private List<Vector2> PlaceCapitals(List<Faction> factions) {
        int margin = 10;
        int maxSlope = 30;
        List<Vector2> capitals = new List<Vector2>();
        while(capitals.Count < factions.Count) {
            int randomIncrement = UnityEngine.Random.Range(terrainLength / 4, terrainWidth / 4);
            for (int i = terrainPosX + margin; i < terrainWidth -margin; i += randomIncrement) {
                for(int j= terrainPosZ + margin; j < terrainLength - margin; j+=randomIncrement) {
                    if (IsValidPoint(i, j)) {
                        capitals.Add(new Vector2(i, j));
                        break;
                    }
                }
            }
        }

        Debug.Log("capitals coords: " + capitals[0] + " " + capitals[1] + " " + capitals[2] + " " + capitals[3]);
        return capitals;
  
    }

    private bool IsValidPoint(int xPos, int zPos) {
        int margin = 10;
        int maxSlope = 30;
      // get the terrain height at the random position, including for the fact that we dont want any objects below the water line, and accounting for the fact water height may? be negative

       float posy = Terrain.activeTerrain.SampleHeight(new Vector3(xPos, 0, zPos)); //finds the y given x and z 
        if (posy > waterHeight && terrain.terrainData.GetSteepness(xPos, zPos) < maxSlope) {
            return true;
        }
        else {
            return false;
        }
        
    }
    private List<Vector2> RaycastAttempt(Vector2 start, float cities, int radius) {
       
        List<Vector2> points = new List<Vector2>();
        // int increment = UnityEngine.Random.Range( 360 / cities)
        //V.x = cos(A)
        // V.y = sin(A)
        int incr = (int)(360f / cities);
        for (int i =0; i <= 360; i+=incr) {
            Vector2 v = new Vector2(Mathf.Cos(i), Mathf.Sin(i));
            
            int scale = UnityEngine.Random.Range(1, radius/3);
            Vector2 end = start + v*scale;
            if (IsValidPoint( (int)end.x, (int)end.y) ){
                points.Add(scale * v);
            }
        }
        Debug.Log("points len: " + points.Count);
        return points;
    }

    public void GenerateFactionsObjects () {

        Faction alliesFac = factions[0];
        Faction enemiesFac = factions[1];
        PlaceBuildings2(factionAPoints, alliesFac);
        PlaceBuildings2(factionBPoints, enemiesFac);
        //spawn the prefabs of the player and enemy, set factions here
        PlaceChallengers(enemyPrefabs, alliesFac);
        PlaceChallengers(playerPrefabs, enemiesFac);

    }

    //hardcoded starting positions
    //move them to position
    public void PlaceChallengers(GameObject[] array, Faction f) {
       
        for(int i =0; i < array.Length; i++) {
            array[i].GetComponent<Transform>().position.Set( spawnPoints[i].x, spawnPoints[i].y, spawnPoints[i].z ); // this isnt working?? 
            array[i].GetComponent<Challenger>().SetFaction(f);
        }
    }

    void PlaceBuildings2(List<Vector2> factionPoints, Faction fac) {
        GameObject capital = objectToPlace[0]; //one per faction, can hire troops or get resources
        GameObject city = objectToPlace[1]; //can hire troops here 
        GameObject village = objectToPlace[2]; //can get resources here

        fac.SetChallengersToFaction(); //makes sure challengers in list of faction have this faction as their faction
        //Debug.Log(" length is : " + factionPoints.Count);
        var result = PickNPoints(factionPoints, 1);
        //place city
        float posy = Terrain.activeTerrain.SampleHeight(new Vector3(result.GetFirst()[0].x, 0, result.GetFirst()[0].y)); //finds the y given x and z 
                                                                                                                         //generate the object
        Vector3 capPos = new Vector3(result.GetFirst()[0].x, posy + cubeOffset, result.GetFirst()[0].y);
        GameObject capitalObj = (GameObject)Instantiate(capital, capPos, Quaternion.identity);
        Town capitalRef = capitalObj.GetComponent<Town>();
        //set capital cities properties
        capitalRef.Setup(true, 3, 10, fac, capPos); //todo randomise as appropr
        capitalRef.buildingName = SpawnManager.AssignName();
        capitalRef.AddTroops(new Unit(troopPic, 10),10); //10 as setup is 10 above
        fac.capital = capitalRef; //sets capital of faction in class
        factionAPoints = result.GetSecond();
        int aSize = factionAPoints.Count;
        int offset = aSize % 3; //we want every town to have 2 villages so we are splitting array into chunks of 3
        VectorTuple threePoints;
        List<Vector2> t;
        VectorTuple t2;
        Vector2 townPoint;
        List<Vector2> villagePoints;
        float posY;
        //int count = 1;
        for (int i = 0; i < targetPoints / 2; i += 3) {
            threePoints = PickNPoints(factionAPoints, 3);
            factionAPoints = threePoints.GetSecond();
            t = threePoints.GetFirst();
            t2 = PickNPoints(t, 1);
            townPoint = t2.GetFirst()[0];//this is the town, the other 2 are the attached smaller villages
            villagePoints = t2.GetSecond();
            posY = Terrain.activeTerrain.SampleHeight(new Vector3(townPoint.x, 0, townPoint.y)); //finds the y given x and z 
            //placing town
            Vector3 townPos = new Vector3(townPoint.x, posY, townPoint.y);
            GameObject cityObj = (GameObject)Instantiate(city, townPos, Quaternion.identity);
            Town cityRef = cityObj.GetComponent<Town>();
            //set capital cities properties
            cityRef.Setup(false, 2, 5, fac, townPos); //todo randomise as appropr
            cityRef.buildingName = SpawnManager.AssignName();
            cityRef.AddTroops(new Unit(troopPic, 5), 5);
            fac.towns.Add(cityRef); //adds town
            //place villages
            //Debug.Log("village length (should be 2): " + villagePoints.Count);
            foreach (Vector2 v in villagePoints) {
                float y = Terrain.activeTerrain.SampleHeight(new Vector3(v.x, 0, v.y)); //finds the y given x and z 
                Vector3 pos = new Vector3(v.x, y + cubeOffset, v.y);
                GameObject villageObj = (GameObject)Instantiate(village, pos, Quaternion.identity);
                //setup village objects (TODO)
                Village vilRef = villageObj.GetComponent<Village>();
                vilRef.buildingName = SpawnManager.AssignName();
                vilRef.Setup(fac, pos);
                fac.villages.Add(vilRef);
            }
            //factionAPoints = threePoints.GetSecond();
            // count += 3;
        }
    }

   /* void PlaceBuildings(List<Vector2> factionPoints) {
        GameObject capital = objectToPlace[0]; //one per faction
        GameObject city = objectToPlace[1]; //bigger town, has some towns associated with it
        GameObject village = objectToPlace[2]; //placed near asociated city
        //faction A
       // Debug.Log(factionAPoints.Count + " faction A buildings to place");

        var result = PickNPoints(factionPoints, 1);
        //place city
        float posy = Terrain.activeTerrain.SampleHeight(new Vector3(result.GetFirst()[0].x, 0, result.GetFirst()[0].y)); //finds the y given x and z 
                                                                                                                         //generate the object
        GameObject newObject = (GameObject)Instantiate(capital, new Vector3(result.GetFirst()[0].x, posy + cubeOffset, result.GetFirst()[0].y), Quaternion.identity);
        factionAPoints = result.GetSecond();
        int aSize = factionAPoints.Count;
        int offset = aSize % 3; //we want every town to have 2 villages so we are splitting array into chunks of 3
        VectorTuple threePoints;
        List<Vector2> t;
        VectorTuple t2;
        Vector2 townPoint;
        List<Vector2> villagePoints;
        float posY;
        //int count = 1;
        for(int i =0; i< targetPoints/2; i+=3) {
            threePoints = PickNPoints(factionAPoints, 3);
            factionAPoints = threePoints.GetSecond();
            t = threePoints.GetFirst();
             t2 = PickNPoints(t, 1); 
             townPoint = t2.GetFirst()[0];//this is the town, the other 2 are the attached smaller villages
             villagePoints = t2.GetSecond();
             posY = Terrain.activeTerrain.SampleHeight(new Vector3(townPoint.x, 0, townPoint.y)); //finds the y given x and z 
            //placing town
            GameObject cityObj = (GameObject)Instantiate(city, new Vector3(townPoint.x, posY + cubeOffset, townPoint.y), Quaternion.identity); 
            //place villages
            //Debug.Log("village length (should be 2): " + villagePoints.Count);
            foreach (Vector2 v in villagePoints) {
                float y = Terrain.activeTerrain.SampleHeight(new Vector3(v.x, 0, v.y)); //finds the y given x and z 
                GameObject villageObj = (GameObject)Instantiate(village, new Vector3(v.x, y + cubeOffset, v.y), Quaternion.identity);
            }
            //factionAPoints = threePoints.GetSecond();
           // count += 3;
        }

    } */
           
        

    // old method, randomly places 
    public void GenerateObjects() {

         cubeOffset = 2.0f;
        if (currentObjects <= numberOfObjects) {
           
            // create new gameObject on random position
            foreach (GameObject g in objectToPlace) {
                currentObjects = 0;
                
                while (currentObjects < numberOfObjects) {
                    
                    // generate random x and z position
                    int posx = Random.Range(terrainPosX, terrainPosX + terrainWidth);
                    int posz = Random.Range(terrainPosZ, terrainPosZ + terrainLength);
                    // get the terrain height at the random position, including for the fact that we dont want any objects below the water line, and accounting for the fact water height may? be negative
                    
                    float posy = Terrain.activeTerrain.SampleHeight(new Vector3(posx, 0, posz)); //finds the y given x and z 
                    if (posy > waterHeight) {
                        GameObject newObject = (GameObject)Instantiate(g, new Vector3(posx, posy + cubeOffset, posz), Quaternion.identity);
                        currentObjects++;
                    } else {
                        //continue trying positions
                        //increase counter still (maybe can remove this for smaller spawn numbers incase no spawns)
                        currentObjects++;
                    }
                    //g.transform.SetParent(holderObject.transform); // makes objects child of this holder obj for neatness
                }
            }
        }
        if (currentObjects == numberOfObjects) {
           // Debug.Log("Generate objects complete!");
            generation = true;
        }
    }
}