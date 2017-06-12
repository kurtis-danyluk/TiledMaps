using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleLabel : MonoBehaviour {
    private TextMesh tMesh;
    public Generate_Terrain map;

	// Use this for initialization
	void Start () {
        tMesh = this.GetComponent<TextMesh>();
	}
	
	// Update is called once per frame
	void Update () {
        tMesh.text = (map.center.collect.mRes * map.map_width).ToString() + "M";
	}
}
