using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class Generates and stores all of the terrains in the scene
 * 
 * 
 */
public class Generate_Terrain : MonoBehaviour {
    
    
    //2D list of all the terrains in the scene
    GameObject[,] terrains;
    private TerrainData[,] terDatas;
    //The miniMap the corrisponds to those terrains
    Terrain miniMap;
    private TerrainData terrainPrefab;


    //Number of terrains tall (must be odd number)
    public int terrains_height;
    private int centerY;

    //Number of terrains wide (must be odd number)
    public int terrains_width;
    private int centerX;

    // Use this for initialization
    void Start () {
        centerX = (int)(terrains_width / 2);
        centerY = (int)(terrains_height / 2);

        terrains = new GameObject[terrains_height, terrains_width];
        terDatas = new TerrainData[terrains_height, terrains_width];

        for (int i = 0; i < terrains_height; i++)
            for (int j = 0; j < terrains_width; j++)
            {
                terDatas[i, j] = new TerrainData();
                terDatas[i, j].heightmapResolution = 257;
                terDatas[i, j].size = new Vector3(256, 256, 256);
                terDatas[i, j].SetDetailResolution(1024, 8);
                terDatas[i, j].baseMapResolution = 1024;

                terrains[i, j] = Terrain.CreateTerrainGameObject(terDatas[i, j]);
                terrains[i, j].name = "Terr" + i + j;
                terrains[i, j].AddComponent<collect_tiles>();
            }
        for (int i = 0; i < terrains_height; i++)
            for (int j = 0; j < terrains_width; j++)
            {
                int xpos = i - centerX;
                int ypos = j - centerY;
                terrains[i, j].GetComponent<collect_tiles>().xpos = xpos;
                terrains[i, j].GetComponent<collect_tiles>().ypos = ypos;
                terrains[i, j].transform.position = new Vector3(128 * xpos , 0 , 128 * ypos);



                terrains[i, j].GetComponent<collect_tiles>().me = terrains[i, j].GetComponent<Terrain>();
                
                terrains[i, j].GetComponent<collect_tiles>().center = terrains[centerX, centerY].GetComponent<collect_tiles>();

                
            }

        terrains[centerX, centerY].GetComponent<collect_tiles>().isCenter = true;

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
