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
        if (!File.Exists(filename))
            File.AppendAllText(filename, "Date,Participant_name,Event_type,start_time,end_time,x_pos,y_pos,z_pos,x_angle,y_angle,z_angle\n");
        string format_string = "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}\n";

        if(1/Time.smoothDeltaTime <= 50 )
            yield return null;

        foreach (posTime v in rigPositions)
        {
            File.AppendAllText(filename, string.Format(format_string, System.DateTime.Now.ToString(),bank.participant_name,"HeadPosition",v.t.ToString(),v.t.ToString(), v.p.x,v.p.y,v.p.z,v.a.x,v.a.y,v.a.z));
        }
        rigPositions.Clear();
        if (1 / Time.smoothDeltaTime <= 50)
            yield return null;

        foreach (posTime v in rightHPositions)
        {
            File.AppendAllText(filename, string.Format(format_string, System.DateTime.Now.ToString(), bank.participant_name, "RightHPosition", v.t.ToString(), v.t.ToString(), v.p.x, v.p.y, v.p.z, v.a.x, v.a.y, v.a.z));
        }
        rightHPositions.Clear();
        if (1 / Time.smoothDeltaTime <= 50)
            yield return null;

        foreach (posTime v in leftHPositions)
        {
            File.AppendAllText(filename, string.Format(format_string, System.DateTime.Now.ToString(), bank.participant_name, "LeftHPosition", v.t.ToString(), v.t.ToString(), v.p.x, v.p.y, v.p.z, v.a.x, v.a.y, v.a.z));
        }
        leftHPositions.Clear();
        if (1 / Time.smoothDeltaTime <= 50)
            yield return null;

        foreach (Vector2 t in grabs)
        {
            File.AppendAllText(filename, string.Format(format_string, System.DateTime.Now.ToString(), bank.participant_name, "Grab", t.x, t.y, "", "", "", "", "", ""));
        }
        grabs.Clear();
        if (1 / Time.smoothDeltaTime <= 50)
            yield return null;
        foreach (Vector2 t in flyTouchs)
        {
            File.AppendAllText(filename, string.Format(format_string, System.DateTime.Now.ToString(), bank.participant_name, "FlyEvent", t.x, t.y, "", "", "", "", "", ""));
        }
        flyTouchs.Clear();

        if (1 / Time.smoothDeltaTime <= 50)
            yield return null;
        foreach (float t in teleports)
        {
            File.AppendAllText(filename, string.Format(format_string, System.DateTime.Now.ToString(), bank.participant_name, "Teleport", t, t, "", "", "", "", "", ""));
        }
        teleports.Clear();
    }

    void logDataFull(string filename)
    {

        if (!File.Exists(filename))
            File.AppendAllText(filename, "Date,Participant_name,Event_type,start_time,end_time,x_pos,y_pos,z_pos,x_angle,y_angle,z_angle");
        string format_string = "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}\n";

        foreach (posTime v in rigPositions)
        {
            File.AppendAllText(filename, string.Format(format_string, System.DateTime.Now.ToString(), bank.participant_name, "HeadPosition", v.t.ToString(), v.t.ToString(), v.p.x, v.p.y, v.p.z, v.a.x, v.a.y, v.a.z));
        }
        rigPositions.Clear();

        foreach (posTime v in rightHPositions)
        {
            File.AppendAllText(filename, string.Format(format_string, System.DateTime.Now.ToString(), bank.participant_name, "RightHPosition", v.t.ToString(), v.t.ToString(), v.p.x, v.p.y, v.p.z, v.a.x, v.a.y, v.a.z));
        }
        rightHPositions.Clear();

        foreach (posTime v in leftHPositions)
        {
            File.AppendAllText(filename, string.Format(format_string, System.DateTime.Now.ToString(), bank.participant_name, "LeftHPosition", v.t.ToString(), v.t.ToString(), v.p.x, v.p.y, v.p.z, v.a.x, v.a.y, v.a.z));
        }
        leftHPositions.Clear();

        foreach (Vector2 t in grabs)
        {
            File.AppendAllText(filename, string.Format(format_string, System.DateTime.Now.ToString(), bank.participant_name, "Grab", t.x, t.y, "", "", "", "", "", ""));
        }
        grabs.Clear();
        foreach (Vector2 t in flyTouchs)
        {
            File.AppendAllText(filename, string.Format(format_string, System.DateTime.Now.ToString(), bank.participant_name, "FlyEvent", t.x, t.y, "", "", "", "", "", ""));
        }
        flyTouchs.Clear();

        foreach (float t in teleports)
        {
            File.AppendAllText(filename, string.Format(format_string, System.DateTime.Now.ToString(), bank.participant_name, "Teleport", t, t, "", "", "", "", "", ""));
        }
        teleports.Clear();
    }

}
