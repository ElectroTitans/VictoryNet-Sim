using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotController : MonoBehaviour {
    Rigidbody rBody;
    public float maxSpeed = 10;
    public float forceToAdd = 15;
	// Use this for initialization
	void Start () {
        rBody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        
    }

    private void FixedUpdate()
    {
        
    }

    public void MovementUpdate(float throttle, float turn)
    {
        if(throttle > 0 && rBody.velocity.magnitude < 10)
        {
            rBody.AddRelativeForce(Vector3.right * throttle * forceToAdd);
        }

        if (throttle < 0 && rBody.velocity.magnitude > -10)
        {
            rBody.AddRelativeForce(Vector3.right * throttle * forceToAdd);
        }
       
        transform.Rotate(Vector3.forward * turn * forceToAdd* 0.4f);
    }
}
