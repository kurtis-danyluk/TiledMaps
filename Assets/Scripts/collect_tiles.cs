using UnityEngine;
using System.Collections;
using System.Net;
using System.IO;
using System.Linq;

public class collect_tiles : MonoBehaviour {

    WebClient client = new WebClient();
    Terrain Terr;
    public Terrain mTerr;
    public Terrain me;
    public collect_tiles center;


    public bool isCenter;
   // string webPath = "s3.amazonaws.com/elevation-tiles-prod/";
    static string base_dir = @"Assets/Textures/";
    private string elvFilename = "elvTile.png";
    private string aerImageFilename = "aerImage.jpeg";
    static string key = "AkkXBASn6AiuOToNWy_FDOv7iU5W8G8lyc_jYWpCKf-dWGzal51unBkQ4G209Iut";
    string tile_type;
    private string ImageURL;
    private string oImageURL;
    static float earthCircumference = 6378137f;
    public float latitude;
    private float olatitude;
    public float longitude;
    private float olongitude;
    static int zoom;
    float ozoom;
    float tile_lat_arc;
    float tile_lon_arc;
    public int xpos;
    public int ypos;
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
        tile_type = "Road/";//"Aerial/";//
        //  Debug.Log(elvFilename);

        //Load the active terrains TODO: Make this generic!
        //     Terr = Terrain.activeTerrains[1];
        Terr = me;
        //mTerr = Terrain.activeTerrains[0];

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
        longitude = -114f;
        olongitude = longitude;
        latitude = 51f;
        olatitude = latitude;
        zoom = 11;
        ozoom = 2;
        tile_lat_arc = 90;
        tile_lon_arc = 180;

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


    void dlElvFile(int merc_long, int merc_lat, int zoom)
    {
        Debug.Log(Terr.name);
        Debug.Log("lat lon " + merc_lat + " " + merc_long);

        string eQuery = "http://s3.amazonaws.com/elevation-tiles-prod/normal/" + zoom + "/" + merc_long.ToString() + "/" + merc_lat.ToString() + ".png";
        try
        {
            //Debug.Log(elvFilename);
            client.DownloadFile(eQuery, elvFilename);
        }
        catch (WebException e)
        {
            Debug.Log("Error Trying To get Elevation Data");
            Debug.LogException(e);
            Debug.Log("latitude:" + latitude + " " + merc_lat);
            Debug.Log("Longitude:" + longitude + " " + merc_long);
            Debug.Log("Zoom:" + zoom);
            Debug.Log(eQuery);
        }


    }

    void dlElvFile(float latitude,float longitude , int zoom)
    {
        int merc_long;
        int merc_lat;

        mercator(latitude, longitude, zoom, out merc_long, out merc_lat);
        


        string eQuery = "http://s3.amazonaws.com/elevation-tiles-prod/normal/" + zoom + "/" + merc_long.ToString() + "/" + merc_lat.ToString() + ".png";

        try
        {
            //Debug.Log(elvFilename);
            client.DownloadFile(eQuery, elvFilename);
        }
        catch (WebException e)
        {
            Debug.Log("Error Trying To get Elevation Data");
            Debug.LogException(e);
            Debug.Log("latitude:" + latitude + " " + merc_lat);
            Debug.Log("Longitude:" + longitude + " " + merc_long);
            Debug.Log("Zoom:" + zoom);
            Debug.Log(eQuery);
        }


    }
    void dlImgFile(float latitude, float longitude, int zoom)
    {
        string bQuery = "http://dev.virtualearth.net/REST/V1/Imagery/Metadata/" + tile_type + latitude.ToString() + "," + longitude.ToString() + "?zl=" + zoom.ToString() + "&o=xml&key=" + key;

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
        catch (WebException e)
        {
            Debug.Log("Error Getting Image Data");
            Debug.LogException(e);
            Debug.Log(bQuery);
        };
    }
    void dlImgFile(int merc_lat, int merc_lon, int zoom)
    {
        float latitude;
        float longitude;

        inverse_mercator(out latitude, out longitude, zoom, merc_lon, merc_lat);
        Debug.Log(Terr.name);
        Debug.Log("mlat mlon " + merc_lat + " " + merc_lon);
        Debug.Log("Lat Lon " + latitude + " " + longitude);

        string bQuery = "http://dev.virtualearth.net/REST/V1/Imagery/Metadata/" + tile_type + latitude.ToString() + "," + longitude.ToString() + "?zl=" + zoom.ToString() + "&o=xml&key=" + key;

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
        catch (WebException e)
        {
            Debug.Log("Error Getting Image Data");
            Debug.LogException(e);
            Debug.Log(bQuery);
        };
    }

