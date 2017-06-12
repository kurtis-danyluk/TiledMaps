using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class miniMap : MonoBehaviour {

    public float scale;
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
            heights = scaleHeightmap(heights, map.map_width, mMap.terrainData.heightmapWidth);
            setHeightMap(heights, mMap);
            //Debug.Log(map.center.Terr.terrainData.size.z);
            float yheight = (mMap.terrainData.heightmapWidth / map.map_width   )* (map.center.Terr.GetComponent<collect_tiles>().terrBaseHeight / (scale * map.terrains_width));
            mMap.terrainData.size = new Vector3(mMap.terrainData.size.x, yheight , mMap.terrainData.size.z);
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

    float [,] scaleHeightmap(float [,] oHeights, int oSize, int nSize)
    {
        float[,] nHeights = new float[nSize, nSize];
        float scale = (float)oSize / (float)nSize;
        //Debug.Log(scale + " " + oSize + " " + nSize);
        for (int i = 0; i < nSize; i++)
            for (int j = 0; j < nSize; j++)
                nHeights[i, j] = oHeights[(int)(i * scale), (int)(j * scale)];

        return nHeights;
    }

    void setHeightMap(float [,] heights , Terrain map)
    {
        map.terrainData.SetHeights(0, 0, heights);
    }
}
