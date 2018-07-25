using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMS : MonoBehaviour {
    public List<Scoringzone> closeSwitch = new List<Scoringzone>(2);
    public Scoringzone closeSwitch_Correct;
    public Scoringzone closeSwitch_Wrong;
    public int side;
    public Material correct;
    public Material wrong;
    // Use this for initialization
    void Start () {
        ChooseSides();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChooseSides()
    {
        int ran = Random.Range(0, 10);

        if(ran >4)
        {
            closeSwitch_Correct = closeSwitch[0];
            closeSwitch_Wrong = closeSwitch[1];
            side = 0;
        }
        else
        {
            closeSwitch_Correct = closeSwitch[1];
            closeSwitch_Wrong = closeSwitch[0];
            side = 1;
        }

        closeSwitch_Correct.GetComponent<MeshRenderer>().material = correct;
        closeSwitch_Wrong.GetComponent<MeshRenderer>().material = wrong;
        closeSwitch_Correct.scored = false;
        closeSwitch_Wrong.scored = false;
    }
}
