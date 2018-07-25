using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
public class AutoAgent : Agent
{
    private DriveController botController;
    private FieldSettings field;
    private LIDAR lidar;
    private FMS fms;

    
    private IntakeController intake;

    bool isColliding = false;
    float lastX = float.MinValue;
    float lastThrottle = 0;
    float lastTurn = 0;

    bool isResetting = false;

    Vector3 startingPos;
    Quaternion startingRot;
    private void Start()
    {
        botController = GetComponent<DriveController>();
        intake = GetComponentInChildren<IntakeController>();
        lidar = GetComponentInChildren<LIDAR>();
        field = FindObjectOfType<FieldSettings>();
        fms = FindObjectOfType<FMS>();
        startingPos = transform.position;
        startingRot = transform.rotation;
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.tag == "Wall")
        {
            isColliding = true;
        }
    }
   
    public override void AgentReset()
    {
        lastX = float.MinValue;
        isColliding = false;

        Vector3 _startPos = startingPos;
        _startPos.z += field.ConvertToUnityY(Random.Range(-0.7f, 0.7f));
        transform.position = _startPos;
        transform.rotation = startingRot;

        fms.ChooseSides();
        intake.ResetIntake();

    }

    public override void CollectObservations()
    {
        float heading = transform.rotation.eulerAngles.y;
        heading /= 360;
        Vector3 currentFieldPos = field.ConvertToCoordFromUnity(transform.position);

        AddVectorObs(currentFieldPos.x);
        AddVectorObs(currentFieldPos.y);
        AddVectorObs(heading);
        AddVectorObs(lidar.GetPointsRaw());
        AddVectorObs(fms.side);
    }


    public override void AgentAction(float[] vectorAction, string textAction)
    {


        float throttle = Mathf.Clamp(vectorAction[0], -1, 1);
        float turn = Mathf.Clamp(vectorAction[1], -1, 1);
        float intakeAction = Mathf.Clamp(vectorAction[2], 0, 1);

        Vector3 currentFieldPos = field.ConvertToCoordFromUnity(transform.position);

        Monitor.Log("Current Field", currentFieldPos.x.ToString());

        if (currentFieldPos.x > 1.2 || currentFieldPos.x < -1.2 || currentFieldPos.y > 1.2 || currentFieldPos.y < -1.2)
        {
            Done();
        }

        if (isColliding)
        {
            AddReward(-0.1f);
            isColliding = false;
        }

        if (fms.closeSwitch_Correct.scored)
        {
            Monitor.Log("Scored", "Correct");
            AddReward(0.05f);
            
        }

        if (fms.closeSwitch_Wrong.scored)
        {
            Monitor.Log("Scored", "Wrong");
            AddReward(-0.05f);
        }



        if (currentFieldPos.x > -0.35 && currentFieldPos.x < 0)
        {
            AddReward(0.015f);
        }else if (currentFieldPos.x >= 0)
        {
            AddReward(-0.05f);
        }



        AddReward(-0.005f);

        if (intakeAction > 0.9)
        {
            AddReward(-0.01f);
            intake.Drop();
        }

        FindObjectOfType<DiscordStatus>().SubmitStep(GetCumulativeReward());
        lastThrottle = Mathf.Clamp(throttle, -1.0f, 1.0f);
        lastTurn = Mathf.Clamp(turn, -1.0f, 1.0f);
        transform.position += new Vector3(lastThrottle * 0.05f, 0, lastTurn * 0.05f);




    }
    public void FixedUpdate()
    {
       
        ///botController.MovementUpdateRotated(lastThrottle, lastTurn);
    }
    private Vector2 CartesianToPolar(Vector2 input)
    {
        Vector2 polar = new Vector2();
        polar.x = Mathf.Sqrt(Mathf.Pow(input.x, 2) + Mathf.Pow(input.y, 2));
        polar.y = Mathf.Atan2(input.y, input.x);

        polar *= Mathf.Deg2Rad;

        return polar;
    }
}
