﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;



/*
 * Class Generates and stores all of the terrains in the scene
 * 
 * 
 */
public class Generate_Terrain : MonoBehaviour {
    //Width of a single tile in game units
    public const int tile_width = 256;
    //Height of a Single tile in game units
    public const int tile_height = 256;
    
    //Width of the total map in game units
    public int map_width;
    //Height of the total map in game units
    public int map_height;

    //2D list of all the terrains in the scene
    public GameObject[,] terrains;
    private TerrainData[,] terDatas;
    //The miniMap the corrisponds to those terrains

    //Describes the tracker object to be created
    public GameObject trackerPrefab;
    //A reference to the tracker object
    public GameObject posTracker;

    public GameObject miniMap;
    private TerrainData mMapTerrData;

    private TerrainData terrainPrefab;

    public char map_style = 'r';

    //Number of terrains tall (must be odd number)
    public int terrains_height;
    private int centerY;

    //Number of terrains wide (must be odd number)
    public int terrains_width;
    private int centerX;

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
                Texture2D tex= new Texture2D(tile_width, tile_height);
                tex.LoadImage(File.ReadAllBytes(@"Assets/Textures/" + terrains[i,j].name + "aerImage.jpeg"));
                splats[0].texture = tex;
                splats[0].tileSize = new Vector2(tile_width, tile_height);

                terrains[i, j].GetComponent<Terrain>().terrainData.splatPrototypes = splats;


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
        SplatPrototype[] mSplats = new SplatPrototype[1];
        mSplats[0] = new SplatPrototype();
        mSplats[0].texture = (new Texture2D(1, 1));
        mMapTerrData.splatPrototypes = mSplats;
        
        
        miniMap = Terrain.CreateTerrainGameObject(mMapTerrData);
        miniMap.AddComponent<miniMap>();
        miniMap.GetComponent<miniMap>().map = this;
        miniMap.name = "miniMap";
        miniMap.transform.parent = this.gameObject.transform;
        miniMap.transform.localPosition = new Vector3(-0.5f, 1, -0.5f);
        miniMap.GetComponent<Terrain>().detailObjectDistance = 250;


        center.Terr = terrains[centerX, centerY].GetComponent<Terrain>();
        center.collect = terrains[centerX, centerY].GetComponent<collect_tiles>();

        terrains[centerX, centerY].GetComponent<collect_tiles>().isCenter = true;
        terrains[centerX, centerY].GetComponent<collect_tiles>().mTerr = miniMap;

        posTracker = Instantiate(trackerPrefab);

        posTracker.GetComponent<tracker_guide>().map = this;
        posTracker.GetComponent<tracker_guide>().cameraRigTransform = GameObject.Find("[CameraRig]").transform;
        posTracker.GetComponent<tracker_guide>().headTransform = GameObject.Find("Camera (eye)").transform;
        posTracker.transform.parent = miniMap.transform;
        


    }
	
	// Update is called once per frame
	void Update () {
		
	}
}