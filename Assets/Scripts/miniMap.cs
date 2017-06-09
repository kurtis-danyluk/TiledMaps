using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class miniMap : MonoBehaviour {

    float scale;
    public Generate_Terrain map;
    public bool hasChanged = false;
    Terrain mMap ;

    // Use this for initialization
    void Start () {
        //
        scale = 256;
        mMap = this.gameObject.GetComponent<Terrain>();
    }
	void LateUpdate()
    {
        if (hasChanged)
        {
            //Debug.Log(map.name);
            float[,] heights = createHeightmap(map, map.terrains_width, map.terrains_height);
            setHeightMap(heights, mMap);
            Debug.Log(map.center.Terr.terrainData.size.z);
            mMap.terrainData.size = new Vector3(mMap.terrainData.size.x, map.center.Terr.GetComponent<collect_tiles>().terrBaseHeight / (scale * map.terrains_width), mMap.terrainData.size.z);
            hasChanged = false;
        }
    }
	// Update is called once per frame
	void Update () {
        

	}
    float [,] createHeightmap(Generate_Terrain maps, int tiles_width, int tiles_height)
    {

        float[,] heights = new float[Generate_Terrain.tile_width * tiles_width, Generate_Terrain.tile_height * tiles_height];

        for(int k =0; k < tiles_width; k++)
            for(int q = 0; q < tiles_height; q++)
            {
                float[,] aHeight = maps.terrains[q, k].GetComponent<Terrain>().terrainData.GetHeights(0, 0, Generate_Terrain.tile_width, Generate_Terrain.tile_height);

                for (int i = 0; i < Generate_Terrain.tile_width; i++)
                    for (int j = 0; j < Generate_Terrain.tile_height; j++)
                    {
                        heights[i + k * Generate_Terrain.tile_width, j + q * Generate_Terrain.tile_height] = aHeight[i, j];
                    }
            }
               
        return heights;
    }
    void setHeightMap(float [,] heights , Terrain map)
    {
        map.terrainData.SetHeights(0, 0, heights);
    }
}
