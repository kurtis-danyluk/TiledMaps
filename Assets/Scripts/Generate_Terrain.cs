using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;



/*
 * Class Generates and stores all of the terrains in the scene
 * 
 * 
 */
public class Generate_Terrain : MonoBehaviour {
    /// <summary>
    /// Width of a single tile in game units eg. 256
    /// </summary>
    public const int tile_width = 256;
    /// <summary>
    /// Height of a Single tile in game units eg. 256
    /// </summary>
    public const int tile_height = 256;

    public static string init_filename = @"Assets/sample_coins.txt";

    /// <summary>
    ///Width of the total map in game units eg 768 (256 x 3)
    /// </summary>
    public int map_width;

    /// <summary>
    ///Height of the total map in game units eg 768 (256 x 3)
    /// </summary>
    public int map_height;

    //2D list of all the terrains in the scene
    public GameObject[,] terrains;
    private TerrainData[,] terDatas;
    //The miniMap the corrisponds to those terrains

    //Describes the tracker object to be created
    public GameObject trackerPrefab;
    //A reference to the tracker object
    public GameObject posTracker;

    /// <summary>
    ///A reference to the minimap we'll use in our scene
    /// </summary>
    public GameObject miniMap;
    //And the terrain data we'll use to create
    private TerrainData mMapTerrData;

    //A text mesh we're making to show how long the minimap is
    private GameObject scaleLabel;
    private GameObject oneKMLabel;

    //The basic data we'll use for all of our terrain tiles.
    private TerrainData terrainPrefab;


    /// <summary>
    ///The style of visual tiles we want to grab such as 'r' for road maps or 'a' for aerial maps
    /// </summary>
    public char map_style = 'r';

    /// <summary>
    /// Number of terrains tall (must be odd number) eg 3.
    /// </summary>
    public int terrains_height;
    private int centerY;

    /// <summary>
    /// The bank that holds all of the trials beacons and coins
    /// </summary>
    public coinBank bank;

    /// <summary>
    ///  Number of terrains wide (should be odd number) eg. 3
    /// </summary>
    public int terrains_width;
    private int centerX;

    public Material tranMat;

    public struct Center
    {
        public Terrain Terr;
        public collect_tiles collect;
    };
    public Center center;

