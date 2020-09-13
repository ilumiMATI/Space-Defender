using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;



public class TouchTracker
{
    public delegate void PlayerTouchEvent(Touch touchInput, ref Player thePlayer);

    public string name = "default";
    public int fingerID { get; }
    public float timeCreated { get; }
    public PlayerTouchEvent OnFrame = null;
    public PlayerTouchEvent OnBegan = null;
    public PlayerTouchEvent OnStationary = null;
    public PlayerTouchEvent OnMoved = null;
    public PlayerTouchEvent OnEnded = null;
    
    public TouchTracker(int fingerID, float time)
    {
        this.fingerID = fingerID;
        timeCreated = time;
    }
}
