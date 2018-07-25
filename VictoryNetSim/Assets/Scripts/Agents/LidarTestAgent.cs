using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class LidarTestAgent : MonoBehaviour {
    public LIDAR lidar;
    public string IP;
    public int PORT;
    public GameObject Visualizer;
    public DriveController drive;
    public FieldSettings field;
    public float fieldX = -2;
    public float fieldY = -2;
    Thread listener;
    UdpClient socket;
    IPEndPoint target;
    bool dataHold;
    Vector2 lastCoord;
    public List<List<float>> lidarPoints = new List<List<float>>();
    public LIDARDataChecker lidarDataChecker;
    // Use this for initialization
    void Start () {

        drive = GetComponent<DriveController>();
        field = FindObjectOfType<FieldSettings>();
        lidarDataChecker = FindObjectOfType<LIDARDataChecker>();
        socket = new UdpClient(7778); // `new UdpClient()` to auto-pick port

        listener = new Thread(new ThreadStart(translater));
        listener.IsBackground = true;
        listener.Start();
        // sending data (for the sake of simplicity, back to ourselves):
         target = new IPEndPoint(IPAddress.Parse("10.0.0.22"), 7777);

        Visualizer.transform.parent = null;
        InvokeRepeating("ManUpdate", 0.5f,0.005f);
        InvokeRepeating("LIDARUpdate", 0f, 0.0025f);
    }
    private void Update()
    {
        
        float throttle = 0;
        float turn = 0;
        if (Input.GetKey(KeyCode.W))
        {
            throttle = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            throttle = -1;
        }

        if (Input.GetKey(KeyCode.A))
        {
            turn = -1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            turn = 1;
        }
        drive.MovementUpdate(throttle, turn);
    }
    // Update is called once per frame
    void ManUpdate () {


        


       if(dataHold)
        {
            dataHold = false;
            Visualizer.transform.position = field.ConvertToUnityFromCoord(new Vector2(fieldX, fieldY), 1);

            lidarDataChecker.SubmitResult(field.ConvertToCoordFromUnity(transform.position), new Vector2(fieldX, fieldY));
            
        }
        else if(!dataHold)
        {
            dataHold = true;

            
            int pointCount = lidarPoints[0].Count;
            List<float> lidarAvg = new List<float>(new float[pointCount]);
            //Debug.Log("Point Count: " + pointCount);
            for(int i = 0; i < pointCount; i++)
            {
                float currentValue = 0;
                for(int j = 0; j < lidarPoints.Count; j++)
                {
                    currentValue += lidarPoints[j][i];
                }
                currentValue = currentValue / lidarPoints.Count;
                lidarAvg[i] = currentValue;
            }
           // Debug.Log("lidarAvg: " + lidarPoints.Count);
            string sendString = "";
            float heading = transform.rotation.eulerAngles.y;
            heading /= 360;
            sendString += heading + ",";
            for (int i = 0; i < lidarAvg.Count; i++)
            {
                if(i < lidarAvg.Count - 1)
                {
                    sendString += lidarAvg[i] + ",";
                }
                else
                {
                    sendString += lidarAvg[i];
                }
                
            }
            byte[] toSend = System.Text.Encoding.UTF8.GetBytes(sendString );
            socket.Send(toSend, toSend.Length, target);
            lidarPoints.Clear();
        }
    }
    void LIDARUpdate()
    {
        lidarPoints.Add(lidar.GetPointsRaw());
    }
    void translater()
    {
        Byte[] data = new byte[0];
        while (true)
        {
            try
            {
                data = socket.Receive(ref target);
             
            }
            catch (Exception err)
            {
                Debug.LogError(err);
                socket.Close();
                return;
            }
         
            String rawData = System.Text.Encoding.UTF8.GetString(data);
          
        
            //Debug.Log("Raw Data: " + rawData);
            String[] vals = rawData.Split(',');
            //Debug.Log(vals[0] + "," + vals[1]);
            fieldX = (float)Convert.ToDecimal(vals[0]);
            fieldY = (float)Convert.ToDecimal(vals[1]);
             
            /*

            if(lastCoord == null)
            {
                lastCoord = new Vector2(fieldX, fieldY);
            }

            float xDif = Math.Abs(fieldX - lastCoord.x);
            float yDif = Math.Abs(fieldY - lastCoord.y);

            if(xDif > 0.2 || xDif < 0.01)
            {
                fieldX = lastCoord.x;
            }

            if (yDif > 0.2 || yDif < 0.01)
            {
                fieldY = lastCoord.y;
            }

            lastCoord = new Vector2(fieldX, fieldY);
            */
        }
    }
}