    // Use this for initialization
    void Start () {
        map_width = tile_width * terrains_width;
        map_height = tile_height * terrains_height;

        centerX = (int)(terrains_width / 2);
        centerY = (int)(terrains_height / 2);

        terrains = new GameObject[terrains_height, terrains_width];
        terDatas = new TerrainData[terrains_height, terrains_width];

        for (int i = 0; i < terrains_height; i++)
            for (int j = 0; j < terrains_width; j++)
            {
                terDatas[i, j] = new TerrainData();
                terDatas[i, j].heightmapResolution = tile_width + 1;
                terDatas[i, j].size = new Vector3(tile_width, 1, tile_height);
                terDatas[i, j].SetDetailResolution(1024, 8);
                terDatas[i, j].baseMapResolution = 1024;

                terrains[i, j] = Terrain.CreateTerrainGameObject(terDatas[i, j]);
                terrains[i, j].name = "Terr" + i + j;
                terrains[i, j].AddComponent<collect_tiles>();

                SplatPrototype[] splats = new SplatPrototype[1];
                splats[0] = new SplatPrototype();
                Texture2D tex= new Texture2D(tile_width, tile_height, TextureFormat.ARGB32, true);
                if (!File.Exists((@"Assets/Textures/" + terrains[i, j].name + "aerImage.jpeg")))
                    collect_tiles.dlImgFile(0, 0, 0, @"Assets/Textures/" + terrains[i, j].name + "aerImage.jpeg", 'r', null);
                tex.LoadImage(File.ReadAllBytes(@"Assets/Textures/" + terrains[i,j].name + "aerImage.jpeg"));
                splats[0].texture = tex;
                splats[0].tileSize = new Vector2(tile_width, tile_height);

                terrains[i, j].GetComponent<Terrain>().terrainData.splatPrototypes = splats;
                terrains[i, j].GetComponent<Terrain>().heightmapPixelError = 8;

            }
        for (int i = 0; i < terrains_height; i++)
            for (int j = 0; j < terrains_width; j++)
            {
                int xpos = i - centerX;
                int ypos = j - centerY;
                terrains[i, j].GetComponent<collect_tiles>().xpos = xpos;
                terrains[i, j].GetComponent<collect_tiles>().ypos = ypos;
                terrains[i, j].transform.position = new Vector3(tile_width * xpos , 0 , tile_height  * ypos);

                terrains[i, j].GetComponent<collect_tiles>().me = terrains[i, j].GetComponent<Terrain>();
                terrains[i, j].GetComponent<collect_tiles>().center = terrains[centerX, centerY].GetComponent<collect_tiles>();
                terrains[i, j].GetComponent<collect_tiles>().texture_mode = map_style;
                
            }
        
       

        mMapTerrData = new TerrainData();
        mMapTerrData.heightmapResolution = tile_width * terrains_width + 1;
        mMapTerrData.size = new Vector3(1, 1, 1);
        mMapTerrData.SetDetailResolution(1024, 8);
        mMapTerrData.baseMapResolution = 1024;
        SplatPrototype[] mSplats = new SplatPrototype[2];
        mSplats[0] = new SplatPrototype();

        Texture2D circleTex = new Texture2D(256, 256, TextureFormat.ARGB32, true);
        circleTex.LoadImage(File.ReadAllBytes(@"assets/transparent256x256.png"));
            
        circleTex.Apply();
        mSplats[0].texture = circleTex;
        mSplats[0].texture.alphaIsTransparency = true;

        mSplats[0].tileSize = new Vector2(255, 255);
        
        mSplats[1] = new SplatPrototype();
        mSplats[1].texture = new Texture2D(256, 256, TextureFormat.ARGB32, true);
        mMapTerrData.splatPrototypes = mSplats;
        
        miniMap = Terrain.CreateTerrainGameObject(mMapTerrData);
        miniMap.AddComponent<miniMap>();
        miniMap.GetComponent<miniMap>().map = this;
        miniMap.GetComponent<miniMap>().bank = bank;
        miniMap.name = "miniMap";
        miniMap.transform.parent = this.gameObject.transform;
        miniMap.transform.localPosition = new Vector3(-0.5f, 1, -0.5f);
        miniMap.GetComponent<Terrain>().detailObjectDistance = 250;
        miniMap.GetComponent<Terrain>().heightmapPixelError = 3;
        //miniMap.GetComponent<Terrain>().basemapDistance = 10;
        
        miniMap.GetComponent<Terrain>().materialType = Terrain.MaterialType.Custom;
        miniMap.GetComponent<Terrain>().materialTemplate = tranMat;

        float[,,] splatMapAlphas = miniMap.GetComponent<Terrain>().terrainData.GetAlphamaps(0,0, miniMap.GetComponent<Terrain>().terrainData.alphamapWidth, miniMap.GetComponent<Terrain>().terrainData.alphamapHeight) ;

        float radius = 0.98f;
        for (int i = 0; i < miniMap.GetComponent<Terrain>().terrainData.alphamapHeight; i++)
            for (int j = 0; j < miniMap.GetComponent<Terrain>().terrainData.alphamapWidth; j++)
            {
                int height = miniMap.GetComponent<Terrain>().terrainData.alphamapHeight;
                int width = miniMap.GetComponent<Terrain>().terrainData.alphamapWidth;
                if (Mathf.Sqrt(Mathf.Pow(i - (height/2 ), 2) + Mathf.Pow(j - (width/2 ), 2)) > (radius * height * 0.5))
                {
                    splatMapAlphas[i, j, 0] = 1;
                    splatMapAlphas[i, j, 1] = 0;
                }
                else
                {

                    splatMapAlphas[i, j, 0] = 0;
                    splatMapAlphas[i, j, 1] = 1;
                }
                
            }
        miniMap.GetComponent<Terrain>().terrainData.SetAlphamaps(0, 0, splatMapAlphas);



        center.Terr = terrains[centerX, centerY].GetComponent<Terrain>();
        center.collect = terrains[centerX, centerY].GetComponent<collect_tiles>();

        terrains[centerX, centerY].GetComponent<collect_tiles>().isCenter = true;

        loadLatLonZ(init_filename, out center.collect.latitude, out center.collect.longitude, out collect_tiles.zoom);

        terrains[centerX, centerY].GetComponent<collect_tiles>().mTerr = miniMap;

        posTracker = Instantiate(trackerPrefab);

        posTracker.GetComponent<tracker_guide>().map = this;
        posTracker.GetComponent<tracker_guide>().cameraRigTransform = GameObject.Find("[CameraRig]").transform;
        posTracker.GetComponent<tracker_guide>().headTransform = GameObject.Find("Camera (eye)").transform;
        posTracker.transform.parent = miniMap.transform;

        scaleLabel = new GameObject("Scale_Label");
        scaleLabel.AddComponent<TextMesh>();
        scaleLabel.transform.parent = miniMap.transform;
        scaleLabel.GetComponent<TextMesh>().text = "Hello World";
        scaleLabel.GetComponent<TextMesh>().characterSize = 0.01f;
        scaleLabel.GetComponent<TextMesh>().fontSize = 102;
        scaleLabel.transform.localPosition = new Vector3(0, -0.2f, 0);
        scaleLabel.AddComponent<ScaleLabel>();
        scaleLabel.GetComponent<ScaleLabel>().map = this;

        
        oneKMLabel = new GameObject("One_KM_Label");
        oneKMLabel.AddComponent<TextMesh>();
        oneKMLabel.transform.parent = miniMap.transform;
        oneKMLabel.GetComponent<TextMesh>().text = "1 Kilometer";
        oneKMLabel.GetComponent<TextMesh>().characterSize = 0.01f;
        oneKMLabel.GetComponent<TextMesh>().fontSize = 102;
        oneKMLabel.transform.localPosition = new Vector3(0, 0f, 0);
        oneKMLabel.AddComponent<oneKMLabel>();
        oneKMLabel.GetComponent<oneKMLabel>().map = this;
        oneKMLabel.GetComponent<oneKMLabel>().mMap = miniMap.GetComponent<miniMap>();
        

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void loadLatLonZ(string filename, out float lat, out float lon, out int zoom)
    {
        lat = lon = zoom = 0;
        string[] fileTex = File.ReadAllLines(filename);

        foreach (string line in fileTex)
        {
            if (line.StartsWith("@Lat"))
            {
                string[] e = line.Split('\t');
                lat = float.Parse(e[1]);
            }
            else if (line.StartsWith("@Lon")){
                string[] e = line.Split('\t');
                lon = float.Parse(e[1]);
            }
            else if (line.StartsWith("@Zoom"))
            {
                string[] e = line.Split('\t');
                zoom = int.Parse(e[1]);
            }
        }

    }
}
