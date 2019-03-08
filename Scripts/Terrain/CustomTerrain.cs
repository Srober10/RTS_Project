
using System.Linq;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;
using System;

//so this runs in editor

[ExecuteInEditMode]


public class CustomTerrain : MonoBehaviour {

    public Vector2 randomHeightRange = new Vector2(0, 0.1f);
    public Texture2D heightMapImage;
    public Vector3 heightMapScale = new Vector3(1, 1, 1);
    public Terrain terrain = null;
    public TerrainData terrainData = null;

    public bool resetTerrain = true;

    //perlin noise scaling floats, as 1 is flat----------------
    public float perlinXScale = 0.01f;
    public float perlinYScale = 0.01f;
    public int perlinXOffset = 0;
    public int perlinYOffset = 0;
    public int perlinOctaves = 3;
    public float perlinPersistance = 8;
    public float perlinHeightScale = 0.09f;
    public float freqMultiplier = 2;

    //multiple perlin noise ----------------------
    //create a class to keep track of each type of perlin used
    [System.Serializable]
    public class PerlinParameters {
        public float mPerlinXScale = 0.01f;
        public float mPerlinYScale = 0.01f;
        public int mPerlinXOffset = 0;
        public int mPerlinYOffset = 0;
        public int mPerlinOctaves = 3;
        public float mPerlinPersistance = 8;
        public float mPerlinHeightScale = 0.09f;
        public float mFreqMultiplier = 2;
        public bool remove = false;

    }
    public List<PerlinParameters> perlinParameters = new List<PerlinParameters>() {
        new PerlinParameters()
    };

    //veronoi floats for the editor
    public float voronoiFallOff = 0.2f;
    public float voronoiDropOff = 0.6f;
    public float voronoiMaxHeight = 0.5f;
    public float voronoiMinHeight = 0.1f;
    public int voronoiPeaks = 5;
    public enum VoronoiType { Linear = 0, Power =1 , Combined = 2, SinPow =3};
    public VoronoiType voronoiType = VoronoiType.Linear;


    //midpoint displacement for editor
    public float MPDheightMin = -2f; //height limits for the random.range noise
    public float MPDheightMax = 2f;//height limits for the random.range noise 
    public float MPDheightDampenerPower = 2.0f; //reduces the height each iteration
    public float MPDroughness = 2.0f; // sets how jagged/smooth terrain is

    //smooth for the editor
    public int smoothAmount = 1;

    //SPLAT MAPS class
    [System.Serializable]
    public class SplatHeights {
        public Texture2D texture = null;
        public float minHeight = 0.1f;
        public float maxHeight = 0.2f;
        public float minSlope = 0f;
        public float maxSlope = 1.42f; //sqrt of 2
        public Vector2 tileOffset = new Vector2(0, 0);
        public Vector2 tileSize = new Vector2(50, 50);
        public float textureXScale = 0.01f;
        public float textureYScale = 0.01f;
        public float textureScale = 0.3f;
        public float textureOffset = 0.02f;
        public bool remove = false;
    }
    //constructor 
    public List<SplatHeights> splatHeights = new List<SplatHeights>() {
        new SplatHeights()
    };

    //VEGETATION editor values and CLASS
    public int maxTrees = 5000;
    public int treeSpacing = 10;
    [System.Serializable]
    public class Vegetation {
        public GameObject mesh;
        public float minHeight = 0.1f;
        public float maxHeight = 0.2f;
        public float minSlope = 0;
        public float maxSlope = 90;
        public float minScale = 0.5f;
        public float maxScale = 2f;
        public float minRotation= 0;
        public float maxRotation = 360;
        public float density = 0.5f; //control density individually
        public Color colour1 = Color.white;
        public Color colour2 = Color.white;
        public Color lightColor = Color.white;
        public bool remove = false;
    }
    public List<Vegetation> vegetation = new List<Vegetation>() {
        new Vegetation()

    };
    //details editor values and CLASS
    [System.Serializable]
    public class Detail {
        public GameObject prototype = null;
        public Texture2D prototypeTexture = null;
        public float minHeight = 0.1f;
        public float maxHeight = 0.2f;
        public float minSlope = 0;
        public float maxSlope = 1;
        public Color dryColour = Color.white;
        public Color healthyColour = Color.white;
        public Vector2 heightRange = new Vector2(1, 1);
        public Vector2 widthRange = new Vector2(1, 1);
        public float noiseSpread = 0.5f;
        public float overlap = 0.01f;
        public float feather = 0.05f;
        public float density = 0.5f;
        public bool remove = false;
    }
    //Erosion class
    public enum ErosionType {  Rain = 0, Thermal = 1, Tidal = 2, River =3, Wind =4,
                               Canyon = 5}
    public ErosionType erosionType = ErosionType.Rain;
    public float erosionStrength = 0.15f;
    public int springsPerDroplet = 5;
    public float solubility = 0.01f;
    public int droplets = 10;
    public int erosionSmoothAmount = 5;
    public float thermalStrength = 0.08f;
    public float erosionAmount = 0.01f;

   


    public List<Detail> details = new List<Detail>() {
        new Detail()
    };
    public int maxDetails = 5000;
    public int detailSpacing = 5;

