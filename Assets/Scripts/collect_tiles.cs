using UnityEngine;
using System.Collections;
using System.Net;
using System.IO;
using System.Linq;

public class collect_tiles : MonoBehaviour {

    WebClient client = new WebClient();
    Terrain Terr;
    Terrain mTerr;
    public Terrain me;
   // string webPath = "s3.amazonaws.com/elevation-tiles-prod/";
    static string base_dir = @"Assets/Textures/";
    private string elvFilename = "elvTile.png";
    private string aerImageFilename = "aerImage.jpeg";
    static string key = "AkkXBASn6AiuOToNWy_FDOv7iU5W8G8lyc_jYWpCKf-dWGzal51unBkQ4G209Iut";
    private string ImageURL;
    private string oImageURL;
    static float earthCircumference = 6378137f;
    public float latitude;
    private float olatitude;
    public float longitude;
    private float olongitude;
    static int zoom;
    float ozoom;
    private float[,] heights;
    private bool image_changed;
    private bool tex_swap;
    public static bool hasSeaFloor;
    float terrBaseHeight;
    float mTerrBaseHeight;
    static System.Collections.Generic.List<float> qMappingTable;
    float[,,] map;
    float[,,] mapB;

    MeshRenderer mesh;
    Texture2D filetex;
    Texture2D oFiletex;
    Texture2D tileTex;
    byte[] fileData;

    // Use this for initialization
    void Start() {

        //Setup filenames
        elvFilename = base_dir + this.name + elvFilename;
        aerImageFilename = base_dir + this.name + aerImageFilename;
        //  Debug.Log(elvFilename);

        //Load the active terrains TODO: Make this generic!
        //     Terr = Terrain.activeTerrains[1];
        Terr = me;
        mTerr = Terrain.activeTerrains[0];

        //Setup a table of heightmap values
        heights = Terr.terrainData.GetHeights(0, 0, Terr.terrainData.heightmapWidth, Terr.terrainData.heightmapHeight);

        //Setup the textures to be used
        filetex = new Texture2D(256, 256, TextureFormat.ARGB32, false);
        oFiletex = new Texture2D(256, 256, TextureFormat.ARGB32, false);
        tileTex = new Texture2D(256, 256);

     //   mesh = this.GetComponent<MeshRenderer>();
        //Setup bing maps tile URL
        ImageURL = string.Empty;
        oImageURL = ImageURL;

        //Setup default lat, long, zoom
        longitude = 0.0f;
        olongitude = longitude;
        latitude = 0.0f;
        olatitude = latitude;
        zoom = 1;
        ozoom = zoom;

        //  exageration_constant = 0;
        //Currently unused but these are set at minimums for terrain exageration
        terrBaseHeight = 15f;
        mTerrBaseHeight = 0.1f;

        //Records state of image change
        image_changed = false;
        tex_swap = false;

        //Determine if we care about the sea floor
        hasSeaFloor = false;

        
        //Link the terrain's texture to the appropriate files
        Terr.terrainData.splatPrototypes[0].texture = filetex;
     //   Terr.terrainData.splatPrototypes[1].texture = oFiletex;
        //Use map and mapB to swap between the 2 textures
        map = new float[Terr.terrainData.alphamapWidth, Terr.terrainData.alphamapHeight, 2];
        mapB = new float[Terr.terrainData.alphamapWidth, Terr.terrainData.alphamapHeight, 2];
        for (int i = 0; i < Terr.terrainData.alphamapWidth; i++)
            for (int j = 0; j < Terr.terrainData.alphamapHeight; j++)
            {
                map[i, j, 0] = 1;
                map[i, j, 1] = 0;
                mapB[i, j, 0] = 0;           
                mapB[i, j, 1] = 1;
            }

        //Generate a quantized table
        qMappingTable = generate_quantized_table();
    }


    //Some sample bing maps tile requests
    //http://dev.virtualearth.net/REST/V1/Imagery/Metadata/Aerial/40.714550167322159,-74.007124900817871?zl=15&o=xml&key=AkkXBASn6AiuOToNWy_FDOv7iU5W8G8lyc_jYWpCKf-dWGzal51unBkQ4G209Iut
    //http://dev.virtualearth.net/REST/v1/Imagery/Map/imagerySet/centerPoint/zoomLevel?mapSize=mapSize&pushpin=pushpin&mapLayer=mapLayer&format=format&mapMetadata=mapMetadata&key=BingMapsKey
    //http://dev.virtualearth.net/REST/v1/Imagery/Map/imagerySet=Aerial/centerPoint=47,-122/zoomLevel=1?mapSize=mapSize&pushpin=pushpin&mapLayer=mapLayer&format=format&mapMetadata=mapMetadata&key=BingMapsKey
    //template for an aws elevation tile request. z is zoom level and x and y refer to a tile in mercantor format
    //https://s3.amazonaws.com/elevation-tiles-prod/normal/{z}/{x}/{y}.png



