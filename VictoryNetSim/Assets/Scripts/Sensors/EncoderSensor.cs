using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncoderSensor : MonoBehaviour {
    public int Ticks;
    public int TicksPerUnit = 200;
    public Vector3 lastPos ;
	// Use this for initialization
	void Start () {
		
	}
	
	int Measure()
    {
        if (lastPos == null)
        {
            Ticks = 0;
            lastPos = transform.position;
            return Ticks;
        }

        Vector2 _2DLastPos = new Vector2(lastPos.x, lastPos.z);
        Vector2 _2DCurrentPos = new Vector2(transform.position.x, transform.position.z);

        Ticks += (int)Vector2.Distance(_2DLastPos, _2DCurrentPos) * TicksPerUnit;

        lastPos = transform.position;

        return Ticks;
    }

   void ResetTicks()
    {
        Ticks = 0;
    }
}