    //-----------------------------------------------------------------------------------===========================
    float [,] GetHeightMap() {

        if (resetTerrain) {
            return new float[terrainData.heightmapWidth, terrainData.heightmapHeight];
        } else {
            return terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight);
        }
    }
    //erosion

     public void Erode() {

        smoothAmount = erosionSmoothAmount;
        Smooth();
        switch (erosionType) {
            case (ErosionType.Rain):
                Rain();
                break;
            case (ErosionType.Tidal):
                Tidal();
                break;
            case (ErosionType.Thermal):
                Thermal();
                break;
            case (ErosionType.River):
                River();
                break;
            case (ErosionType.Wind):
                Wind();
                break;
            case (ErosionType.Canyon):
                Debug.Log("canyon called");
                Canyon();
                break;
            default:
                break;
            //smooth after eroding as it leaves sharp edges
            
        }
        
        

    }

    void Rain() {
        // for any random point on the map, x droplets fall and randomly decrease the height.
        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapWidth,
           terrainData.heightmapHeight); //we dont want to reset by accident

       for(int i =0; i < droplets; i++) {
            heightMap[UnityEngine.Random.Range(0, terrainData.heightmapWidth),
                UnityEngine.Random.Range(0, terrainData.heightmapHeight)]
                -= erosionStrength;
            }
        terrainData.SetHeights(0, 0, heightMap);
    }
    void Tidal() {
        //do this after shoreline tut done
            }

    //gradient descent moving sediment down the river 
    void River() {
        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapWidth,
       terrainData.heightmapHeight); //we dont want to reset by accident
        //keeps track of river placement
        float[,] erosionMap = new float[terrainData.heightmapWidth,
            terrainData.heightmapHeight];
        for (int i = 0; i < droplets; i++) {
            Vector2 dropletPosition = new Vector2(UnityEngine.Random.Range(0, terrainData.heightmapWidth),
                              UnityEngine.Random.Range(0, terrainData.heightmapHeight));
            erosionMap[(int)dropletPosition.x, (int)dropletPosition.y] = erosionStrength;
            for (int j = 0; j < springsPerDroplet; j++) {
                //process movement of river down the terrain
                erosionMap = RunRiver(dropletPosition, heightMap, erosionMap,
                    terrainData.heightmapWidth, terrainData.heightmapHeight);
            }
        }

        for (int y = 0; y < terrainData.heightmapHeight; y++) {
            for (int x = 0; x < terrainData.heightmapWidth; x++) {
              if(erosionMap[x,y] > 0) {
                    heightMap[x, y] -= erosionMap[x, y];
              }
            }
         }
        terrainData.SetHeights(0, 0, heightMap);
    }

    // calculates where the rivers go to on the map 
    public float[,] RunRiver(Vector3 dropletPosition, float[,] heightMap,
        float[,] erosionMap, int terrainWidth, int terrainHeight) {


         while(erosionMap[(int)dropletPosition.x, (int)dropletPosition.y] > 0) { 
        List<Vector2> neighbours = Utils.GenerateNeighbours(dropletPosition, terrainWidth,
            terrainHeight);
        neighbours.Shuffle(); //picks a random neighbour
        bool foundLower = false;
        foreach (Vector2 n in neighbours) {
            if (heightMap[(int)n.x, (int)n.y] < heightMap[(int)dropletPosition.x,
                (int)dropletPosition.y]) {
                erosionMap[(int)n.x, (int)n.y] = erosionMap[(int)dropletPosition.x,
                (int)dropletPosition.y] - solubility;
                dropletPosition = n;
                foundLower = true;
                break;
            }
        }
        if (!foundLower) {
            erosionMap[(int)dropletPosition.x, (int)dropletPosition.y] -= solubility;
        }
         }
        return erosionMap;
    }


    //landslides caused by temperature change, causing cliffs to be steeper and hills
    //to become smaller, dirt falls down the side
    //shifting all height maps value down based on steepness
    void Thermal() {
       float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapWidth,
       terrainData.heightmapHeight); //we dont want to reset by accident

        for (int y = 0; y < terrainData.heightmapHeight;y++) {
           for (int x = 0; x < terrainData.heightmapWidth; x++) {
                Vector2 thisLocation = new Vector2(x, y);
                List<Vector2> neighbours = Utils.GenerateNeighbours(thisLocation,
                    terrainData.heightmapWidth, terrainData.heightmapHeight);
                // if neighbour is lower than current position, move some sediment down
                foreach (Vector2 n in neighbours){
                    if (heightMap[x, y] > heightMap[(int)n.x, (int)n.y] + erosionStrength) {
                        
                        float currentHeight = heightMap[x, y];
                        heightMap[x, y] -= currentHeight * thermalStrength;
                        heightMap[(int)n.x, (int)n.y] += currentHeight * thermalStrength;
                    }
                }
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    //suspention
    //saltation,
    //creep
    //has to be used on terrains higher than 0 height as it 'digs' and moves height
    void Wind() {
        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapWidth,
                     terrainData.heightmapHeight); //we dont want to reset by accident
        int width = terrainData.heightmapWidth;
        int height = terrainData.heightmapHeight;
        //wind direction
        float WindDir = 30f; //degrees. 0 creates horizontal lines across x axis
        float sinAngle = -Mathf.Sin(Mathf.Deg2Rad * WindDir);
        float cosAngle = Mathf.Cos(Mathf.Deg2Rad * WindDir);
        //incrementing y by 10 here and not 1. must be greater than the 5 added
        //in ny. needs to cover whole range of values as rotation is applied
        for (int y = -(height -1)*2; y <= height*2; y+=10) {
            for(int x = -(width -1)*2; x<= width*2; x++) {
                //adds perlin noise to create ripple effect instead of straight lines
                float pNoise = (float)Mathf.PerlinNoise(x * 0.06f, y * 0.06f)
                                * 20 * erosionStrength;
                int nx = (int)x + (int)pNoise;
                int digy = (int)y + (int)pNoise; //create wave in y 
                int ny = (int)y + 5 + (int) pNoise;

                Vector2 digCoords = new Vector2(x * cosAngle - digy * sinAngle,
                    digy * cosAngle + x * sinAngle);
                Vector2 pileCoords = new Vector2(nx * cosAngle - ny * sinAngle,
                  ny * cosAngle + nx * sinAngle);

                //check if pile coords are within heightmap and if dig coords are
                //if so move from dig to pile
                if (!(pileCoords.x < 0 || pileCoords.x > (width -1) || pileCoords.y < 0 ||
                    pileCoords.y > (height - 1) || (int) digCoords.x < 0 ||
                    (int)digCoords.x > (width -1) || (int)digCoords.y < 0 ||
                    (int) digCoords.y > (height-1))){
                    heightMap[(int)digCoords.x, (int)digCoords.y] -= 0.001f;
                    heightMap[(int)pileCoords.x, (int)pileCoords.y] += 0.001f;
                }
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    //digs terrain from one side of the terrain to the other 
    //(if rand.range of x lower bound is positive)
    //, moving randomly in the opposite axis. eg makes river across x and varies y 
    // !! if erosion strength is too high ( compared to map height) this wont erode a canyon at all.
    float[,] canyonheightMap;

    void Canyon() {

        float digDepth = erosionStrength; // dont make this too high?? 
        float bankSlope = erosionAmount;
        float maxDepth = 0;
        canyonheightMap = terrainData.GetHeights(0, 0, terrainData.heightmapWidth,
                                                terrainData.heightmapHeight);
        int cx = 1;
        int cy = UnityEngine.Random.Range(10, terrainData.heightmapHeight - 10);
        while (cy >= 0 && cy < terrainData.heightmapHeight && cx > 0
               && cx < terrainData.heightmapWidth) {
            CanyonCrawler(cx, cy, canyonheightMap[cx, cy] - digDepth, bankSlope, maxDepth);
            //control straightness of river / amount it moves on y axis
            cx = cx + UnityEngine.Random.Range(-1, 4);
            cy = cy + UnityEngine.Random.Range(-3, 4);
        }
        Debug.Log("canyon 1 called");
        terrainData.SetHeights(0, 0, canyonheightMap);
    }

    //recursive algorithm
    void CanyonCrawler(int x, int y, float height, float slope, float maxDepth) {
        if (x < 0 || x >= terrainData.heightmapWidth) return; //off x range of map
        if (y < 0 || y >= terrainData.heightmapHeight) return; //off y range of map
        if (height <= maxDepth) return; //if hit lowest level
        if (canyonheightMap[x, y] <= height) return; //if run into lower elevation

        canyonheightMap[x, y] = height;

        CanyonCrawler(x + 1, y, height + UnityEngine.Random.Range(slope, slope + 0.01f), slope, maxDepth);
        CanyonCrawler(x - 1, y, height + UnityEngine.Random.Range(slope, slope + 0.01f), slope, maxDepth);
        CanyonCrawler(x + 1, y + 1, height + UnityEngine.Random.Range(slope, slope + 0.01f), slope, maxDepth);
        CanyonCrawler(x - 1, y + 1, height + UnityEngine.Random.Range(slope, slope + 0.01f), slope, maxDepth);
        CanyonCrawler(x, y - 1, height + UnityEngine.Random.Range(slope, slope + 0.01f), slope, maxDepth);
        CanyonCrawler(x, y + 1, height + UnityEngine.Random.Range(slope, slope + 0.01f), slope, maxDepth);
    }

    //can change more properties here in code if need to later
    public void SpawnDetails() {
        DetailPrototype[] newDetailPrototypes;
        newDetailPrototypes = new DetailPrototype[details.Count];
        int dIndex = 0;
        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapWidth,
                                            terrainData.heightmapHeight);

        foreach (Detail d in details) {
            newDetailPrototypes[dIndex] = new DetailPrototype();
            newDetailPrototypes[dIndex].prototype = d.prototype;
            newDetailPrototypes[dIndex].prototypeTexture = d.prototypeTexture;
            newDetailPrototypes[dIndex].healthyColor = d.healthyColour;
            newDetailPrototypes[dIndex].dryColor = d.dryColour;
            newDetailPrototypes[dIndex].minHeight = d.heightRange.x;
            newDetailPrototypes[dIndex].maxHeight = d.heightRange.y;
            newDetailPrototypes[dIndex].minWidth = d.widthRange.x;
            newDetailPrototypes[dIndex].maxWidth = d.widthRange.y;
            newDetailPrototypes[dIndex].noiseSpread = d.noiseSpread;

            if (newDetailPrototypes[dIndex].prototype) {
                newDetailPrototypes[dIndex].usePrototypeMesh = true;
                newDetailPrototypes[dIndex].renderMode = DetailRenderMode.VertexLit;
            }
            else {
                newDetailPrototypes[dIndex].usePrototypeMesh = false;
                newDetailPrototypes[dIndex].renderMode = DetailRenderMode.GrassBillboard;
            }
            dIndex++;
        }
        terrainData.detailPrototypes = newDetailPrototypes;

        for (int i = 0; i < terrainData.detailPrototypes.Length; ++i) {
            int[,] detailMap = new int[terrainData.detailWidth, terrainData.detailHeight];
            for (int y = 0; y < terrainData.detailHeight; y += detailSpacing) {
                for (int x = 0; x < terrainData.detailWidth; x += detailSpacing) {
                    if (UnityEngine.Random.Range(0.0f, 1.0f) > details[i].density) continue;

                    int xHM = (int)(x / (float)terrainData.detailWidth * terrainData.heightmapWidth);
                    int yHM = (int)(y / (float)terrainData.detailHeight * terrainData.heightmapHeight);

                    float thisNoise = Utils.Map(Mathf.PerlinNoise(x * details[i].feather,
                                                y * details[i].feather), 0, 1, 0.5f, 1);
                    float thisHeightStart = details[i].minHeight * thisNoise -
                                            details[i].overlap * thisNoise;
                    float nextHeightStart = details[i].maxHeight * thisNoise +
                                            details[i].overlap * thisNoise;

                    float thisHeight = heightMap[yHM, xHM];
                    float steepness = terrainData.GetSteepness(xHM / (float)terrainData.size.x,
                                                                yHM / (float)terrainData.size.z);
                    if ((thisHeight >= thisHeightStart && thisHeight <= nextHeightStart) &&
                        (steepness >= details[i].minSlope && steepness <= details[i].maxSlope)) {
                        detailMap[y, x] = 1;
                    }
                }
            }
            terrainData.SetDetailLayer(0, 0, i, detailMap);
        }
    }

    public void AddDetail() {
        details.Add(new Detail());
    }
    public void RemoveDetail() {
        List<Detail> keptDetail = new List<Detail>();
        for (int i = 0; i < details.Count; i++) {
            if (!details[i].remove) {
                keptDetail.Add(details[i]);
            }
        }
        if (keptDetail.Count == 0) { //we dont want to keep any 
            keptDetail.Add(details[0]); //tables list cant be empty so add an element
        }
        details = keptDetail;
    }


    public void PlantVegetation() {
        //links up the tree meshes automatically from UI into the terrain
        TreePrototype[] newTreePrototypes;
        newTreePrototypes = new TreePrototype[vegetation.Count];
        int i = 0;
        foreach(Vegetation t in vegetation) {
            newTreePrototypes[i] = new TreePrototype();
            newTreePrototypes[i].prefab = t.mesh;
            i++;
        }
        terrainData.treePrototypes = newTreePrototypes;
        //instantiates tree instances, have to set all of its properties 
        List<TreeInstance> allVegetation = new List<TreeInstance>();
        float sizeX = terrainData.size.x;
        float sizeZ = terrainData.size.z;
        //loop around world coordinates, y is height and z is depth
        for (int z =0; z < sizeZ; z += treeSpacing) {
            for(int x=0; x < sizeX; x+= treeSpacing) {
                for (int tp = 0; tp < terrainData.treePrototypes.Length; tp++) {
                    //density check code
                    if (UnityEngine.Random.Range(0, 1f) > vegetation[tp].density) continue;
                    float thisHeight = terrainData.GetHeight(x, z) / terrainData.size.y;  //gets height between 0 and 1   
                    //gets height ranges from editor
                    float thisHeightStart = vegetation[tp].minHeight;
                    float thisHeightEnd = vegetation[tp].maxHeight;
                  
                    float steepness = terrainData.GetSteepness (x / (float)sizeX, z / (float)sizeZ );
                    float minSteepness = vegetation[tp].minSlope;
                    float maxSteepness = vegetation[tp].maxSlope;
                    //debugging
                    if (z % 10 == 1) Debug.Log("terrain steepness: " + steepness + " Tree placement: " + z + "," + x);
                    //  if height is within tree height and gradient spawning range, spawn a tree
                    if ((thisHeight >= thisHeightStart && thisHeight <= thisHeightEnd) && 
                        (steepness >= minSteepness && steepness <=  maxSteepness)) {
                        TreeInstance instance = new TreeInstance();
                        //puts tree on grid at a random slight offset, if instance pos within terrain mesh

                        instance.position = new Vector3((x + UnityEngine.Random.Range(-5f, 5f))/sizeX,
                                                    terrainData.GetHeight(x,z)/terrainData.size.y,
                                              (z + UnityEngine.Random.Range(-5f, 5f)) / sizeZ);
                        //raycast down from trees to terrain so trees are put onto the ground, and up so trees are moved up
                        //if inside the terrain
                        Vector3 treeWorldPos = new Vector3(instance.position.x * terrainData.size.x,
                             instance.position.y * terrainData.size.y,
                             instance.position.z * terrainData.size.z)
                                                          + this.transform.position;
                        RaycastHit hit;
                        int layerMask = 1 << terrainLayer;
                        //adding a small amount as raycast will miss trees placed on a height of 0 otherwise
                        if (Physics.Raycast(treeWorldPos + new Vector3(0, 10, 0), -Vector3.up, out hit, 100, layerMask) ||
                             Physics.Raycast(treeWorldPos - new Vector3(0, 10, 0), Vector3.up, out hit, 100, layerMask)) {
                            float treeHeight = (hit.point.y - this.transform.position.y) / terrainData.size.y;
                            instance.position = new Vector3(instance.position.x,
                                                             treeHeight,
                                                             instance.position.z);

                            instance.rotation = UnityEngine.Random.Range(vegetation[tp].minRotation,
                                                                         vegetation[tp].maxRotation);
                            instance.prototypeIndex = tp;
                            instance.color = Color.Lerp(vegetation[tp].colour1,
                                                        vegetation[tp].colour2,
                                                        UnityEngine.Random.Range(0.0f, 1.0f));
                            instance.lightmapColor = vegetation[tp].lightColor;
                            //keeping aspect ratio same
                            float s = UnityEngine.Random.Range(vegetation[tp].minScale, vegetation[tp].maxScale);
                            instance.heightScale = s;
                            instance.widthScale = s;

                            allVegetation.Add(instance);
                            if (allVegetation.Count >= maxTrees) goto TREESDONE;
                        }
                        //else tree is off terrain so dont add.
                    }
                }
            }
        }
        TREESDONE:
        Debug.Log("Trees printed");
        terrainData.treeInstances = allVegetation.ToArray();
    }
 

    public void AddVegetation() {
        vegetation.Add(new Vegetation());
    }
    public void RemoveVegetation() {
        List<Vegetation> keptVeges = new List<Vegetation>();
        for (int i = 0; i < vegetation.Count; i++) {
            if (!vegetation[i].remove) {
                keptVeges.Add(vegetation[i]);
            }
        }
        if (keptVeges.Count == 0) { //we dont want to keep any 
            keptVeges.Add(vegetation[0]); //tables list cant be empty so add an element
        }
        vegetation = keptVeges;
    }

    // TODO refactor as an interface as this is practically the same code as the perlin remove/add
    public void AddSplatHeight() {
        splatHeights.Add(new SplatHeights());
    }
    //removes row of splat function (removes from list)
    public void RemoveSplatHeight() {
        List<SplatHeights> keptSplatHeights = new List<SplatHeights>();
        for (int i = 0; i < splatHeights.Count; i++) {
            if (!splatHeights[i].remove) {
                keptSplatHeights.Add(splatHeights[i]);
            }
        }
        if (keptSplatHeights.Count == 0) { //we dont want to keep any 
            keptSplatHeights.Add(splatHeights[0]); //tables list cant be empty so add an element
        }
        splatHeights = keptSplatHeights;
    }

    //divides all vectors by length of vector array so they sum to 1
    void NormalizeVector(float [] v) {
        float total = 0;
        for( int i = 0; i < v.Length; i++) {
            total += v[i];
        }
        for(int i =0; i< v.Length; i++) {
            v[i] /= total;
        }
    }

    //gets steepness of current part of the height map, to be able to test if its a cliff/mountain vs flat
    //heighest returned value sqrt(2), least is 0
    //todo: maybe delete unless need this, unity has its own implementation
    float GetSteepness( float [,] heightmap, int x, int y, int width, int height) {
        float h = heightmap[x, y];
        int nx = x + 1;
        int ny = y + 1;
        //sobel algorithm simplfied
        if (nx > width - 1) {
            nx = x - 1;
        }
        if (ny > height - 1) {
            ny = y - 1;
        }
        float dx = heightmap[nx, y] - h;
        float dy = heightmap[x, ny] - h;
        Vector2 gradient = new Vector2(dx, dy);
        return gradient.magnitude;
        
    }
    public void SplatMaps() {
        SplatPrototype[] newSplatPrototypes;
        newSplatPrototypes = new SplatPrototype[splatHeights.Count];
        int spindex = 0;
        //coppy textures details from splatprototypes 
        foreach(SplatHeights h in splatHeights) {
            newSplatPrototypes[spindex] = new SplatPrototype();
            newSplatPrototypes[spindex].texture = h.texture;
            newSplatPrototypes[spindex].tileOffset = h.tileOffset;
            newSplatPrototypes[spindex].tileSize = h.tileSize;
            newSplatPrototypes[spindex].texture.Apply(true);
            spindex++;
        }
        terrainData.splatPrototypes = newSplatPrototypes;

        //set terrain textures based off of the height map
        float[,] heightMap = terrain.terrainData.GetHeights(0, 0, terrainData.heightmapWidth,
                                                    terrainData.heightmapHeight);
        float[,,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight,
                                            terrainData.alphamapLayers];
        //goes through alpha map and work out how much of each texture should exist
        for(int y = 0; y < terrainData.alphamapHeight; y++) {
            for(int x =0; x< terrainData.alphamapWidth; x++) {              
               
                //stores texture blend values for each x,y in terrain as 1 if exists or 0 if not
                float[] splat = new float[terrainData.alphamapLayers];
                for (int i = 0; i < splatHeights.Count; i++) {
                    //create noise to make textures bands look more realistic (scaled to get better results)
                    float noise = Mathf.PerlinNoise(x * splatHeights[i].textureXScale, y * 
                                                splatHeights[i].textureYScale) * splatHeights[i].textureScale;
                    float offset = splatHeights[i].textureOffset + noise; // allows texture blending by forcing overlap
                    float thisHeightStart = splatHeights[i].minHeight -offset;
                    float thisHeightStop = splatHeights[i].maxHeight + offset;
                    //using unity terrain inbuilt steepness, returns number 0 to 90 for degrees? 
                    //alpha map is 90 degrees different to height map so we swap x and y values
                    float steepness = terrainData.GetSteepness(y / (float)terrainData.alphamapHeight,
                                    x / (float)terrainData.alphamapWidth);
                    //set texture if within range and test if steepness above threshold
                    if ((heightMap[x, y] >= thisHeightStart && heightMap[x, y] <= thisHeightStop) &&
                        (steepness >= splatHeights[i].minSlope && steepness <= splatHeights[i].maxSlope)) { 
                        splat[i] = 1;
                    }
                }
                //values in splat must add up to 1 as its a splat map
                NormalizeVector(splat);
                for (int j = 0; j < splatHeights.Count; j++) {
                    splatmapData[x, y, j] = splat[j];
                }
            }
            

        }
        terrainData.SetAlphamaps(0, 0, splatmapData);
    }

   

    //smoothing function, ignoring values at edge of map as they have no adj neighbours
    //based on the blurring technique found in photoshop etc
    //this function takes a while to run, and so am adding a progress bar to appear in the editor to
    //update progress of for loop for clarity that the function is running and hasnt hung
    public void Smooth() {

        //get heights manually from the data so we dont need to worry if the reset box is accidently ticked
        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapWidth,
            terrainData.heightmapHeight);
        int width = terrainData.heightmapWidth;
        int height = terrainData.heightmapHeight;
        float smoothProgress = 0f;
        EditorUtility.DisplayProgressBar("Smoothing Terrain", "Progress", smoothProgress);

        // runs code x amount of times depending on smooth amount
        for (int i = 0; i < smoothAmount; i++) {
            //smooth code
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    float totalHeight = heightMap[x, y];
                    List<Vector2> neighbours = Utils.GenerateNeighbours(new Vector2(x, y), terrainData.heightmapWidth,
                                                                   terrainData.heightmapHeight);
                    foreach (Vector2 n in neighbours) {
                        totalHeight += heightMap[(int)n.x, (int)n.y];
                    }
                    //adding one for the initial totalHeight as its not included in its neighbours
                    heightMap[x, y] = totalHeight / ((float)neighbours.Count + 1);
                }
            }
            smoothProgress++;
            EditorUtility.DisplayProgressBar("Smoothing Terrain", "Progress", smoothProgress/smoothAmount);
        }
        
        terrainData.SetHeights(0, 0, heightMap);
        EditorUtility.ClearProgressBar();
    }

    //iteratve function
    public void MidpointDisplacement() {

        float[,] heightMap = GetHeightMap();
        int width = terrainData.heightmapWidth - 1;
        int squareSize = width; //current squaresize for algorithm
        float heightMin = MPDheightMin; 
        float heightMax = MPDheightMax;        
        float heightDampener = (float)Mathf.Pow(MPDheightDampenerPower, -1* MPDroughness);

        //where each vertex is relative to each edge
        int cornerX, cornerY; // top right corner of square coords
        int midX, midY; //midpoint of squares coords
        int pmidXL, pmidXR, pmidYU, pmidYD; //used in square-step part, thse are the midpoints of the adjacent squares to the current

        //setting corners of terrain to random heights, terrain goes up to 512 not 513 as it has to be square
        /*heightMap[0, 0] = UnityEngine.Random.Range(0f, 0.2f);
        heightMap[0, terrainData.heightmapHeight -2] = UnityEngine.Random.Range(0f, 0.2f);
        heightMap[terrainData.heightmapWidth-2, 0] = UnityEngine.Random.Range(0f, 0.2f);
        heightMap[terrainData.heightmapWidth - 2, terrainData.heightmapHeight - 2] =
                                                                    UnityEngine.Random.Range(0f, 0.2f); */

        
        while (squareSize > 0) {
            //diamond step
            for (int x = 0; x < width; x += squareSize) {
                for (int y = 0; y < width; y += squareSize) {
                    cornerX = (x + squareSize);
                    cornerY = (y + squareSize);

                    midX = (int)(x + squareSize / 2.0f);
                    midY = (int)(y + squareSize / 2.0f);
                    heightMap[midX, midY] = (float)(heightMap[x, y] + heightMap[cornerX, y]
                                            + heightMap[x, cornerY] + heightMap[cornerX, cornerY]) / 4.0f
                                            + UnityEngine.Random.Range(heightMin,heightMax);
                }
            }
            //square step
            for(int x = 0; x< width; x+= squareSize) {
                for (int y = 0; y < width; y+= squareSize) {

                    //update coords for the squares
                    cornerX = (x + squareSize);
                    cornerY = (y + squareSize);
                    midX = (int)(x + squareSize / 2.0f);
                    midY = (int)(y + squareSize / 2.0f);

                    pmidXR = (int)(midX + squareSize);
                    pmidYU = (int)(midY + squareSize);
                    pmidXL = (int)(midX - squareSize);
                    pmidYD = (int)(midY - squareSize);

                    if( pmidXL <=0 || pmidYD <= 0 || pmidXR >= width -1 || pmidYU >= width-1) {
                        continue;
                    }
                    //calculate square value for bottom side
                    heightMap[midX, y] = (float)((heightMap[midX, midY] + heightMap[x, y]
                                         + heightMap[midX, pmidYD] + heightMap[cornerX, y]) / 4.0f
                                         + UnityEngine.Random.Range(heightMin, heightMax));
                    //calc square value for topside
                    heightMap[midX, cornerY] = (float)((heightMap[x, cornerY] + heightMap[midX, midY]
                                         + heightMap[cornerX, cornerY] + heightMap[midX, pmidYU ]) / 4.0f
                                         + UnityEngine.Random.Range(heightMin, heightMax));
                    //calc square value for left side
                    heightMap[x, midY] = (float)((heightMap[x, y] + heightMap[pmidXL, midY]
                                         + heightMap[x, cornerY] + heightMap[midX, midY]) / 4.0f
                                         + UnityEngine.Random.Range(heightMin, heightMax));
                    //calc square value for rightside
                    heightMap[cornerX, midY] = (float)((heightMap[midX, y] + heightMap[midX, midY]
                                        + heightMap[cornerX, cornerY] + heightMap[pmidXR, midY]) / 4.0f
                                        + UnityEngine.Random.Range(heightMin, heightMax));
                }
            }
            //each iteration reduces squaresize and height
            squareSize = (int)(squareSize /2.0f);
            heightMin *= heightDampener;
            heightMax *= heightDampener;
        }
        terrainData.SetHeights(0, 0, heightMap);
    }
    public void Voronoi() {
        float[,] heightMap = GetHeightMap();
        //sets height of the peaks
        //generate number of peak locations
        for (int p = 0; p < voronoiPeaks; p++) {
            Vector3 peak = new Vector3(UnityEngine.Random.Range(0, terrainData.heightmapWidth),
                                        UnityEngine.Random.Range(voronoiMinHeight, voronoiMaxHeight), //height value
                                        UnityEngine.Random.Range(0, terrainData.heightmapHeight)
                                        );
          //  heightMap[(int)peak.x, (int)peak.z] = peak.y;

            //check if new peak is greater than existing terrain elevation
            if (heightMap[(int)peak.x, (int)peak.z] < peak.y) {
                heightMap[(int)peak.x, (int)peak.z] = peak.y;
            }
            else {
                continue; //skip following code
            }
            Vector2 peakLocation = new Vector2(peak.x, peak.z);
            //max distance possible is the diagonal of the plane
            float maxDistance = Vector2.Distance(new Vector2(0, 0), new Vector2(terrainData.heightmapWidth,
                                                                                terrainData.heightmapHeight));
            for (int y = 0; y < terrainData.heightmapHeight; y++) {
                for (int x = 0; x < terrainData.heightmapWidth; x++) {
                    //calculate the height of every point based on peak distance, scaled by heightmap 
                    if (!(x == peak.x && y == peak.z)) { //dont process the peak
                        float distanceToPeak = Vector2.Distance(peakLocation, new Vector2(x, y)) / maxDistance;

                        float h;
                        if (voronoiType == VoronoiType.Combined) {
                             h = peak.y - distanceToPeak * voronoiFallOff -
                                 Mathf.Pow(distanceToPeak, voronoiDropOff);
                        }
                        else if(voronoiType == VoronoiType.Power) {
                            h = peak.y - Mathf.Pow(distanceToPeak, voronoiDropOff) * voronoiFallOff;
                        }
                        else if (voronoiType == VoronoiType.SinPow) {
                            h = peak.y - Mathf.Pow(distanceToPeak * 3, voronoiFallOff) -
                                    Mathf.Sin(distanceToPeak * 2 * Mathf.PI) / voronoiDropOff; //sinpow
                        }
                        else { //linear
                            h = peak.y - distanceToPeak * voronoiFallOff;
                        }
                   
                        //we dont want to overwrite higher values with lower ones
                        if (heightMap[x, y] < h) {
                            heightMap[x, y] = h;
                        }

                    }
                }
            }
        }
        terrainData.SetHeights(0, 0, heightMap);

    }

    public void MultiplePerlinTerrain() {
        float[,] heightMap = GetHeightMap();
        for(int y =0; y< terrainData.heightmapHeight; y++) {
            for(int x =0; x <terrainData.heightmapWidth; x++) {
                foreach(PerlinParameters p in perlinParameters) {
                    heightMap[x, y] += Utils.fBM((x + p.mPerlinXOffset) * p.mPerlinXScale,
                                                (y + p.mPerlinYOffset) * p.mPerlinYScale,
                                            p.mPerlinOctaves, p.mPerlinPersistance, p.mFreqMultiplier)
                                            * p.mPerlinHeightScale;
                }
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    //add new row of perlin function (adds to list)
    public void AddNewPerlin() {
        perlinParameters.Add(new PerlinParameters());
    }
    //removes row of perlin function (removes from list)
    public void RemovePerlin() {
        List<PerlinParameters> keptPerlinParamers = new List<PerlinParameters>();
        for (int i = 0; i < perlinParameters.Count; i++) {
            if (!perlinParameters[i].remove) {
                keptPerlinParamers.Add(perlinParameters[i]);
            }
        }
        if(keptPerlinParamers.Count == 0) { //we dont want to keep any 
            keptPerlinParamers.Add(perlinParameters[0]); //tables list cant be empty so add an element
        }
        perlinParameters = keptPerlinParamers;        
    }

    public void Perlin() {
        float[,] heightmap = GetHeightMap();

        for (int y = 0; y< terrainData.heightmapWidth; y++) {
            for (int x = 0; x < terrainData.heightmapHeight; x++) {

                //where x is depth and y is width
                heightmap[x, y] += Utils.fBM((x+perlinXOffset) * perlinXScale, (y+perlinYOffset) * perlinYScale, 
                                            perlinOctaves, perlinPersistance, freqMultiplier) * perlinHeightScale;

            }
        }
        terrainData.SetHeights(0, 0, heightmap);
    }

    public void LoadTexture() {

        //gets existing hieghtmap data.
        float[,] heightmap;
            heightmap = GetHeightMap();
        for (int x = 0; x < terrainData.heightmapWidth; x++) {
                for (int y = 0; y < terrainData.heightmapHeight; y++) {

                //where x is depth and y is width
                heightmap[x, y] += heightMapImage.GetPixel((int)(x * heightMapScale.x),
                                    (int)(y * heightMapScale.z)).grayscale * heightMapScale.y;
                }
            }
            terrainData.SetHeights(0, 0, heightmap);
    }

    public void RandomTerrain() {
        //gets existing hieghtmap data.
        float[,] heightmap = GetHeightMap();
        heightmap = new float[terrainData.heightmapWidth, terrainData.heightmapHeight];
        for(int x = 0; x<terrainData.heightmapWidth; x ++) {
            for(int y =0; y < terrainData.heightmapHeight; y++) {

                //where x is depth and y is width
                heightmap[x, y] += UnityEngine.Random.Range(randomHeightRange.x, randomHeightRange.y);
            }
        }
        terrainData.SetHeights(0, 0, heightmap);
    }
    public void ResetTerrain() {
        //fills map with 0s
        float[,] heightmap;
        heightmap = new float[terrainData.heightmapWidth, terrainData.heightmapHeight];
        for (int x = 0; x < terrainData.heightmapWidth; x++) {
            for (int y = 0; y < terrainData.heightmapHeight; y++) {

                //where x is depth and y is width
                heightmap[x, y] = 0;
            }
        }
        terrainData.SetHeights(0, 0, heightmap);

    }
    private void OnEnable() {

        Debug.Log("Initialising Terrain Data");
        terrain = this.GetComponent<Terrain>();
        terrainData = terrain.terrainData;
    }

    public enum TagType { Tag =0, Layer =1}
    [SerializeField] int terrainLayer = -1;

    //remove component and readd it on inspector to add tags programmatically.
    void Awake () {
        
        //adds the terrain tags into editor

        SerializedObject tagManager = new SerializedObject(
                                        AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        AddTag(tagsProp, "Terrain", TagType.Tag);
        AddTag(tagsProp, "Cloud", TagType.Tag);
        AddTag(tagsProp, "Shore", TagType.Tag);
        tagManager.ApplyModifiedProperties();
        //applies tag changes to tag database
        SerializedProperty layerProp = tagManager.FindProperty("layers");
        terrainLayer = AddTag(layerProp, "Terrain", TagType.Layer);
        tagManager.ApplyModifiedProperties();
        //take the object tag
        this.gameObject.tag = "Terrain";
        this.gameObject.layer = terrainLayer;
    }
	
    int AddTag(SerializedProperty tagsProp, string newTag, TagType type) {
        bool found = false;
        //check tag doesnt already exist
        for (int i = 0; i < tagsProp.arraySize; i++) {
            SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
            if (t.stringValue.Equals(newTag)) {
                found = true;
                return i;
            }
        }
            //else add new tag
            if (!found && type ==TagType.Tag) {
                tagsProp.InsertArrayElementAtIndex(0);
                SerializedProperty newTagProp = tagsProp.GetArrayElementAtIndex(0);
                newTagProp.stringValue = newTag;
            }
        //add new layer
        else if(!found && type == TagType.Layer) {
            //looping from 8 as in unity user editable layers start from layer 8
            for(int j = 8; j<tagsProp.arraySize; j++) {
                SerializedProperty newLayer = tagsProp.GetArrayElementAtIndex(j);
                //add new layer in empty slot
                if(newLayer.stringValue == "") {
                    //Debug.Log("adding new layer :" + newTag);
                    newLayer.stringValue = newTag;
                    return j;
                }
            }
           
        }
        return -1;
    }
	// Update is called once per frame
	void Update () {
		
	}
}
