using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
public class MovementAgents : Agent {
    private DriveController botController;
    private FieldSettings field;
    private LIDAR lidar;

    private Vector3 targetUnityCoord;
    private Vector2 targetFieldCoord;


    public GameObject goalObject;
    GameObject targetCube;
    bool isColliding = false;
    float lastThrottle = 0;
    float lastTurn = 0;
    bool isResetting = false;
    public bool polarCoordDif = false;
    public int lesson = 0; // 0 = Drive Straight, 1 = Drive to AUto Lone, 2 = Drive to X line, 3 = Drive to X,Y
    Vector3 startingPos;
    private void Start()
    {
        botController = GetComponent<DriveController>();
        lidar = GetComponentInChildren<LIDAR>();
        field = FindObjectOfType<FieldSettings>();
        goalObject.transform.parent = null;
        startingPos = transform.position;
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.tag == "Wall")
        {
            isColliding = true;
        }
    }
    float lastDistance = float.MaxValue;
    float lastRotate = float.MaxValue;
    public override void AgentReset()
    {
        lastDistance = float.MaxValue;
        lastRotate = float.MaxValue;
        isColliding = false;
        

        Destroy(targetCube);
      
        Vector3 _startPos = startingPos;
        _startPos.z += field.ConvertToUnityY(Random.Range(-0.7f, 0.7f));
        transform.position = _startPos;
        targetFieldCoord.x = Random.Range(-0.95f, 0.95f);
        targetFieldCoord.y = Random.Range(-0.95f, 0.95f);

  
        targetUnityCoord = field.ConvertToUnityFromCoord(targetFieldCoord, 1);


        goalObject.transform.position = targetUnityCoord;

        
        //Invoke("OnCompleteTimeout", 20f);
    }

    public override void CollectObservations()
    {
        float heading = transform.rotation.eulerAngles.y;
        heading /= 360;
        Vector3 currentFieldPos = field.ConvertToCoordFromUnity(transform.position);

        Vector2 diffrence = new Vector2(currentFieldPos.x - targetFieldCoord.x, currentFieldPos.y - targetFieldCoord.y);
        Vector2 polarDif = CartesianToPolar(diffrence);

        Monitor.Log("Current Field Pos", currentFieldPos.ToString(), transform);
        Monitor.Log("Current Diffrence", diffrence.ToString(), transform);
        Monitor.Log("Target Field", targetFieldCoord.ToString(), transform);

        AddVectorObs(currentFieldPos.x);
        AddVectorObs(currentFieldPos.y);
        AddVectorObs(diffrence.x);
        AddVectorObs(diffrence.y);
        //AddVectorObs(currentFieldPos.x);
        //AddVectorObs(currentFieldPos.y);
        //AddVectorObs(tagetFieldCoord.x);
        //AddVectorObs(tagetFieldCoord.y);
        AddVectorObs(heading);
        //AddVectorObs(GetComponent<Rigidbody>().velocity.z);
        //AddVectorObs(GetComponent<Rigidbody>().velocity.x);
        AddVectorObs(lidar.GetPointsRaw());
    }

    
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        

        float throttle = Mathf.Clamp(vectorAction[0], -1, 1);
        float turn = Mathf.Clamp(vectorAction[1], -1, 1);

        Vector3 currentFieldPos = field.ConvertToCoordFromUnity(transform.position);

        if(currentFieldPos.x > 1.2 || currentFieldPos.x < -1.2 || currentFieldPos.y > 1.2 || currentFieldPos.y < -1.2)
        {
            Done();
        }

        float distance = Vector2.Distance(currentFieldPos, targetFieldCoord);


        float distanceX = Mathf.Abs(currentFieldPos.x - targetFieldCoord.x) / 2;
        float distanceY = Mathf.Abs(currentFieldPos.y - targetFieldCoord.y) / 2;
        
        float normalizedDistanace = distanceX + distanceY / 2;
        Vector3 forward = transform.TransformDirection(Vector3.right);
        Vector3 toOther = targetUnityCoord - transform.position;
        float rotationDif = Mathf.Abs(Vector3.Angle(forward, toOther));

        if(rotationDif > 180)
        {
            rotationDif -= 360;
        }
        rotationDif = rotationDif / 180;

        

        

        if (lastRotate == float.MaxValue)
        {
            lastRotate = Mathf.Abs(rotationDif);
        }

        Monitor.Log("Distance", distance.ToString(), transform);

        //Time Punishment
        if (distance < 0.05)
        {
            AddReward(1.0f);
            Done();
        }


        if (isColliding)
        {
            SetReward(-1.0f);
            isColliding = false;
            Done();
        }

        if (distance < lastDistance)
        {
            AddReward(0.015f);
            lastDistance = distance;
        }

        if (distance > lastDistance)
        {
            AddReward(-0.015f);
            lastDistance = distance;
        }


        AddReward(-0.01f);
        /*
         * 
         *  if(rotationDif< lastRotate)
        {
            lastRotate = rotationDif;
            AddReward(0.02f);
        }
        */

        float deltaRotate = lastRotate - Mathf.Abs(rotationDif);

        lastRotate = Mathf.Abs(rotationDif);

  
        FindObjectOfType<DiscordStatus>().SubmitStep(GetCumulativeReward());
        lastThrottle = Mathf.Clamp(throttle, -1.0f, 1.0f);
        lastTurn = Mathf.Clamp(turn, -1.0f, 1.0f);

        //transform.position += new Vector3(Mathf.Clamp(throttle, -1.0f, 1.0f) * 0.07f, 0, Mathf.Clamp(turn, -1.0f, 1.0f) * 0.07f);
    }
    public void FixedUpdate()
    {
        botController.MovementUpdateRotated(lastThrottle, lastTurn);
    }
    private Vector2 CartesianToPolar(Vector2 input)
    {
        Vector2 polar = new Vector2();
        polar.x = Mathf.Sqrt(Mathf.Pow(input.x, 2)+  Mathf.Pow(input.y, 2));
        polar.y = Mathf.Atan2(input.y, input.x);

        polar *= Mathf.Deg2Rad;

        return polar;
    }
}
