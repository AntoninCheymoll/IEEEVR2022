using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextManager
{
    public enum TextName { TakeController, PD_Explanation, Over, Break, VisuoMotor, SelfTouch, Task0, Task1, Task2, Task3, TaskVM, TaskST};
    static string[] transformationName = new string[] { "normal hand", "hand with longer fingers", "six fingers hand", "block" };

    static public string getText(TextName textName)
    {
        if (textName == TextName.TakeController)return "Wait for the experimenter to give you the controller.\n\nWhen it is done, press the button A.";
        if (textName == TextName.PD_Explanation)return "Please put your index immobile on the table in front of you.\n\n After pressing A, a moving panel will appear, displace it with the joystick to align it with your index extremity and press A.";
        if (textName == TextName.Over)return "The experiment is Over, thank you for your participation.";
        if (textName == TextName.Break)return "You can take a short break.\n" +
                "Put the controller on the blue circle, then you can remove the headset.";
        if (textName == TextName.VisuoMotor) return "The next samples will be under \"Visuo-motor condition\". \n\nYou will only have the right to move your hands, please don't bring your hands close to each other.";
        if (textName == TextName.SelfTouch) return "The next samples will be under \"Self-touch condition\". \n\nDuring those samples, a path will be indicated on your left hand, please follow this path with your right index.";
        if (textName == TextName.Task0 || textName == TextName.Task1 || textName == TextName.Task2 || textName == TextName.Task3)
        {
            int transformation = int.Parse(textName.ToString()[4].ToString());

            return "Your left hand will be embodied into a " + transformationName[transformation] + ".\n\n Please put the controller on the blue circle and wait the embodiement start.";
        }
        if (textName == TextName.TaskVM) { return "You can move your hands freely, please refrain from bringging your hands close to each other."; }
        if (textName == TextName.TaskST) { return "Please follow the path indicated on your left hand with your right index.\n\nWhen the path is completed, spread your hands to reveal the next one."; };

            return "";
    }
}