    void dlFile() {
        Debug.Log(this.name + elvFilename);
        int merc_lat;
        int merc_long;

        mercator(latitude, longitude, zoom, out merc_long, out merc_lat);


        string bQuery = "http://dev.virtualearth.net/REST/V1/Imagery/Metadata/Aerial/" + latitude.ToString() +","+longitude.ToString()+"?zl="+zoom.ToString()+"&o=xml&key=" + key;
        string eQuery = "http://s3.amazonaws.com/elevation-tiles-prod/normal/"+ zoom + "/"+merc_long.ToString()+"/"+merc_lat.ToString() +".png";
        Debug.Log(eQuery);
        Debug.Log(bQuery);
       // inverse_mercator(out latitude, out longitude, zoom, merc_long, merc_lat);
        /*
        if (latitude >= 85)
            latitude = 85;
        if (latitude <= -85)
            latitude = -85;
           */
        try
        {
            //Debug.Log(elvFilename);
            client.DownloadFile(eQuery, elvFilename);
        }
        catch(WebException e)
        {
            Debug.Log("Error Trying To get Elevation Data");
            Debug.LogException(e);
            Debug.Log("latitude:" + latitude + " " + merc_lat);
            Debug.Log("Longitude:" + longitude + " " + merc_long);
            Debug.Log("Zoom:" + zoom);
            Debug.Log(eQuery);
        }
        try
        {
            
            string line = client.DownloadString(bQuery);
            string[] lines = line.Split((new char[] { '<', '>' }));
            foreach (string item in lines)
            {
                if (item.StartsWith("http"))
                    ImageURL = item;
            }

            if (!ImageURL.Equals(oImageURL))
            {
                client.DownloadFile(ImageURL, aerImageFilename);
                oImageURL = ImageURL;
                image_changed = true;
            }
        }
        catch(WebException e)
        {
            Debug.Log("Error Getting Image Data");
            Debug.LogException(e);
            Debug.Log(bQuery);
        };
        

    }
        // Update is called once per frame
	void Update () {

        if (olatitude != latitude || olongitude != longitude || ozoom != zoom)
        {           
            olatitude = latitude;
            olongitude = longitude;
            ozoom = zoom;
            dlFile();
        }

        if (File.Exists(elvFilename) && image_changed)
            formHeight();
        if (File.Exists(aerImageFilename) && image_changed)
        {
            changeTex();
            image_changed = false;
            tex_swap = true;
      //      Terr.terrainData.SetAlphamaps(0, 0, map);
        }
        
    }
    private void changeTex()
    {
        /*
        fileData = File.ReadAllBytes(aerImageFilename);
        if (tex_swap)
        {
            oFiletex.LoadImage(fileData);
            oFiletex.Apply();
            Terr.terrainData.SetAlphamaps(0, 0, mapB);
            tex_swap = false;
        }
        else
        {
            filetex.LoadImage(fileData);
            filetex.Apply();
            Terr.terrainData.SetAlphamaps(0, 0, map);
            tex_swap = true;
        }
        */
        UnityEditor.AssetDatabase.Refresh();
    }


