using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LIDARDataChecker : MonoBehaviour {
    public double samplesTrue = 0;
    public double samplesTotal = 0;
    public TextMesh text;
	// Use this for initialization
	void Start () {
        text = GetComponent<TextMesh>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SubmitResult(Vector2 truePos, Vector2 prediction)
    {
        if(samplesTotal > 500)
        {
            samplesTotal = 0;
            samplesTrue = 0;
        }
        samplesTotal++;
        float distance = Vector2.Distance(truePos, prediction);

        if(distance < 0.15)
        {
            samplesTrue++;
        }

        text.text = "Acc: " +(double) System.Math.Round((samplesTrue / samplesTotal) * 100 ,2)+ "%";
    }
}
