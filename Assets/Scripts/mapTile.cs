using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public class mapTile : MonoBehaviour {

    /// <summary>
    /// We'll change this to true anytime we want to reload the tile
    /// </summary>
    public bool hasChanged = false;


    /// <summary>
    /// The terrain object associated with this tile
    /// </summary>
    Terrain Terr;

    /// <summary>
    /// The X coordinate of the top left tile section in mercantor coordinates
    /// </summary>
    private int mercX;
    /// <summary>
    /// The Y coordinate of the top left tile section in mercantor coordinates
    /// </summary>
    private int mercY;
    /// <summary>
    /// The current zoom level of the total tile
    /// </summary>
    private int zoom;
    /// <summary>
    /// The detail level of the tile
    /// Described as tiles across
    /// Must be positive
    /// </summary>
    private int detail;

    /// <summary>
    /// The relative scale of the map compared to its pieces
    /// </summary>
    float scale;

    /// <summary>
    /// The number of textures and heightmaps
    /// this si derived from detail
    /// </summary>
    int pieces;

    /// <summary>
    /// The directory we'll store formated textures for this tile
    /// </summary>
    static string base_tex_dir = @"Assets/Textures/";

    /// <summary>
    /// This list will store all of the height maps used by our map tile
    /// </summary>
    List<Texture2D> heights;
    List<string> texNames;
    List<string> elvNames;

    /// <summary>
    /// Default constructor for a map tile. If no terrain is specfied the 
    /// </summary>
    /// <param name="detail"></param>
    /// <param name="mercX"></param>
    /// <param name="mercY"></param>
    /// <param name="zoom"></param>
    /// <param name="Terr"></param>
    public void SetupMapTile(int detail, Terrain Terr, int mercX = 1, int mercY = 1, int zoom = 1)
    {
        this.detail = detail;
        this.mercX = mercX;
        this.mercY = mercY;
        this.zoom = zoom;
        scale = 1;

        if (Terr == null)
            throw new System.Exception("Terrain must be intitialized");
        this.Terr = Terr;
        this.name = "monoTile";

        

        pieces = (int)detail * (int)detail;
        heights = new List<Texture2D>(pieces);

        texNames = new List<string>(pieces);
        elvNames = new List<string>(pieces);
        Terr.terrainData.size = new Vector3(256 * detail * scale, 100, 256 * detail * scale);
        float[,,] splatMapAlphas = new float[256*detail,256*detail, pieces];

        Terr.terrainData.alphamapResolution = 256 * detail;

        for (int i = 0; i < 256 * detail; i++)
            for (int j = 0; j < 256 * detail; j++)
                for (int k = 0; k < pieces; k++)
                    splatMapAlphas[i, j, k] = 0;

        SplatPrototype[] splats = new SplatPrototype[pieces];
        for (int i = 0; i < pieces; i++)
        {
            string fileName = base_tex_dir + i.ToString() + this.name + "tex.jpeg";
            texNames.Insert(i, fileName);
            if (!File.Exists(fileName))
            {
                File.WriteAllBytes(fileName, new byte[256 * 256]);
            }
            splats[i] = new SplatPrototype();
            Texture2D tex = new Texture2D(256, 256);
            tex.LoadImage(File.ReadAllBytes(fileName));
            splats[i].texture = tex;
            splats[i].tileSize = new Vector2(256, 256);
            splats[i].tileOffset = new Vector2((i % detail) * splats[i].tileSize.x, ((i / detail) * splats[i].tileSize.y));
            //for (float p = splats[i].tileOffset.x; p < splats[i].tileOffset.x + splats[i].tileSize.x; p++)

            for (int p = (int)(Terr.terrainData.alphamapResolution - splats[i].tileOffset.x) - 1; p > (Terr.terrainData.alphamapResolution - splats[i].tileOffset.x) - splats[i].tileSize.x -1; p--)
                for (float q = splats[i].tileOffset.y; q < splats[i].tileOffset.y + splats[i].tileSize.y; q++)
                //for (int q = (int)(Terr.terrainData.alphamapResolution - splats[i].tileOffset.y) - 1; q > (Terr.terrainData.alphamapResolution - splats[i].tileOffset.y) - splats[i].tileSize.y; q--)
                    splatMapAlphas[(int)p, (int)q, i] = 1;
        }
        Terr.terrainData.splatPrototypes = splats;
        
        //Terr.terrainData.heightmapResolution = 256 * detail;
        Terr.terrainData.SetAlphamaps(0, 0, splatMapAlphas);

       


            //

            for (int i = 0; i < pieces; i++)
        {
            string fileName = base_tex_dir + i.ToString() + this.name + "height.png";
            elvNames.Insert(i,fileName);
            if (!File.Exists(fileName))
            {
                File.WriteAllBytes(fileName, new byte[256 * 256]);
            }
            Texture2D hm = new Texture2D(256, 256);
            hm.LoadImage(File.ReadAllBytes(fileName));
            heights.Insert(i,hm);
        }

        //Create an appropriate heightmap 


        hasChanged = true;

    }
    
    // Use this for initialization
    void Start () {
        
	}

    // Update is called once per frame
    void Update()
    {

        if (hasChanged) {
            StartCoroutine(loadTile());
            hasChanged = false;
        }



    }

    IEnumerator loadTile()
    {
        //Grab a heightmap and throw it in our heights list

        //Grab a texture and throw it in our splats
        for(int i =0; i< pieces; i++)
        {
            int merc_lon = mercX + (i / detail);
            int merc_lat = mercY + (i % detail);
            int tZoom = zoom - (int)Mathf.Log((float)detail, 4f);
            //Debug.Log("Grabbing tile " + merc_lon + " " + merc_lat + " " + tZoom);
            collect_tiles.dlImgFile(merc_lon, merc_lat, tZoom, texNames[i], 'a', null);
            Texture2D tex = new Texture2D(256, 256);
            tex.LoadImage(File.ReadAllBytes(texNames[i]));
            Terr.terrainData.splatPrototypes[i].texture = tex;
            yield return null;
        }
        try
        {
            UnityEditor.AssetDatabase.Refresh();
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }

        

        for (int i = 0; i < pieces; i++)
        {
            collect_tiles.dlElvFile(mercY, mercX, zoom + (detail-1), elvNames[i], null);
            Texture2D tex = new Texture2D(256, 256);
            tex.LoadImage(File.ReadAllBytes(texNames[i]));
            heights[i] = tex;
            yield return null;
        }
        /*
        float[,] heightM = Terr.terrainData.GetHeights(0, 0, Terr.terrainData.heightmapWidth, Terr.terrainData.heightmapHeight);
        for(int i =0; i<heights.Count; i++)
            for(int j = 0; j < heights[i].width; j++)
                for (int k = 0; k < heights[i].height; k++)
                {
                    heightM[j + ((i%detail)*256), k + ((i / detail) * 256)] = heights[i].GetPixel(j, k).a;
                }
        Terr.terrainData.SetHeights(0, 0, heightM);
        */
        try
        {
            UnityEditor.AssetDatabase.Refresh();
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }


    }


}
