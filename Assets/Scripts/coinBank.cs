﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class coinBank : MonoBehaviour {

    public List<GameObject> tokens;
    public GameObject tokenPrefab;
    string init_filename = @"Assets/sample_coins.txt";
    private string result_filename;
    public string participant_name;
    public int count;
    public TextMesh counter;
    public GameObject beaconPrefab;
    public GameObject beaconPinPrefab;
    Generate_Terrain terrain;
    public int active_coin;
    public bool hasChanged;
    public FunctionController funcController;


	// Use this for initialization
	void Start () {

    active_coin = 0;

    tokens = new List<GameObject>();

    count = 0;
	if(init_filename == "none")
        {
            for (int i = 0; i < 10; i++)
            {
                GameObject tToken = Instantiate(tokenPrefab);
                tToken.SetActive(false);
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
        if (collect_tiles.center_changed || hasChanged)
        {
            foreach (GameObject t in tokens)
            {
                t.GetComponent<basicToken>().hasChanged = true;
            }
            hasChanged = false;
        }
        if (active_coin < tokens.Count)
        {
            if (tokens[active_coin].GetComponent<basicToken>().isGrabbed && tokens[active_coin].GetComponent<basicToken>().beaconEntered)
                active_coin++;

            if (active_coin < tokens.Count)
                if (tokens[active_coin].activeSelf == false)
                    tokens[active_coin].SetActive(true);
        }
	}

    void OnApplicationQuit()
    {

        Debug.Log("Quiting: Time Scores Are:");
        if (participant_name == null)
            participant_name = "Anon";
        string outF = string.Format("{0},Finish Time,{1}\n", participant_name, System.DateTime.Now.ToString());
        outF += string.Format(",name,,tokenID,,Token Time,,Beacon time,,Point back Angle\n");
        foreach (GameObject e in tokens)
        {
            string outl = "id:" + e.GetComponent<basicToken>().coin_id + ";Coin Time:" + e.GetComponent<basicToken>().time_grabbed + ";Beacon: " + e.GetComponent<basicToken>().time_entered + ";Point Back Angle: " + e.GetComponent<basicToken>().point_back_angle+ "\n";
            outl = string.Format("name,{0},tokenID,{1},tokenTime,{2},beaconTime,{3},pointAngle,{4}\n", participant_name, e.GetComponent<basicToken>().coin_id, e.GetComponent<basicToken>().time_grabbed, e.GetComponent<basicToken>().time_entered, e.GetComponent<basicToken>().point_back_angle);
            Debug.Log(outl);
            outF += outl;
        }
        if (result_filename != null)
            File.AppendAllText(result_filename, outF);
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
                tToken.transform.parent = this.transform;
                tToken.GetComponent<basicToken>().bank = this;
                tToken.GetComponent<basicToken>().lightHouseTransform = GameObject.Find("[CameraRig]").transform;
                tToken.GetComponent<basicToken>().coin_id = e[2];
                float x = float.Parse(e[4]);
                float z = float.Parse(e[6]);
                float y = float.Parse(e[8]);
                string bcid = e[9];

                tToken.GetComponent<basicToken>().offset = y;
                //RaycastHit hit;
                //Physics.Raycast(new Vector3(x, 1000 ,z), Vector3.down, out hit);
                //float y = hit.point.y +

                tToken.transform.position = new Vector3(x, y, z);
                tToken.name = "coin" + tToken.GetComponent<basicToken>().coin_id;
                tToken.GetComponent<basicToken>().laserPrefab = beaconPrefab;

                if (bcid == "c")
                {
                    tToken.GetComponent<basicToken>().showCoin = true;
                    tToken.GetComponent<basicToken>().showBeacon = false;
                }                   
                else if (bcid == "b")
                {

                    tToken.GetComponent<basicToken>().showCoin = false;
                    tToken.GetComponent<basicToken>().showBeacon = true;
                }
                else if (bcid == "bc" || bcid == "cb")
                {

                    tToken.GetComponent<basicToken>().showCoin = true;
                    tToken.GetComponent<basicToken>().showBeacon = true;
                }

                    tokens.Add(tToken);
            }
            if (line.StartsWith("@outfile")){
                string[] e = line.Split('\t');
                result_filename = e[1];
            }
            if (line.StartsWith("@pname"))
            {
                string[] e = line.Split('\t');
                participant_name = e[1];
            }
            if (line.StartsWith("@functions"))
            {
                string [] e =  line.Split('\t');
                string fs = e[1];

                if (fs.Contains("f"))
                    funcController.toggleFlying(true);
                else
                    funcController.toggleFlying(false);
                if (fs.Contains("r"))
                {
                    FunctionController.enableTokenMove = true;
                }
                else
                    FunctionController.enableTokenMove = false;
                if (fs.Contains("t"))
                {
                    funcController.toggleTeleportation(true);
                }
                else
                    funcController.toggleTeleportation(false);
                if (fs.Contains("m"))
                {
                    FunctionController.enableMiniMap = false;
                    funcController.boolMinimap = true;

                }
                else
                {
                    FunctionController.enableMiniMap = true;
                    funcController.boolMinimap = true;
                }
            }
        }

    }

}
