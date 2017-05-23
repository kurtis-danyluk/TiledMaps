using UnityEngine;
using System.Collections;
using System.Net;
using System.IO;
using System.Linq;

public class collect_tiles : MonoBehaviour {

    WebClient client = new WebClient();
    Terrain Terr;
    Terrain mTerr;
   // string webPath = "s3.amazonaws.com/elevation-tiles-prod/";
    static string elvFilename = @"Assets/Textures/elvTile.png";
    static string aerImageFilename = @"Assets/Textures/aerImage.jpeg";
    static string key = "AkkXBASn6AiuOToNWy_FDOv7iU5W8G8lyc_jYWpCKf-dWGzal51unBkQ4G209Iut";
    static string ImageURL;
    static string oImageURL;
    public float latitude;
    static float olatitude;
    public float longitude;
    static float olongitude;
    static int zoom;
    static float ozoom;
    static float[,] heights;
    static bool image_changed;
    float terrBaseHeight;
    float mTerrBaseHeight;


    MeshRenderer mesh;
    Texture2D filetex;
    Texture2D tileTex;
    byte[] fileData;

    // Use this for initialization
    void Start() {
        Terr = Terrain.activeTerrains[1];
        mTerr = Terrain.activeTerrains[0];
        heights = Terr.terrainData.GetHeights(0, 0, Terr.terrainData.heightmapWidth, Terr.terrainData.heightmapHeight);
        filetex = new Texture2D(256, 256, TextureFormat.ARGB32, false);
        tileTex = new Texture2D(256, 256);
        mesh = this.GetComponent<MeshRenderer>();
        ImageURL = string.Empty;
        oImageURL = ImageURL;
        longitude = 0.0f;
        olongitude = longitude;
        latitude = 0.0f;
        olatitude = latitude;
        zoom = 1;
        ozoom = zoom;
        image_changed = false;
        terrBaseHeight = 15f;
        mTerrBaseHeight = 0.1f;
    }
    //http://dev.virtualearth.net/REST/V1/Imagery/Metadata/Aerial/40.714550167322159,-74.007124900817871?zl=15&o=xml&key=AkkXBASn6AiuOToNWy_FDOv7iU5W8G8lyc_jYWpCKf-dWGzal51unBkQ4G209Iut
    //http://dev.virtualearth.net/REST/v1/Imagery/Map/imagerySet/centerPoint/zoomLevel?mapSize=mapSize&pushpin=pushpin&mapLayer=mapLayer&format=format&mapMetadata=mapMetadata&key=BingMapsKey
    //http://dev.virtualearth.net/REST/v1/Imagery/Map/imagerySet=Aerial/centerPoint=47,-122/zoomLevel=1?mapSize=mapSize&pushpin=pushpin&mapLayer=mapLayer&format=format&mapMetadata=mapMetadata&key=BingMapsKey

    void dlFile() {
        //template for an aws elevation tile request. z is zoom level and x and y refer to a tile in mercantor format
        // https://s3.amazonaws.com/elevation-tiles-prod/normal/{z}/{x}/{y}.png
        int merc_lat;
        int merc_long;

        mercator(latitude, longitude, zoom, out merc_long, out merc_lat);

        string bQuery = "http://dev.virtualearth.net/REST/V1/Imagery/Metadata/Aerial/" + latitude.ToString() +","+longitude.ToString()+"?zl="+zoom.ToString()+"&o=xml&key=" + key;
        string eQuery = "http://s3.amazonaws.com/elevation-tiles-prod/normal/"+ zoom + "/"+merc_long.ToString()+"/"+merc_lat.ToString() +".png";
        // Debug.Log(eQuery);

        try
        {
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

        // Debug.Log(latitude + " " + longitude + " " + zoom);
        if (File.Exists(elvFilename) && image_changed)
            formHeight();
        if (File.Exists(aerImageFilename) && image_changed)
        {
            changeTex();
        }
        

    }
    private void changeTex()
    {
        fileData = File.ReadAllBytes(aerImageFilename);

        filetex.LoadImage(fileData);
        filetex.alphaIsTransparency = true;
        filetex.Apply();

        //   mesh.material.SetTexture("_MainTex", filetex);
        image_changed = false;

        Terr.terrainData.splatPrototypes[0].texture = filetex;
        float[,,] map = new float[Terr.terrainData.alphamapWidth, Terr.terrainData.alphamapHeight, 20];
        Terr.terrainData.SetAlphamaps(0, 0, map);
       
        Terr.terrainData.RefreshPrototypes();
        Terr.Flush();
        
    }


    public void formHeight()
    {
        byte[] imageBytes = File.ReadAllBytes(elvFilename);    // Read
        tileTex.LoadImage(imageBytes);
        float min_height= 1;
        for (int i = 0; i < Terr.terrainData.heightmapWidth; i++)
            for (int j = 0; j < Terr.terrainData.heightmapHeight; j++)
            {
                heights[j, i] = 1 - tileTex.GetPixel(i, j).a;
                if (heights[j, i] < min_height)
                    min_height = heights[j, i];
            }
        /*
        for (int i = 0; i < Terr.terrainData.heightmapWidth; i++)
            for (int j = 0; j < Terr.terrainData.heightmapHeight; j++)
                heights[j,i] = heights[j,i] - min_height;
*/
        Terr.terrainData.SetHeights(0, 0, heights);
        float tX = Terr.terrainData.size.x;
        float tZ = Terr.terrainData.size.z;
     //   Terr.terrainData.size = new Vector3 (tX, terrBaseHeight * (zoom - 1) * 1.2f, tZ);

        mTerr.terrainData.SetHeights(0, 0, heights);
        tX = mTerr.terrainData.size.x;
        tZ = mTerr.terrainData.size.z;
     //   mTerr.terrainData.size = new Vector3(tX, mTerrBaseHeight * (zoom - 1) * 1.2f, tZ);

        Terr.terrainData.splatPrototypes[0].normalMap = tileTex;
    //    mTerr.terrainData.splatPrototypes[0].normalMap = tileTex;

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

}
