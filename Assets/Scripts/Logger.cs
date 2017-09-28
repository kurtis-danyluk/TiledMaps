using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Logger : MonoBehaviour {

    float updateRate = 1;
    float cTime;
    float nTime;

    float cTimeU;
    float nTimeU;

    public GameObject cameraRig;
    public GameObject controllerLeft;
    public GameObject controllerRight;
    public GameObject headCam;
    public coinBank bank;

    public string filename= "LogFile.csv";
    System.IO.FileStream logFile;

    public struct posTime
    {
        public Vector3 p;
        public Vector3 a;
        public float t;
    }

    public static List<Vector2> grabs;
    public static List<Vector2> flyTouchs;
    public static List<float> teleports;

    public static List<posTime> rigPositions;
    public static List<posTime> leftHPositions;
    public static List<posTime> rightHPositions;




    // Use this for initialization
    void Start () {
        //logFile = System.IO.File.Open(filename, System.IO.FileMode.Append);
        rigPositions = new List<posTime>();
        leftHPositions = new List<posTime>();
        rightHPositions = new List<posTime>();

        grabs = new List<Vector2>();
        flyTouchs = new List<Vector2>();
        teleports = new List<float>();
        cTime = nTime = cTimeU = nTimeU = Time.time;

	}
	
	void FixedUpdate()
    {
        cTimeU = Time.time;

        if (cTimeU > nTimeU)
        {
            StartCoroutine(logData("Log"+bank.result_filename));
            nTimeU = cTimeU + updateRate * 10;
        }
    }
    
    // Update is called once per frame
	void Update () {

        cTime = Time.time;

        if (cTime > nTime)
        {
            posTime temp = new posTime();
            temp.p = headCam.transform.position;
            temp.a = headCam.transform.transform.forward;
            temp.t = Time.time;
            rigPositions.Add(temp);

            posTime tempR = new posTime();
            tempR.p = controllerRight.transform.position;
            tempR.a = controllerRight.transform.transform.forward;
            tempR.t = Time.time;
            rightHPositions.Add(tempR);

            posTime tempL = new posTime();
            tempL.p = controllerLeft.transform.position;
            tempL.a = controllerLeft.transform.transform.forward;
            tempL.t = Time.time;
            leftHPositions.Add(tempL);

            nTime = cTime + updateRate;
        }
    }

    void OnApplicationQuit()
    {
        logDataFull("Log" + bank.result_filename);
    }

    IEnumerator logData(string filename)
    {
        
        if(1/Time.smoothDeltaTime <= 50 )
            yield return null;

        foreach (posTime v in rigPositions)
        {
            File.AppendAllText(filename, string.Format("{4},name,{3},HeadPosition,{0},time,{1},angle,{2}\n", v.p.ToString(), v.t.ToString(), v.a.ToString(), bank.participant_name, System.DateTime.Now.ToString()));
        }
        rigPositions.Clear();
        if (1 / Time.smoothDeltaTime <= 50)
            yield return null;

        foreach (posTime v in rightHPositions)
        {
            File.AppendAllText(filename, string.Format("{4},name,{3},RightHandPosition,{0},time,{1},angle,{2}\n", v.p.ToString(), v.t.ToString(), v.a.ToString(), bank.participant_name, System.DateTime.Now.ToString()));
        }
        rightHPositions.Clear();
        if (1 / Time.smoothDeltaTime <= 50)
            yield return null;

        foreach (posTime v in leftHPositions)
        {
            File.AppendAllText(filename, string.Format("{4},name,{3},LeftHandPosition,{0},time,{1},angle,{2}\n", v.p.ToString(), v.t.ToString(), v.a.ToString(), bank.participant_name, System.DateTime.Now.ToString()));
        }
        leftHPositions.Clear();
        if (1 / Time.smoothDeltaTime <= 50)
            yield return null;

        foreach (Vector2 t in grabs)
            File.AppendAllText(filename, string.Format("{3},name,{2},grab,time,{0},release,{1}\n", t.x,t.y, bank.participant_name, System.DateTime.Now.ToString()));
        grabs.Clear();
        if (1 / Time.smoothDeltaTime <= 50)
            yield return null;
        foreach (Vector2 t in flyTouchs)
        {
            File.AppendAllText(filename, string.Format("{3},name,{2},flytouch,down,time,{0},up,{1}\n", t.x,t.y, bank.participant_name, System.DateTime.Now.ToString()));
        }
        flyTouchs.Clear();

        if (1 / Time.smoothDeltaTime <= 50)
            yield return null;
        foreach (float t in teleports)
        {
            File.AppendAllText(filename, string.Format("{2},name,{1},teleport,time,{0}\n", t.ToString(), bank.participant_name, System.DateTime.Now.ToString()));
        }
        teleports.Clear();
    }

    void logDataFull(string filename)
    {

        
        foreach (posTime v in rigPositions)
        {
            File.AppendAllText(filename, string.Format("{4},name,{3},HeadPosition,{0},time,{1},angle,{2}\n", v.p.ToString(), v.t.ToString(), v.a.ToString(), bank.participant_name, System.DateTime.Now.ToString()));
        }
        rigPositions.Clear();
        
        foreach (posTime v in rightHPositions)
        {
            File.AppendAllText(filename, string.Format("{4},name,{3},RightHandPosition,{0},time,{1},angle,{2}\n", v.p.ToString(), v.t.ToString(), v.a.ToString(), bank.participant_name, System.DateTime.Now.ToString()));
        }
        rightHPositions.Clear();
        
        foreach (posTime v in leftHPositions)
        {
            File.AppendAllText(filename, string.Format("{4},name,{3},LeftHandPosition,{0},time,{1},angle,{2}\n", v.p.ToString(), v.t.ToString(), v.a.ToString(), bank.participant_name, System.DateTime.Now.ToString()));
        }
        leftHPositions.Clear();
        
        foreach (Vector2 t in grabs)
            File.AppendAllText(filename, string.Format("{3},name,{2},grab,time,{0},release,{1}\n", t.x, t.y, bank.participant_name, System.DateTime.Now.ToString()));
        grabs.Clear();
        foreach (Vector2 t in flyTouchs)
        {
            File.AppendAllText(filename, string.Format("{3},name,{2},flytouch,down,time,{0},up,{1}\n", t.x, t.y, bank.participant_name, System.DateTime.Now.ToString()));
        }
        flyTouchs.Clear();
        
        foreach (float t in teleports)
        {
            File.AppendAllText(filename, string.Format("{2},name,{1},teleport,time,{0}\n", t.ToString(), bank.participant_name, System.DateTime.Now.ToString()));
        }
        teleports.Clear();
    }

}
