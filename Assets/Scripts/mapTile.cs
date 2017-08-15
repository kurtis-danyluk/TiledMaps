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
    /// The web client we'll use to grab textures and heightmaps we may be missing
    /// </summary>
    static WebClient client = new WebClient();

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
    /// 0: 1 texture and heightmap
    /// 1: 4 textures and heightmaps
    /// 2: 16 textures and heightmaps
    /// n: 4^n textures and heightmaps
    /// </summary>
    private uint detail;

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
    /// The directory we'll store tiles we grab from the web for quick access
    /// </summary>
    static string base_web_dir = @"Assets/Cache/";
    
    /// <summary>
    /// The identifcation key required for 
    /// </summary>
    static string key = "AkkXBASn6AiuOToNWy_FDOv7iU5W8G8lyc_jYWpCKf-dWGzal51unBkQ4G209Iut";

    /// <summary>
    /// This list will store all of the height maps used by our map tile
    /// </summary>
    List<Texture2D> heights;

    /// <summary>
    /// Default constructor for a map tile. If no terrain is specfied the 
    /// </summary>
    /// <param name="detail"></param>
    /// <param name="mercX"></param>
    /// <param name="mercY"></param>
    /// <param name="zoom"></param>
    /// <param name="Terr"></param>
    public void SetupMapTile(uint detail, Terrain Terr, int mercX = 1, int mercY = 1, int zoom = 1)
    {
        this.detail = detail;
        this.mercX = mercX;
        this.mercY = mercY;
        this.zoom = zoom;
        if (Terr == null)
            throw new System.Exception("Terrain must be intitialized");
        this.Terr = Terr;


        pieces = (int)System.Math.Pow(4, detail);
        heights = new List<Texture2D>();

        SplatPrototype[] splats = new SplatPrototype[pieces];
        for (int i = 0; i < pieces; i++)
        {
            string fileName = base_tex_dir + i.ToString() + this.name + "tex";
            if (!File.Exists(fileName))
            {
                File.WriteAllBytes(fileName, new byte[256 * 256]);
            }
            splats[i] = new SplatPrototype();
            Texture2D tex = new Texture2D(256, 256);
            tex.LoadImage(File.ReadAllBytes(fileName));
            splats[i].texture = tex;
            splats[i].tileSize = new Vector2(256, 256);
        }
        Terr.terrainData.splatPrototypes = splats;


        for (int i = 0; i < pieces; i++)
        {
            string fileName = base_tex_dir + i.ToString() + this.name + "height";
            if (!File.Exists(fileName))
            {
                File.WriteAllBytes(fileName, new byte[256 * 256]);
            }
            Texture2D hm = new Texture2D(256, 256);
            hm.LoadImage(File.ReadAllBytes(fileName));
            heights.Add(hm);
        }

        //Create an appropriate heightmap 


        hasChanged = true;

    }
    
    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {

        if (hasChanged)
        {
            for (int i = 0; i < pieces; i++)
            {



            }

        }

	}



}
