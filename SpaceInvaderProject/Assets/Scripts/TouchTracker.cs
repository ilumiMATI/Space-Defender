using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void TouchEvent(Touch touchInput);

public class TouchTracker
{
    public string name = "default";
    public int fingerID { get; }
    public float timeCreated { get; }
    public TouchEvent OnFrame = null;
    public TouchEvent OnBegan = null;
    public TouchEvent OnStationary = null;
    public TouchEvent OnMoved = null;
    public TouchEvent OnEnded = null;
    
    public TouchTracker(int fingerID, float time)
    {
        this.fingerID = fingerID;
        timeCreated = time;
    }
}
