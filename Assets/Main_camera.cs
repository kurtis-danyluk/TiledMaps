using UnityEngine;
using System.Collections;

public class Main_camera : MonoBehaviour {

    public string myName;
    
    Path path;
    float percProgress;

    void Start () {

        path = new Path(0);
        path.smooth(0);
   //     Debug.Log("Total Length: " + path.length());
	}
	

	// Update is called once per frame
	void Update () {

        float trv = path.length() * percProgress;

        transform.position =  path.jump_to(trv);
        Vector3 pos= path.path[path.nPoint] - transform.position;
        Quaternion newRot = Quaternion.LookRotation(pos);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRot, 0.1f);
        /*
        Debug.Log("Travelled: " + path.travelled());
        float pcDone = path.travelled() / path.length();
        Debug.Log("% Through:" + pcDone);
        */
    }

    public void AdjustPosition(float perc)
    {
        percProgress = perc;
    }

}
