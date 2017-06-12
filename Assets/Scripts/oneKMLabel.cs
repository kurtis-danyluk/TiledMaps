using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oneKMLabel : MonoBehaviour {

    private TextMesh tMesh;
    public Generate_Terrain map;
    public miniMap mMap;

    GameObject myLine;

    // Use this for initialization
    void Start () {

        //tMesh = this.GetComponent<TextMesh>();

    }
	
	// Update is called once per frame
	void Update () {

	}

    void LateUpdate()
    {

        if (collect_tiles.center_changed)
        {
            GameObject.Destroy(myLine);
            float map_width = map.center.collect.mRes;
            float laser_length = 1/ map_width;

            Vector3 line_start = this.transform.position + new Vector3(0.5f, 0, 0);
            Vector3 line_end = line_start + new Vector3(laser_length, 0, 0);

            DrawLine(line_start, line_end , new Color(1, 0, 0));

        }

    }


    void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {
        myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        //lr.SetColors(color, color);
        lr.startColor = color;
        lr.endColor = color;
        //lr.SetWidth(0.1f, 0.1f);
        lr.startWidth = 0.01f;
        lr.endWidth = 0.01f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        //GameObject.Destroy(myLine, duration);
    }
}