    public void formHeight()
    {
        //Read the heightmap into a texture file for storage
        byte[] imageBytes = File.ReadAllBytes(elvFilename);
        tileTex.LoadImage(imageBytes);

        //Setup a table to hold quantized height values
        float[,] qHeights= new float[Terr.terrainData.heightmapHeight, Terr.terrainData.heightmapWidth];
        
        //Determine the min and max height.
        //We'll use these to determine the proper height value for the scene
        float min_height= float.MaxValue;
        float max_height = float.MinValue;

        //Get the heights from file and their repsective quantized values
        //While we're at it we'll find the min/max height values
        for (int i = 0; i < Terr.terrainData.heightmapWidth; i++)
            for (int j = 0; j < Terr.terrainData.heightmapHeight; j++)
            {
                
                heights[j, i] = 1- tileTex.GetPixel(i, j).a;
                qHeights[j, i] = quantized_height((int)(heights[j, i] * 255));

                if (qHeights[j, i] < 0 && !hasSeaFloor)
                    qHeights[j, i] = 0f;

                if (qHeights[j, i] < min_height)
                    min_height = qHeights[j, i];
                if (qHeights[j, i] > max_height)
                    max_height = qHeights[j, i];
            }

        if (min_height < 0 && !hasSeaFloor)
            min_height = 0;
        if (max_height < 0 && !hasSeaFloor)
            max_height = 0;

        //Determine the range between the minimum and maximum height
        float hRange = max_height - min_height;
        Debug.Log("hRange:" + hRange);

        float mRes = Mathf.Abs(ground_resolution(latitude, zoom));
        Debug.Log("mRes:" + mRes);

        terrBaseHeight = hRange / (mRes);

    //    terrBaseHeight += (exageration_constant/terrBaseHeight); 

        mTerrBaseHeight = terrBaseHeight / 256;
        
        Debug.Log("Height" + terrBaseHeight);
        Terr.terrainData.size = new Vector3(Terr.terrainData.size.x, (int)terrBaseHeight, Terr.terrainData.size.z);

        mTerr.terrainData.size = new Vector3(mTerr.terrainData.size.x, (mTerrBaseHeight), mTerr.terrainData.size.z);


        for (int i = 0; i < Terr.terrainData.heightmapWidth; i++)
            for (int j = 0; j < Terr.terrainData.heightmapHeight; j++)
            {
                qHeights[i, j] = qHeights[i, j] - min_height;
                /*
                if (qHeights[i, j] < min_height & !hasSeaFloor)
                    qHeights[i, j] = min_height;
                    */
                heights[i, j] = qHeights[i, j] / hRange;
            }

        Terr.terrainData.SetHeights(0, 0, heights);
        mTerr.terrainData.SetHeights(0, 0, heights);
        Terr.terrainData.splatPrototypes[0].normalMap = tileTex;
    //    mTerr.terrainData.splatPrototypes[0].normalMap = tileTex;

    }
    //Gives ratio of meters per pixel at given zoom level and latitude
    private float ground_resolution(float latitude, int zoom)
    {
        float ground_resolution = (Mathf.Cos(latitude * Mathf.PI / 180) * 2 * Mathf.PI * earthCircumference)
            / (256 * Mathf.Pow(2,zoom));

        return ground_resolution;
    }
    private System.Collections.Generic.List<float> generate_quantized_table()
    {
        System.Collections.Generic.List<float> table = new System.Collections.Generic.List<float>();
        for (int i = 0; i <= 11; i++)
            table.Add(-11000 + i * 1000);
        table.Add(-100);
        table.Add(-50);
        table.Add(-20);
        table.Add(-10);
        table.Add(-1);
        for (int i = 0; i <= 150; i++)
            table.Add(20 * i);
        for (int i = 0; i <= 60; i++)
            table.Add(3000 + 50 * i);
        for (int i = 0; i <= 29; i++)
            table.Add(6000 + 100 * i);

        return table;
    }
    private static float quantized_height(int h)
    {
        return qMappingTable[h];
    }

    private static void inverse_mercator(out float lat, out float lon, int zoom, int x3, int y3)
    {
        float pi = Mathf.PI;

        int tiles = (int)System.Math.Pow(2, zoom);
        float diameter = 2 * pi;

        //transform from tile space
        float x2 = ((x3 * diameter)/tiles)- pi;
        float y2 = (-1) * (((y3 * diameter) / tiles) - pi);

        //Project from mercantor
        float x1 = x2;
        float y1 = (float)System.Math.Atan(((float)System.Math.Pow(System.Math.E, y2)) - 4 * pi)* 2;

        //Convert from radians
        lon = x1 * 180 / pi;
        lat = y1 * 180 / pi;
    }

    private static void mercator(float lat, float lon, int zoom, out int x3, out int y3)
    {
        float pi = Mathf.PI;

        //convert to radians
        float x1 = lon * pi / 180;
        float y1 = lat * pi / 180;

        //project to mercantor
        float x2 = x1;
        float y2 = Mathf.Log(Mathf.Tan((0.25f * pi + 0.5f * y1)));

        //transform to tile space
        int tiles = (int)System.Math.Pow(2, zoom);
        float diameter = 2 * pi;

        x3 = (int)(tiles * (x2 + pi)/ diameter);
        y3 = (int)(tiles * (pi - y2)/ diameter);
    }

    public void watch_lat(float l)
    {
        latitude = l;
    }
    public void watch_long(float l)
    {
        longitude = l;
    }
    public void watch_zoom(float z)
    {
        zoom = (int)z;
    }
    /*
    public void watch_exageration(float e)
    {
        exageration_constant = e;
    }
*/
}
