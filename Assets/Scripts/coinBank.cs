using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coinBank : MonoBehaviour {

    private List<GameObject> tokens;
    public GameObject tokenPrefab;
    public string init_filename;
    private string result_filename;
    public int count;
    public TextMesh counter;


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
                tToken.GetComponent<basicToken>().coin_id = i;
                tToken.transform.position = new Vector3(-128, 128 , -128 + i);
                tokens.Add(tToken);
            }

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

}
