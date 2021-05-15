using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Events : MonoBehaviour
{
    float joystickPosThreshold = 0.85f;
    float joystickTimerThreshold = 0.1f;

    float timer = 0;

    private void Start()
    {
        
    }
    void Update()
    {
        timer += Time.deltaTime;
    }

    private bool pressControllerA(){ return OVRInput.GetDown(OVRInput.RawButton.A);}
    private bool pressControllerB(){ return OVRInput.GetDown(OVRInput.RawButton.B);}

    public bool controllerGoLeft(){ return OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x <= -joystickPosThreshold;}
    public bool controllerGoRight(){ return OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x >= joystickPosThreshold;}

    public bool press(string s) { return (Input.GetKeyDown(s)); }   

    public bool selectParticipant()
    {
        return press("a") || pressControllerA();
    }

    public bool selectExperimenter()
    {
        return press("z");
    }

    public bool printResuts()
    {
        return press("p");
    }

    public bool ooo()
    {
        return press("o");
    }

    public bool selecDir(string dir)
    {
        if(dir.Equals("left") && (press("left") || controllerGoLeft()) || dir.Equals("right") && (press("right") || controllerGoRight()) )
        {
            if (timer > joystickTimerThreshold){
                timer = 0;
                return true;
            }
        }
        return false;  
    }

}
