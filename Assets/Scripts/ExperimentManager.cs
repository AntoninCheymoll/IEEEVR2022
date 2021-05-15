using Leap.Unity;
using OculusSampleFramework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class ExperimentManager : MonoBehaviour
{
    public enum State {Room, Task, TakeController, ProprioceptiveDriftExplanation, ProprioceptiveDrift, Questionnaire, Break, Finish }
    public enum Phase { Offset, NoOffset}

    public bool resetPath = false;
    public bool example = false;

    [Header("Experiment State")]
    public State currentState;
    public Phase currentPhase;
    public Text displayText;

    [Header("Exeriment Params")]
    public float taskDuration = 2;
    public float breakDuration = 0;

    [Header("Experiment Values")]
    public int participantNumber;
    public int currentTransform = -1;
    public int currentBlock = 0;

    [Header("Scripts")]
    public TimerManager timerManager;
    public Events events;
    public ProprioceptiveDriftManager pdManager;
    public HandBehavior handBehavior;

    [Header("Hands GameObject")]
    public GameObject leftHand;
    public GameObject rightHand;

    public GameObject[] hands;

    [Header("Other GameObjects")]

    public GameObject controller;
    public GameObject blueCircle;
    public GameObject leftIndex;

    ////////////////////////////


    Participant participant;
   
    public Sample currentSample;

    string path = "Assets/ExperimentFiles/RandomizedSamples.txt";


    // Start is called before the first frame update
    void Start()
    {
        participant =  (example)?Participant.getExampleParticipant():ExperimentFileGenerator.getExperiment().participants[participantNumber];
        
        ResultsManager.participantNumber = participantNumber;

        Sample.initiate(this);
        ResultsManager.initiate(this);
        ProprioceptiveDriftManager.initiate(this);
        Questionnaire.initiate(this);

        currentState = State.Room;
        currentPhase = Phase.NoOffset;
        startState();

        displayText.supportRichText = true;

    }

    // Update is called once per frame
    void Update() {

        testChangeState();

        if (currentState == State.Task)
        {
            currentSample.update();
        } 
        else if (currentState == State.Questionnaire)
        {
            if (events.selecDir("left")) displayText.text = Questionnaire.updateDisplayDecr();
            if (events.selecDir("right")) displayText.text = Questionnaire.updateDisplayIncr();
        }

        if (events.printResuts())
        {
            //Debug.Log(ResultsManager.toCSV());
        }
    }

    void startState()
    {
        if (currentState == State.TakeController)
        {
            activateObjects(new GameObject[] { controller });
            displayText.text = TextManager.getText(TextManager.TextName.TakeController);

        }

        else if (currentState == State.Room)
        {

            activateObjects(new GameObject[] {  });
            displayText.text = "";
            currentSample = getNextSample();

        }

        else if (currentState == State.ProprioceptiveDriftExplanation)
        {
            activateObjects(new GameObject[] { controller });
            displayText.text = TextManager.getText(TextManager.TextName.PD_Explanation);
        }
        else if (currentState == State.ProprioceptiveDrift)
        {
            pdManager.startTest();
            activateObjects(new GameObject[] { });
            displayText.text = "";
        }
        else if (currentState == State.Task)
        {
            activateObjects(new GameObject[] { leftHand, rightHand });
            displayText.text = (currentSample.stimulus == Sample.StimulusType.st)?TextManager.getText(TextManager.TextName.TaskST):TextManager.getText(TextManager.TextName.TaskVM);
            Debug.Log(displayText.text);
            currentSample.setUp();
            TimerManager.createTimer(TimerManager.TimerName.Task);
            ResultsManager.initiate_new_result(currentSample);
        }
        
        else if (currentState == State.Questionnaire)
        {
            displayText.text = Questionnaire.getFirstQuestion();
            activateObjects(new GameObject[] { controller });
        }
        else if (currentState == State.Finish)
        {
            displayText.text = TextManager.getText(TextManager.TextName.Over);
            activateObjects(new GameObject[] { });

            ResultsManager.writeResult();
        }
        else if (currentState == State.Break)
        {
            displayText.text = TextManager.getText(TextManager.TextName.Break);
            activateObjects(new GameObject[] { blueCircle, controller });
            TimerManager.createTimer(TimerManager.TimerName.Break);

        }
    }

    void testChangeState()
    {
        //currentState = isFirstSample() ? State.InitBlock : State.ProprioceptiveDriftExplanation1;

        //            currentState = State.InitSample;


        if (currentState == State.Room && events.selectExperimenter())
        {
            currentState = State.Task;
            startState();
            
        }
        else if (currentState == State.Task && (TimerManager.getTimer(TimerManager.TimerName.Task) >= taskDuration))
        {

            currentSample.end();
            currentState = State.TakeController;
            startState();

        }
        else if (currentState == State.TakeController && events.selectParticipant())
        {
            currentState = (currentPhase == Phase.Offset) ? State.ProprioceptiveDriftExplanation : State.Questionnaire;
            startState();
            

        }
        else if (currentState == State.ProprioceptiveDriftExplanation && events.selectParticipant())
        {
            currentState = State.ProprioceptiveDrift;
            startState();

        }
        else if (currentState == State.ProprioceptiveDrift && events.selectParticipant())
        {
            ResultsManager.add_PD(pdManager.endTest());

            if (isExperimentOver())
            {
                if (currentPhase == Phase.NoOffset)
                {
                    currentState = State.Break;
                }
                else
                {
                    currentState = State.Finish;
                }
                startState();
            }
            else
            {
                currentState = State.Break;
                startState();
            }



        }
        else if (currentState == State.Questionnaire && events.selectParticipant())
        {

            ResultsManager.add_Quest_Res(Questionnaire.current_res());
            string question = Questionnaire.getNextQuestion();

            if (question == null)
            {
                ResultsManager.add_Q();

                if (isExperimentOver())
                {
                    if(currentPhase == Phase.NoOffset)
                    {
                        currentState = State.Break;
                    }
                    else
                    {
                        currentState = State.Finish;
                    }
                    startState();
                }
                else
                {
                    currentState = State.Break;
                    startState();
                }
            }
            else
            {
                displayText.text = question;
            }
            
        }
        else if (currentState == State.Break && TimerManager.getTimer(TimerManager.TimerName.Break) > ((example)?0:breakDuration))
        {
            currentState = State.Room;
            startState();

        }
    }
    public void activateObjects(GameObject[] objects)
    {
        List<GameObject> tab = new List<GameObject>();
        tab.AddRange(new GameObject[] {  controller, blueCircle });
        foreach(GameObject obj in tab)
        {

            obj.SetActive(objects.Contains(obj));
        }

        tab = new List<GameObject>();
        tab.AddRange(new GameObject[] { rightHand, leftHand });
        foreach (GameObject obj in tab)
        {
            obj.SetActive(objects.Contains(obj));

            HandEnableDisable script = obj.GetComponent<HandEnableDisable>();
            script.activated = objects.Contains(obj);
        }
    }

    bool isFirstSample()
    {
        return 0 == currentTransform;
    }

    bool isBlockOver()
    {
        return participant.sampleNumber() - 1 <= currentTransform;
    }

    bool isExperimentOver() {
        return participant.sampleNumber() - 1 <= currentTransform && participant.blockNumber() - 1 <= currentBlock;
    }
    
    Sample getNextSample()
    {
        
        if(isExperimentOver())
        {
            if(currentPhase == Phase.NoOffset)
            {
                currentPhase = Phase.Offset;
                currentBlock = 0;
                currentTransform = 0;

                //activateObjects(hands);
                foreach (GameObject v in hands)
                {
                    v.GetComponent<RiggedHand>().offset = true;

                }
                //activateObjects(new GameObject[0]);


            }
            else
            return null;

        }else if(isBlockOver())
        {
            if (example) currentPhase = Phase.Offset;
            currentBlock++;
            currentTransform = 0;
        }
        else
        {
            currentTransform++;
        }
        Sample sample = participant.blocks[currentBlock][currentTransform];

        return sample;
    }

}
