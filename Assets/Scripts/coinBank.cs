using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class coinBank : MonoBehaviour {

    private List<GameObject> tokens;
    public GameObject tokenPrefab;
    string init_filename = @"Assets/sample_coins.txt";
    private string result_filename;
    public int count;
    public TextMesh counter;
    public GameObject beaconPrefab;


	// Use this for initialization
	void Start () {

    tokens = new List<GameObject>();

    count = 0;
	if(init_filename == "none")
        {
            for (int i = 0; i < 10; i++)
            {
                GameObject tToken = Instantiate(tokenPrefab);
                tToken.GetComponent<basicToken>().bank = this;
                tToken.GetComponent<basicToken>().coin_id = i.ToString();
               
                tToken.transform.position = new Vector3(-128, 128 , -128 + i);
                tokens.Add(tToken);
            }

        }
     else if(init_filename != null)
        {
            loadCoins(init_filename, out tokens);
        }	
	}
	
	// Update is called once per frame
	void Update () {
        counter.text = count.ToString();
	}

    void OnApplicationQuit()
    {
        Debug.Log("Quiting: Time Scores Are:");
        foreach (GameObject e in tokens)
            Debug.Log("id:"+e.GetComponent<basicToken>().coin_id + ";Time:" + e.GetComponent<basicToken>().time_grabbed);
    }

    void loadCoins(string filename, out List<GameObject> tokens)
    {
        
        tokens = new List<GameObject>();
        string [] fileTex = File.ReadAllLines(filename);

        foreach(string line in fileTex)
        {
            if (line.StartsWith("@coin"))
            {
                string[] e = line.Split('\t');

                GameObject tToken = Instantiate(tokenPrefab);
                tToken.GetComponent<basicToken>().bank = this;
                tToken.GetComponent<basicToken>().coin_id = e[2];
                float x = float.Parse(e[4]);
                float z = float.Parse(e[6]);
                float y = float.Parse(e[8]);
                //RaycastHit hit;
                //Physics.Raycast(new Vector3(x, 1000 ,z), Vector3.down, out hit);
                //float y = hit.point.y +

                tToken.transform.position = new Vector3(x, y, z);
                tToken.name = "coin" + tToken.GetComponent<basicToken>().coin_id;
                tToken.GetComponent<basicToken>().laserPrefab = beaconPrefab;
                tokens.Add(tToken);
            }
        }

    }

}
