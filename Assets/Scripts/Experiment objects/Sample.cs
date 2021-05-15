using Leap.Unity;
using OVRTouchSample;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

public class Sample
{

    public enum StimulusType { vm, st };

    public StimulusType stimulus;
    public int transformation;

    public int timeBeforeChangingPath = 15;

    bool isClose;
    bool pathIsOver;
    float minDist = 0.4f;


    static public ExperimentManager experimentManager;

    GameObject leftHand;
    GameObject rightHand;
    GameObject rightIndex;
    HandBehavior.Hand handPoints;


    public static void initiate(ExperimentManager experimentManager)
    {
        Sample.experimentManager = experimentManager;
    }


    public Sample(StimulusType stimulus, int transformation)
    {
        this.stimulus = stimulus;
        this.transformation = transformation;

    }

    public Sample(string parse)
    {
           
        this.stimulus = (parse[0].Equals('m')) ? StimulusType.vm : StimulusType.st;
        this.transformation = int.Parse(parse[1].ToString());
    }

    void getScripts()
    {
        leftHand = experimentManager.leftHand;
        rightHand = experimentManager.rightHand;
        rightIndex = experimentManager.leftIndex;
        handPoints = experimentManager.handBehavior.hand;
    }

    public string toString()
    {
        string stimulusString = (stimulus == StimulusType.st) ? "m" : "t";
        return stimulusString + transformation;
    }

    
    public void update()
    {

        if (stimulus == StimulusType.st)
        {
            if (!pathIsOver) pathIsOver = (handPoints.pathUpdate(rightIndex, transformation));

            if (experimentManager.resetPath || pathIsOver && isClose && Vector3.Distance(rightHand.transform.Find("R_Wrist").Find("R_Palm").transform.position, leftHand.transform.transform.Find("L_Wrist").Find("L_Palm").position) > minDist)
            {
                experimentManager.resetPath = false;

                handPoints.emptyPath();
                handPoints.firstPoint = 0;

                handPoints.path = PathManager.generateNewPath(transformation == 2);

                handPoints.displayPath(transformation);

                pathIsOver = false;
            }

            isClose = Vector3.Distance(rightHand.transform.Find("R_Wrist").Find("R_Palm").transform.position, leftHand.transform.transform.Find("L_Wrist").Find("L_Palm").position) < minDist;
        }
    }

    public void setUp()
    {
        getScripts();
        leftHand.SetActive(true);

        RiggedHand script = leftHand.GetComponent<RiggedHand>();
        script.handModel = transformation;

        if (stimulus == StimulusType.st) {
            handPoints.emptyPath();

            handPoints.path = PathManager.generateNewPath(transformation == 2);
            handPoints.firstPoint = 0;
            handPoints.displayPath(transformation);

            isClose = false;
            pathIsOver = false;
        }

        /*
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = pos.position ;
        sphere.transform.parent = pos.transform;
        sphere.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
        */
    }

    public void end()
    {
        leftHand.SetActive(false);
    }

}
