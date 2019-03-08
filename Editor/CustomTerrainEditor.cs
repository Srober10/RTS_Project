using UnityEngine;
using EditorGUITable;
using UnityEditor;

[CustomEditor(typeof(CustomTerrain))]
[CanEditMultipleObjects]

public class CustomTerrainEditor : Editor {

    //general properties -----------
    SerializedProperty randomHeightRange;
    SerializedProperty heightMapScale;
    SerializedProperty heightMapImage;

    //perlin properties
    SerializedProperty perlinXScale;
    SerializedProperty perlinYScale;
    SerializedProperty perlinXOffset;
    SerializedProperty perlinYOffset;
    SerializedProperty perlinOctaves;
    SerializedProperty perlinPersistance;
    SerializedProperty perlinHeightScale;
    SerializedProperty freqMultiplier;
    SerializedProperty resetTerrain;
    GUITableState perlinParameterTable; //GUI table library
    SerializedProperty perlinParameters;

    //voronoi properties
    SerializedProperty voronoiFallOff;
    SerializedProperty voronoiDropOff;
    SerializedProperty voronoiMinHeight;
    SerializedProperty voronoiMaxHeight;
    SerializedProperty voronoiPeaks;
    SerializedProperty voronoiType;
    //smooth properties
    SerializedProperty smoothAmount;
    //splatmaps properties
    GUITableState splatMapTable;
    SerializedProperty splatHeights;
    //tree- vegetations customisation values
    GUITableState vegetation;
    SerializedProperty maxTrees;
    SerializedProperty treeSpacing;
    //details properties
    // GUITableState detailMapTable;
    GUITableState detailMapTable;
    SerializedProperty detail;
    SerializedProperty maxDetails;
    SerializedProperty detailSpacing;
    //erosion
    SerializedProperty erosionStrength;
    SerializedProperty springsPerDroplet;
    SerializedProperty solubility;
    SerializedProperty droplets;
    SerializedProperty erosionSmoothAmount;
    SerializedProperty erosionType;
    SerializedProperty thermalStrength;



    //fold outs -----------
    bool showRandom = false;
    bool showLoadHeights = false;
    bool showPerlinNoise = false;
    bool showMultiplePerlin = false;
    bool showVoronoi = false;
    bool showMidpointDisplacement = false;
    bool showSmooth = false;
    bool showSplatMaps = false;
    bool showTrees = false;
    bool showDetail = false;
    bool showErosion = false;

