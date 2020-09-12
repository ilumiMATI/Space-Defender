using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void Event();

public class TouchTracker
{
    public string name = "default";
    public Touch theTouch { get; }
    public float timeCreated { get; }
    public Event OnBegan;
    public Event OnMoved;
    public Event OnEnded;

    public TouchTracker(Touch touch, float time, Event OnBegan = null, Event OnMoved = null, Event OnEnded = null)
    {
        theTouch = touch;
        timeCreated = time;
        this.OnBegan = OnBegan;
        this.OnMoved = OnMoved;
        this.OnEnded = OnEnded;
    }
}
