using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAgents : Agent {
    private DriveController botController;
    private FieldSettings field;
    private LIDAR lidar;

    private Vector3 target;
    private Vector2 tagetFieldCoord;
    public GameObject goalObject;
    GameObject targetCube;
    bool isColliding = false;
    float lastThrottle = 0;
    bool isResetting = false;

    public int lesson = 0; // 0 = Drive Straight, 1 = Drive to AUto Lone, 2 = Drive to X line, 3 = Drive to X,Y

    private void Start()
    {
        botController = GetComponent<DriveController>();
        lidar = GetComponentInChildren<LIDAR>();
        field = FindObjectOfType<FieldSettings>();
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.tag == "Wall")
        {
            isColliding = true;
        }
    }
    float lastDistance = float.MaxValue;
    public override void AgentReset()
    {
      
       
        isColliding = false;
        lastDistance = float.MaxValue;

        Destroy(targetCube);
        CancelInvoke("OnCompleteTimeout");

        Vector3 randomPos = new Vector3(field.ConvertToCoordY(-0.9f), transform.position.y, field.ConvertToCoordX(0));
       // transform.position = randomPos;
       // transform.eulerAngles = new Vector3(0,0,0);


        float fieldX = Random.Range(-0.95f, 0.95f);
        float fieldY = Random.Range(-0.95f, 0.95f);

       

        float randomX = field.ConvertToCoordX(fieldX);
        float randomY = field.ConvertToCoordY(fieldY);

        Monitor.Log("Lesson", lesson);
        switch (lesson)
        {
            case 0:
                tagetFieldCoord.x = 0;
                tagetFieldCoord.y = -0.6f;
                target = new Vector3(field.ConvertToCoordY(-0.6f), 0.1f, 0);
                break;

            case 1:
                tagetFieldCoord.x = 0;
                tagetFieldCoord.y = -0.4f;
                target = new Vector3(field.ConvertToCoordY(-0.3f), 0.1f,0);
                break;
            case 2:
                tagetFieldCoord.x = 0;
                tagetFieldCoord.y = fieldY;
                target = new Vector3(randomY, 0.1f, 0);
                break;
            case 3:
                tagetFieldCoord.x = fieldX;
                tagetFieldCoord.y = fieldY;
                target = new Vector3(randomY, 0.1f, randomX);
                break;
        }


        goalObject.transform.position = target;
       
       
        Invoke("OnCompleteTimeout", 20f);
    }

    public override void CollectObservations()
    {
        float heading = transform.rotation.eulerAngles.y;
        heading /= 360;
        Vector3 currentFieldPos = new Vector3(field.ConvertToFieldX(transform.position.z), field.ConvertToFieldY(transform.position.x), 0);
        AddVectorObs(currentFieldPos.x);
        AddVectorObs(currentFieldPos.y);
        AddVectorObs(tagetFieldCoord.x);
        AddVectorObs(tagetFieldCoord.y);
        AddVectorObs(heading);
        AddVectorObs(lidar.GetPointsRaw());
    }

    
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        Debug.Log("Actions: " + vectorAction.Length);
        float throttle = Mathf.Clamp(vectorAction[0], -1, 1);
        float turn = Mathf.Clamp(vectorAction[1], -1, 1);

        Vector3 currentFieldPos = new Vector3(field.ConvertToFieldX(transform.position.z),field.ConvertToFieldY(transform.position.x), 0);
        float distance = Vector3.Distance(transform.position, target);
        float distanceX = Mathf.Abs(currentFieldPos.x - tagetFieldCoord.x) / 2;
        float distanceY = Mathf.Abs(currentFieldPos.y - tagetFieldCoord.y) / 2;

        float normalizedDistanace = 0;
        Vector3 forward = transform.TransformDirection(Vector3.right);
        Vector3 toOther = target - transform.position;
        float rotationDif = Mathf.Abs(Vector3.Angle(forward, toOther));

        if(rotationDif > 180)
        {
            rotationDif -= 360;
        }
        rotationDif = rotationDif / 180;
      

        if(lesson < 3)
        {
            normalizedDistanace = distanceY;
        }
        else
        {
           
            normalizedDistanace = (distanceX + distanceY) / 2;
        }

        AddReward(0.3f  - (Mathf.Abs(rotationDif) * 0.75f));
        if (normalizedDistanace < 0.05)
        {
            if (!isResetting)
            {
                isResetting = true;
                Invoke("OnComplete", 3f);
            }
             AddReward(1.0f);
        }
        else if (distance < lastDistance && Mathf.Abs(distance - lastDistance) > 0.005) 
        {
            lastDistance = distance;
            Debug.Log("Add Distance");
            AddReward(3f - normalizedDistanace );
            
           
        }
        else
        {
            AddReward(Mathf.Clamp(-0.2f * turn, -0.2f, -0.08f));
            if (isResetting)
            {
                AddReward(-0.4f);
            }
        }
      
        AddReward(-0.02f);

        if (isColliding)
        {
        
            AddReward(-0.8f);
            isColliding = false;
           
        }
        AddReward(0.7f * throttle);
        AddReward(0.3f * turn);
        Monitor.Log("Distance", normalizedDistanace);
        
        //Time Punishment
      

      

        FindObjectOfType<DiscordStatus>().SubmitStep(GetCumulativeReward());
        botController.MovementUpdate(throttle , turn);

    }

    void OnComplete()
    {
        SetReward(1.0f);
        isResetting = false;
        Done();
     
    }

    void OnCompleteTimeout()
    {
        SetReward(-2.0f);
        Done();
    }
}
