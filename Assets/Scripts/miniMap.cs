using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class miniMap : collect_tiles {

    float scale;
    int tiles_width;
    int tiles_height;
    public collect_tiles[,] parents;
    string[,] elvFilenames;


    // Use this for initialization
    void Start () {
        scale = 1 / 256;
        tiles_width = 3;
        tiles_height = 3;
        parents = new collect_tiles[tiles_width, tiles_height];
        elvFilenames = new string[tiles_width, tiles_height];
        

    }
	void LateUpdate()
    {

    }
	// Update is called once per frame
	void Update () {
		
	}
    

}