    void dlFile(float latitude, float longitude, int zoom) {
        //Debug.Log(this.name + elvFilename);
        int merc_lat;
        int merc_long;

        mercator(latitude, longitude, zoom, out merc_long, out merc_lat);

        string bQuery = "http://dev.virtualearth.net/REST/V1/Imagery/Metadata/" + tile_type+ latitude.ToString() +","+longitude.ToString()+"?zl="+zoom.ToString()+"&o=xml&key=" + key;
        string eQuery = "http://s3.amazonaws.com/elevation-tiles-prod/normal/"+ zoom + "/"+merc_long.ToString()+"/"+merc_lat.ToString() +".png";
        Debug.Log(Terr.name + " " + eQuery);

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

    /*
    void dlFile(int merc_lat, int merc_long, int zoom)
    {
        float latitude;
        float longitude;

        //Debug.Log(this.name + elvFilename);
        inverse_mercator(out latitude, out longitude, zoom, merc_long, merc_lat);

        string bQuery = "http://dev.virtualearth.net/REST/V1/Imagery/Metadata/" + tile_type + latitude.ToString() + "," + longitude.ToString() + "?zl=" + zoom.ToString() + "&o=xml&key=" + key;
        string eQuery = "http://s3.amazonaws.com/elevation-tiles-prod/normal/" + zoom + "/" + merc_long.ToString() + "/" + merc_lat.ToString() + ".png";
        Debug.Log(Terr.name + " " + eQuery);


        try
        {
            //Debug.Log(elvFilename);
            client.DownloadFile(eQuery, elvFilename);
        }
        catch (WebException e)
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
        catch (WebException e)
        {
            Debug.Log("Error Getting Image Data");
            Debug.LogException(e);
            Debug.Log(bQuery);
        };


    }
    */

    void LateUpdate() { }


        // Update is called once per frame
	void Update () {

        //if (!isCenter)
        
            int x3, y3;
            mercator(center.latitude, center.longitude, zoom, out x3, out y3);
           // Debug.Log(center.latitude + " " + center.longitude + " " + y3 + " " + x3);
            inverse_mercator(out this.latitude, out this.longitude, zoom, x3 + xpos, y3 - ypos);
           // Debug.Log(latitude + " " + longitude + " " + (y3 - ypos) + " " + (x3 + xpos));
        
        if (olatitude != latitude || olongitude != longitude || ozoom != zoom)
        {


            olatitude = latitude;
            olongitude = longitude;
            ozoom = zoom;

          //  dlElvFile(latitude, longitude, zoom);
          //  dlImgFile(latitude, longitude, zoom);

            dlElvFile(x3 + xpos, y3 - ypos, zoom);
            dlImgFile(x3 + xpos, y3 - ypos, zoom);


            //dlFile(this.latitude, this.longitude, zoom);
        }

        if (File.Exists(elvFilename) && image_changed)
            formHeight(Terr, elvFilename);
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
        // 
        //if (isCenter)
        //{

        try
        {
            UnityEditor.AssetDatabase.Refresh();
        }
        catch(System.Exception e)
        {

        }
            
            //}
            

    }


    public void formHeight(Terrain Terr, string elvFilename)
    {
        

        //Read the heightmap into a texture file for storage
        byte[] imageBytes = File.ReadAllBytes(elvFilename);
        tileTex.LoadImage(imageBytes);

        //Setup a table to hold quantized height values
        float[,] qHeights= new float[Terr.terrainData.heightmapHeight, Terr.terrainData.heightmapWidth];
        
        //Determine the min and max height.
        //We'll use these to determine the proper height value for the scene

        float min_height= 0f;
        float max_height = 8900f;

        if (hasSeaFloor)
            min_height = -11000f;

        //Get the heights from file and their repsective quantized values
        //While we're at it we'll find the min/max height values
        for (int i = 0; i < Terr.terrainData.heightmapWidth; i++)
            for (int j = 0; j < Terr.terrainData.heightmapHeight; j++)
            {
                
                heights[i, j] = 1- tileTex.GetPixel(i, j).a;
                qHeights[i, j] = quantized_height((int)(heights[i, j] * 255));

                if (qHeights[j, i] < 0 && !hasSeaFloor)
                    qHeights[j, i] = 0f;
                /*
                if (qHeights[j, i] < min_height)
                    min_height = qHeights[j, i];
                if (qHeights[j, i] > max_height)
                    max_height = qHeights[j, i];*/
            }
        /*
        if (min_height < 0 && !hasSeaFloor)
            min_height = 0;
        if (max_height < 0 && !hasSeaFloor)
            max_height = 0;
            */
        //Determine the range between the minimum and maximum height
        float hRange = max_height - min_height;
        //Debug.Log("hRange:" + hRange);

        float mRes = Mathf.Abs(ground_resolution(latitude, zoom));
        //Debug.Log("mRes:" + mRes);

        terrBaseHeight = hRange / (mRes);

        //    terrBaseHeight *= exageration_constant; 

        if (isCenter)
        {
            mTerrBaseHeight = terrBaseHeight / 256;
            mTerr.terrainData.size = new Vector3(mTerr.terrainData.size.x, (mTerrBaseHeight), mTerr.terrainData.size.z);
        }
        //Debug.Log("Height" + terrBaseHeight);
        Terr.terrainData.size = new Vector3(Terr.terrainData.size.x, terrBaseHeight, Terr.terrainData.size.z);

        
        


        for (int i = 0; i < Terr.terrainData.heightmapWidth; i++)
            for (int j = 0; j < Terr.terrainData.heightmapHeight; j++)
            {
                qHeights[j, i] = qHeights[j, i] - min_height;
                /*
                if (qHeights[i, j] < min_height & !hasSeaFloor)
                    qHeights[i, j] = min_height;
                    */
                heights[j, i] = qHeights[j, i] / hRange;
            }

        Terr.terrainData.SetHeights(0, 0, heights);
        if (isCenter)
            mTerr.terrainData.SetHeights(0, 0, center.heights);
        
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

        float x1,x2,y1,y2;

        int tiles = (int)System.Math.Pow(2, zoom);
        float diameter = 2 * pi;

        x1 = x2 = ((x3 * 2 * pi) / tiles) - pi; 
        lon = (180 * x1) / pi;

        y2 =  (((2 * pi * -y3) / tiles) + pi);
        y1 = (float)(2* (System.Math.Atan(System.Math.Pow(System.Math.E, y2)) - (pi * 0.25)));
        lat = (180 * y1) / pi;
        

        //y1 = (-y3 / tiles) + pi;
        //y2 = (float)System.Math.Pow(System.Math.E, y1);
        //lat = (360 * Mathf.Atan(y2) - (90 * pi)) / pi;
        //Debug.Log("Lat for " + y3 + " is " + lat); 
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