    //allows update in editor (equivalent to awake for editor)
    private void OnEnable() {
        //links to other script to synchronize values
        randomHeightRange = serializedObject.FindProperty("randomHeightRange");
        heightMapScale = serializedObject.FindProperty("heightMapScale");
        heightMapImage = serializedObject.FindProperty("heightMapImage");
        //gets perlin
        perlinXScale = serializedObject.FindProperty("perlinXScale");
        perlinYScale = serializedObject.FindProperty("perlinYScale");
        perlinXOffset = serializedObject.FindProperty("perlinXOffset");
        perlinYOffset = serializedObject.FindProperty("perlinYOffset");
        perlinOctaves = serializedObject.FindProperty("perlinOctaves");
        perlinPersistance = serializedObject.FindProperty("perlinPersistance");
        perlinHeightScale = serializedObject.FindProperty("perlinHeightScale");
        freqMultiplier = serializedObject.FindProperty("freqMultiplier");
        resetTerrain = serializedObject.FindProperty("resetTerrain");
        perlinParameterTable = new GUITableState("perlinParameterTable");
        perlinParameters = serializedObject.FindProperty("perlinParameters");
        //gets voronoi
        voronoiDropOff = serializedObject.FindProperty("voronoiDropOff");
        voronoiFallOff = serializedObject.FindProperty("voronoiFallOff");
        voronoiMaxHeight = serializedObject.FindProperty("voronoiMaxHeight");
        voronoiMinHeight = serializedObject.FindProperty("voronoiMinHeight");
        voronoiPeaks = serializedObject.FindProperty("voronoiPeaks");
        voronoiType = serializedObject.FindProperty("voronoiType");
        //gets smooth amount
        smoothAmount = serializedObject.FindProperty("smoothAmount");
        //splat maps
        splatMapTable = new GUITableState("splatMapTable");
        splatHeights = serializedObject.FindProperty("splatHeights");
        //trees
        vegetation = new GUITableState("vegetation");
        maxTrees = serializedObject.FindProperty("maxTrees");
        treeSpacing = serializedObject.FindProperty("treeSpacing");
        //details
        detailMapTable = new GUITableState("detailMapTable");
        detail = serializedObject.FindProperty("details");
        maxDetails = serializedObject.FindProperty("maxDetails");
        detailSpacing = serializedObject.FindProperty("detailSpacing");
        //erosion
        erosionSmoothAmount = serializedObject.FindProperty("erosionSmoothAmount");
        erosionStrength = serializedObject.FindProperty("erosionStrength");
        springsPerDroplet = serializedObject.FindProperty("springsPerDroplet");
        solubility = serializedObject.FindProperty("solubility");
        droplets = serializedObject.FindProperty("droplets");
        erosionType = serializedObject.FindProperty("erosionType");
        thermalStrength = serializedObject.FindProperty("thermalStrength");



    }
    Vector2 scrollPos; //keeps track of scroll bars position
    public override void OnInspectorGUI() {
        serializedObject.Update();
        CustomTerrain terrain = (CustomTerrain)target;
        EditorGUILayout.PropertyField(resetTerrain);
        /* //scrollbar UI code start
        Rect r = EditorGUILayout.BeginVertical();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(r.width), GUILayout.Height(r.height));
        EditorGUI.indentLevel++; */

        //foldout terrain for random height
        showRandom = EditorGUILayout.Foldout(showRandom, "Random");
        if (showRandom) {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Set heights between random values", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(randomHeightRange);
            //on button click
            if (GUILayout.Button("Random heights")) {
                terrain.RandomTerrain();
            }
        }

        //foldout terrain editor for input image
        showPerlinNoise = EditorGUILayout.Foldout(showPerlinNoise, "Single Perlin Noise");
        if (showPerlinNoise) {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Perlin noisee", EditorStyles.boldLabel);
            EditorGUILayout.Slider(perlinXScale, 0, 0.1f, new GUIContent("X scale"));
            EditorGUILayout.Slider(perlinYScale, 0, 0.1f, new GUIContent("Y scale"));
            EditorGUILayout.IntSlider(perlinXOffset, 0, 10000, new GUIContent("X noise"));
            EditorGUILayout.IntSlider(perlinYOffset, 0, 10000, new GUIContent("Y noise"));
            EditorGUILayout.IntSlider(perlinOctaves, 1, 10, new GUIContent("Octaves"));
            EditorGUILayout.Slider(perlinPersistance, 0, 10f, new GUIContent("Persistance"));
            EditorGUILayout.Slider(perlinHeightScale, 0, 1f, new GUIContent("Height scale"));
            EditorGUILayout.Slider(freqMultiplier, 0, 5f, new GUIContent("frequency Multiplier"));
            //on button click
            if (GUILayout.Button("Perlin")) {
                terrain.Perlin();
            }
        }

        showMultiplePerlin = EditorGUILayout.Foldout(showMultiplePerlin, "Multiple Perlin Noise");
        if (showMultiplePerlin) {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Multiple Perlin Noise", EditorStyles.boldLabel);
            perlinParameterTable = GUITableLayout.DrawTable(perlinParameterTable, perlinParameters);
            GUILayout.Space(20); //formatting space so buttons dont appear on slider
            //this makes the plus and minus button next to each other horizontally on the gui
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+")) {
                terrain.AddNewPerlin();
            }
            if (GUILayout.Button("-")) {
                terrain.RemovePerlin();
            }

            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Apply Multiple Perlin")) {
                terrain.MultiplePerlinTerrain();
            }

        }
        showLoadHeights = EditorGUILayout.Foldout(showLoadHeights, "Load Height Map in");
        if (showLoadHeights) {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Set heights from input image", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(heightMapImage);
            EditorGUILayout.PropertyField(heightMapScale);
            //on button click
            if (GUILayout.Button("Load Texture")) {
                terrain.LoadTexture();
            }
        }
        showVoronoi = EditorGUILayout.Foldout(showVoronoi, "Voronoi");
        if (showVoronoi) {

            EditorGUILayout.IntSlider(voronoiPeaks, 1, 10, new GUIContent("Peak count"));
            EditorGUILayout.Slider(voronoiFallOff, 0, 10, new GUIContent("FallOff"));
            EditorGUILayout.Slider(voronoiDropOff, 0, 10, new GUIContent("Dropoff"));
            EditorGUILayout.Slider(voronoiMaxHeight, 0, 1, new GUIContent("Max Height"));
            EditorGUILayout.Slider(voronoiMinHeight, 0, 1, new GUIContent("Min Height"));
            EditorGUILayout.PropertyField(voronoiType);
            if (GUILayout.Button("Generate peaks")) {
                terrain.Voronoi();
            }
        }

        showMidpointDisplacement = EditorGUILayout.Foldout(showMidpointDisplacement, "Midpoint Displacement");
        if (showMidpointDisplacement) {
            if (GUILayout.Button("MP displacement")) {
                terrain.MidpointDisplacement();
            }
        }
        showSmooth = EditorGUILayout.Foldout(showSmooth, "Smooth");
        if (showSmooth) {
            EditorGUILayout.IntSlider(smoothAmount, 1, 10, new GUIContent("Smooth iterations"));
            if (GUILayout.Button("Smooth")) {
                terrain.Smooth();
            }
        }

        showSplatMaps = EditorGUILayout.Foldout(showSplatMaps, "Splat Maps");
        if (showSplatMaps) {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Splat Maps", EditorStyles.boldLabel);
            splatMapTable = GUITableLayout.DrawTable(splatMapTable, serializedObject.FindProperty("splatHeights"));
            GUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+")) {
                terrain.AddSplatHeight();
            }
            if (GUILayout.Button("-")) {
                terrain.RemoveSplatHeight();
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Apply Splatmaps")) {
                terrain.SplatMaps();
            }



        }
        showTrees = EditorGUILayout.Foldout(showTrees, "show Trees");
        if (showTrees) {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("show Trees", EditorStyles.boldLabel);
            EditorGUILayout.IntSlider(maxTrees, 0, 10000, new GUIContent("Max Trees"));
            EditorGUILayout.IntSlider(treeSpacing, 2, 20, new GUIContent("Tree Spacing"));
            vegetation = GUITableLayout.DrawTable(vegetation, serializedObject.FindProperty("vegetation"));
            GUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+")) {
                terrain.AddVegetation();
            }
            if (GUILayout.Button("-")) {
                terrain.RemoveVegetation();
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Apply trees")) {
                terrain.PlantVegetation();
            }
         
        }

        showDetail = EditorGUILayout.Foldout(showDetail, "Detail");
        if (showDetail) {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Detail", EditorStyles.boldLabel);
            EditorGUILayout.IntSlider(maxDetails, 0, 10000, new GUIContent("Maximum Details"));
            EditorGUILayout.IntSlider(detailSpacing, 1, 20, new GUIContent("Detail Spacing"));
            //this line is broken even copy and pasted,, 
            detailMapTable = GUITableLayout.DrawTable(detailMapTable,
                serializedObject.FindProperty("details"));
            //sets the view distance of the details to the amount of objects so you can see all of them
            terrain.GetComponent<Terrain>().detailObjectDistance = maxDetails.intValue;

            GUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+")) {
                terrain.AddDetail();
            }
            if (GUILayout.Button("-")) {
                terrain.RemoveDetail();
            }
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Apply Details")) {
                terrain.SpawnDetails();
            }
        }
        showErosion = EditorGUILayout.Foldout(showErosion, "Erosion");
        if (showErosion) {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Erosion", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(erosionType);
            EditorGUILayout.IntSlider(erosionSmoothAmount, 0, 10, new GUIContent("Smooth AMount"));
            EditorGUILayout.Slider(erosionStrength, 0.01f, 1, new GUIContent("Erosion strength"));
            EditorGUILayout.IntSlider(springsPerDroplet, 0, 60, new GUIContent("Spring/droplet"));
            EditorGUILayout.Slider(solubility, 0, 1, new GUIContent("Solubility"));
            EditorGUILayout.IntSlider(droplets, 1, 10000, new GUIContent("Droplets"));
            EditorGUILayout.Slider(thermalStrength, 0, 1, new GUIContent("ThermalStrength"));


            if (GUILayout.Button("Erode")) {
                terrain.Erode();
            }
 
    }
        //reset button seperate 
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        if (GUILayout.Button("Reset Terrain")) {
            terrain.ResetTerrain();
        }
        /*//scrollbar code END
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    */
        serializedObject.ApplyModifiedProperties();
        }

    
}
