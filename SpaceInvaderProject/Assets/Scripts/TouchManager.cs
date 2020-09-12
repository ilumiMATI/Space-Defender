using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public delegate void OnTouchTrackerLost(string lostTrackerName);

public class TouchManager : MonoBehaviour
{
    [SerializeField] List<TouchTracker> touchTrackers = new List<TouchTracker>();
    public OnTouchTrackerLost onTouchTrackerLost = null;

    void Start()
    {
        
    }

    void Update()
    {
        HandleMultipleTouch();
        HandleTrackers();
    }

    private void HandleTrackers()
    {
        foreach(TouchTracker touchTracker in touchTrackers)
        {
            if(touchTracker.theTouch.phase == TouchPhase.Began)
            {
                touchTracker.OnBegan();
            }
            else if(touchTracker.theTouch.phase == TouchPhase.Moved)
            {
                touchTracker.OnMoved();
            }
            else if (touchTracker.theTouch.phase == TouchPhase.Ended)
            {
                touchTracker.OnEnded();
                onTouchTrackerLost(touchTracker.name);
            }
        }
    }

    private void HandleMultipleTouch()
    {
        for (int index = 0; index < Input.touchCount; index++)
        {
            Touch currentTouch = Input.GetTouch(index);
            TouchTracker tracker = null;

            if (currentTouch.phase == TouchPhase.Began)
            {
                tracker = new TouchTracker(currentTouch,Time.time);
                touchTrackers.Add(tracker);

                Debug.Log("Added touch." + currentTouch.fingerId);
            }
            else if (currentTouch.phase == TouchPhase.Moved)
            {

            }
            else if (currentTouch.phase == TouchPhase.Ended)
            {
                touchTrackers.Remove(tracker);

                Debug.Log("Removed touch." + currentTouch.fingerId);
            }
        }
    }

    public TouchTracker GetNewTouchTracker(string name)
    {
        touchTrackers.Sort(CompareByTime);
        return touchTrackers[touchTrackers.Count-1];
    }

    private int CompareByTime(TouchTracker a,TouchTracker b)
    {
        if (a.timeCreated > b.timeCreated)
        {
            return -1;
        } 
        if (a.timeCreated < b.timeCreated)
        {
            return 1;
        }
        return 0;
    }
}