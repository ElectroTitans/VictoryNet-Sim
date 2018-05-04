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
    // Use this for initialization
    void Start () {

        drive = GetComponent<DriveController>();
        field = FindObjectOfType<FieldSettings>();
        socket = new UdpClient(7778); // `new UdpClient()` to auto-pick port
                            // schedule the first receive operation:
        listener = new Thread(new ThreadStart(translater));
        listener.IsBackground = true;
        listener.Start();
        // sending data (for the sake of simplicity, back to ourselves):
         target = new IPEndPoint(IPAddress.Parse("10.0.0.25"), 7777);

        Visualizer.transform.parent = null;
        InvokeRepeating("ManUpdate", 1.0f,0.1f);
    }
    private void Update()
    {

        float throttle = Input.GetAxis("Vertical");
        float turn = Input.GetAxis("Horizontal");
        drive.MovementUpdate(throttle, turn);
    }
    // Update is called once per frame
    void ManUpdate () {


        


       if(dataHold)
        {
            dataHold = false;
            Debug.Log(fieldX + "/" + fieldY);
            float randomX = field.ConvertToCoordX(fieldX);
            float randomY = field.ConvertToCoordY(fieldY);

            Visualizer.transform.position =  new Vector3(randomY, 0.1f, randomX);
            
        }
        else if(!dataHold)
        {
            dataHold = true;
            List<float> lidarData = lidar.GetPointsRaw();
            string sendString = "";
            float heading = transform.rotation.eulerAngles.y;
            heading /= 360;
            sendString += heading + ",";
            for (int i = 0; i < lidarData.Count; i++)
            {
                if(i<lidarData.Count - 1)
                {
                    sendString += lidarData[0] + ",";
                }
                else
                {
                    sendString += lidarData[0];
                }
                
            }
            byte[] toSend = System.Text.Encoding.UTF8.GetBytes(sendString );
            socket.Send(toSend, toSend.Length, target);
        }
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
          
        
            Debug.Log("Raw Data: " + rawData);
            String[] vals = rawData.Split(',');
            Debug.Log(vals[0] + "," + vals[1]);
            fieldX = (float)Convert.ToDecimal(vals[0]);
            fieldY = (float)Convert.ToDecimal(vals[1]);

          

           
        }
    }
}
