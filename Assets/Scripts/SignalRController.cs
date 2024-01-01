using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System;
using UnityEngine.UI;
using System.Timers;
using Oculus.Interaction;

public class SignalRController : MonoBehaviour
{

    private static HubConnection connection;
    private ArticulationBody baseArticulation;
    private ArticulationBody armArticulation;
    private RobotController robotController;

    public GameObject robot;
    public string Rotation;
    public string Reach;
    public string Grab;
    public string signalRHubURL;

    public Slider rotateSlider;
    public Slider reachSlider;
    public Slider grabSlider;

    private int rotateValueOld;
    private int rotateValue;
    private int reachValueOld;
    private int reachValue;
    private int grabValueOld;
    private int grabValue;
    
    private bool remoteMoveRotation = false;
    private bool remoteMoveReach = false;
    private bool remoteMoveGrab = false;

    private System.Timers.Timer disableRotationTimer = new System.Timers.Timer(1000);
    private System.Timers.Timer disableReachTimer = new System.Timers.Timer(1000);
    private System.Timers.Timer disableGrabTimer = new System.Timers.Timer(1000);

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Starting SignalR Class");
        StartSignalR();
    }

    public void SetRotation(float value)
    {
        Rotation = value.ToString();
        Debug.Log("Rotation is " + value.ToString());

        if (remoteMoveRotation == false)
        {
            if (connection == null)
            {
                StartSignalR();
            }

            connection.SendAsync("SendMessage", "servo1", value.ToString());
            setRemoteRotate((int)value);
        }
        else
        {
            remoteMoveRotation = false;
        }

        //robotBody.transform.Rotate(0.0f, value, 0.0f, Space.Self);
    }

    public void SetReach(float value)
    {

        Reach = value.ToString();
        Debug.Log("Reach is " + value.ToString());

        if (remoteMoveReach == false)
        {
            if (connection == null)
            {
                StartSignalR();
            }

            connection.SendAsync("SendMessage", "servo2", value.ToString());
            setRemoteReach((int)value);
        }
        else
        {
            remoteMoveReach = false;
        }

        //robotArm.transform.Rotate(0.0f, 0.0f, value, Space.Self);
    }

    public void SetGrab(float value)
    {
        Grab = value.ToString();
        Debug.Log("Grab is " + value.ToString());

        if (remoteMoveGrab == false)
        {
            if (connection == null)
            {
                StartSignalR();
            }

            connection.SendAsync("SendMessage", "servo3", value.ToString());
            setRemoteGrab((int)value);
        }
        else
        {
            remoteMoveGrab = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        robotController = robot.GetComponent<RobotController>();
        baseArticulation = robotController.joints[0].robotPart.GetComponent<ArticulationBody>();
        armArticulation = robotController.joints[1].robotPart.GetComponent<ArticulationBody>();

        if (rotateValue != rotateValueOld )
        {
            rotateValueOld = rotateValue;
            
            var rotateDrive = baseArticulation.xDrive;
            rotateDrive.target = rotateValue;
            baseArticulation.xDrive = rotateDrive;
            remoteMoveRotation = true;
            rotateSlider.value = rotateValue;
        }

        if (reachValue != reachValueOld )
        {
            reachValueOld = reachValue;

            var reachDrive = armArticulation.xDrive;
            reachDrive.target = reachValue - 90;
            armArticulation.xDrive = reachDrive;

            remoteMoveReach = true;
            reachSlider.value = reachValue;
        }

        if (grabValue != grabValueOld )
        {
            grabValueOld = grabValue;

            float grabValueFloat;

            grabValueFloat = (float)grabValue;
            grabValueFloat = (grabValueFloat - 75) / 105;

            robotController.joints[6].robotPart.GetComponent<PincherController>().grip = grabValueFloat;
            grabSlider.value = grabValue;
        }
    }

    void setRemoteRotate(int value)
    {
        rotateValue = value;        
    }

    void setRemoteReach(int value)
    {
        reachValue = value;
    }

    void setRemoteGrab(int value)
    {
        grabValue = value;
        // Debug.Log("Grab at: " + grabValue);
    }

    void rotationDisableTimerExpired(System.Object source, ElapsedEventArgs e)
    {
        rotateSlider.interactable = true;
    }

    void reachDisableTimerExpired(System.Object source, ElapsedEventArgs e)
    {
        reachSlider.interactable = true;
    }

    void grabDisableTimerExpired(System.Object source, ElapsedEventArgs e)
    {
        grabSlider.interactable = true;
    }

    async void StartSignalR()
    {

        try
        {
            connection = new HubConnectionBuilder()
            .WithUrl(signalRHubURL)
            .WithAutomaticReconnect()
            .Build();

            connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                var encodedMsg = $"{user}: {message}";
                Debug.Log(encodedMsg);

                switch (user)
                {
                    case "servo1":

                        setRemoteRotate(Int32.Parse(message));
                        remoteMoveRotation = true;
                        rotateSlider.interactable = false;
                        disableRotationTimer.AutoReset = false;
                        disableRotationTimer.Start();
                        break;

                    case "servo2":

                        setRemoteReach(Int32.Parse(message));
                        remoteMoveReach = true;
                        reachSlider.interactable = false;
                        disableReachTimer.AutoReset = false;
                        disableReachTimer.Start();
                        break;

                    case "servo3":

                        setRemoteGrab(Int32.Parse(message));
                        remoteMoveGrab = true;
                        grabSlider.interactable = false;
                        disableGrabTimer.AutoReset = false;
                        disableGrabTimer.Start();

                        break;
                }

            });

            disableRotationTimer.Elapsed += rotationDisableTimerExpired;
            disableReachTimer.Elapsed += reachDisableTimerExpired;
            disableGrabTimer.Elapsed += grabDisableTimerExpired;

            await connection.StartAsync();
        }
        catch (Exception ex)
        {
            Debug.Log("Failed to connect to SignalR Service");
            Debug.LogException(ex);            
        }

        
    }


}
