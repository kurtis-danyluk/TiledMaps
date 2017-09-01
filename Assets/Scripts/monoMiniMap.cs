﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monoMiniMap : MonoBehaviour {

    private float mapWidth;
    public float scale;
    public float yOffset;
    Terrain monoMini;
    public Terrain mainMap;
    public coinBank bank;
    public Dictionary<string, GameObject> beacons;
    public bool hasChanged = true;

    // Use this for initialization
    void Start () {
        yOffset = 0;
        mapWidth = 1;
        scale = 256;
        monoMini = this.gameObject.GetComponent<Terrain>();
        beacons = new Dictionary<string, GameObject>();
        }
	
    void LateUpdate()
    {
        
        if (hasChanged)
        {
            hasChanged = false;

            scale = mainMap.terrainData.size.x / mapWidth;
            float [,] nHeightMap = miniMap.scaleHeightmap(
                mainMap.terrainData.GetHeights(0, 0, mainMap.terrainData.heightmapWidth, mainMap.terrainData.heightmapHeight), 
                    mainMap.terrainData.heightmapWidth,
                    monoMini.terrainData.heightmapWidth);
            monoMini.terrainData.SetHeights(0, 0, nHeightMap);
            float yheight = mainMap.terrainData.size.y / scale;
            monoMini.terrainData.size = new Vector3(mapWidth, yheight , mapWidth);

            Texture2D nTex = combineSplatTextures(
                mainMap.terrainData.splatPrototypes,
                mainMap.terrainData.GetAlphamaps(0, 0, mainMap.terrainData.alphamapWidth, mainMap.terrainData.alphamapHeight),
                mainMap.terrainData.alphamapResolution,
                mainMap.GetComponent<mapTile>().detail);

            TextureScale.Point(nTex, monoMini.terrainData.alphamapWidth, monoMini.terrainData.alphamapHeight);
            

            SplatPrototype splat = new SplatPrototype();
            splat.texture = new Texture2D(monoMini.terrainData.alphamapWidth, monoMini.terrainData.alphamapHeight, TextureFormat.RGB24, true);
            Graphics.CopyTexture(nTex, splat.texture);

            splat.tileSize = new Vector2(1, 1);

            SplatPrototype [] splats = new SplatPrototype[2];
            splats[0] = monoMini.terrainData.splatPrototypes[0];
            splats[1] = splat;

            monoMini.terrainData.splatPrototypes = splats;
            

            //miniMap.terrainData.alphamapResolution = mainMap.terrainData.alphamapResolution;
            //miniMap.terrainData.SetAlphamaps(0, 0, mainMap.terrainData.GetAlphamaps(0, 0, mainMap.terrainData.alphamapWidth, mainMap.terrainData.alphamapHeight));
            //miniMap.terrainData.splatPrototypes = mainMap.terrainData.splatPrototypes;

        }
    }


    Texture2D combineSplatTextures(SplatPrototype [] splats, float [,,] AlphaMap, int AlphaMapResolution, int width)
    {
        Texture2D retTex = new Texture2D(AlphaMapResolution, AlphaMapResolution, TextureFormat.RGB24, true);
        for (int k = 0; k < splats.Length; k++)
            for (int i = 0; i<AlphaMapResolution; i++)
                for (int j = 0; j < AlphaMapResolution; j++)
                {
                    if (AlphaMap[i, j, k]  == 1)
                    {
                        int x= (int)(i % splats[k].tileSize.x);
                        int y= (int)(j % splats[k].tileSize.y);
                        Color c = splats[k].texture.GetPixel(x, y);
                        //c = c * k;
                        retTex.SetPixel(i, j, c);
                    }               
                }
        retTex.Apply();
        return retTex;
    }

	// Update is called once per frame
	void Update () {
		
	}
}
