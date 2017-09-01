using System.Collections;
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
            nTex.Apply();
            SplatPrototype splat = new SplatPrototype();
            splat.texture = nTex;
            splat.tileSize = new Vector2(monoMini.terrainData.alphamapWidth, monoMini.terrainData.alphamapWidth);
            splat.tileOffset = new Vector2(0, 0);

            

            monoMini.terrainData.splatPrototypes[1] = splat;
            

            //miniMap.terrainData.alphamapResolution = mainMap.terrainData.alphamapResolution;
            //miniMap.terrainData.SetAlphamaps(0, 0, mainMap.terrainData.GetAlphamaps(0, 0, mainMap.terrainData.alphamapWidth, mainMap.terrainData.alphamapHeight));
            //miniMap.terrainData.splatPrototypes = mainMap.terrainData.splatPrototypes;

        }
    }


    Texture2D combineSplatTextures(SplatPrototype [] splats, float [,,] AlphaMap, int AlphaMapResolution, int width)
    {
        Texture2D retTex = new Texture2D(AlphaMapResolution, AlphaMapResolution, TextureFormat.RGB24, false);
        
        for (int i = 0; i<AlphaMapResolution; i++)
            for (int j = 0; j < AlphaMapResolution; j++)
                for(int k =0; k < splats.Length; k++)
                {
                    if (AlphaMap[i, j, k] > 0)
                    {
                        
                        Color c = splats[k].texture.GetPixel((int)(i % splats[k].tileSize.x), (int)(j % splats[k].tileSize.y));
                        c = c * k;
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
