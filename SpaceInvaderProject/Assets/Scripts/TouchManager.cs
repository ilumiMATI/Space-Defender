using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public delegate void EventDetails(string lostTrackerName);
public delegate void Event();

public class TouchManager : MonoBehaviour
{
    [SerializeField] List<TouchTracker> touchTrackers = new List<TouchTracker>();
    public EventDetails OnTrackerLost = null;
    public Event OnTrackerCreated = null;

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
            Touch handledTouch = FindTouch(touchTracker);

            if (handledTouch.phase == TouchPhase.Began)
            {
                touchTracker.OnBegan(handledTouch);
            }
            else if (handledTouch.phase == TouchPhase.Moved)
            {
                touchTracker.OnMoved(handledTouch);
            }
            else if (handledTouch.phase == TouchPhase.Stationary)
            {
                touchTracker.OnStationary(handledTouch);
            }

            touchTracker.OnFrame(handledTouch);
        }
    }

    private static Touch FindTouch(TouchTracker touchTracker)
    {
        foreach (Touch touchElement in Input.touches)
        {
            if (touchElement.fingerId == touchTracker.fingerID)
            {
                return touchElement;
            }
        }
        return Input.GetTouch(0);
    }

    private void OnFrameDefault(Touch touch)
    {

    }
    private void OnBeganDefault(Touch touch)
    {

    }
    private void OnStationaryDefault(Touch touch)
    {

    }
    private void OnMovedDefault(Touch touch)
    {

    }
    private void OnEndedDefault(Touch touch)
    {

    }

    private void HandleMultipleTouch()
    {
        for (int index = 0; index < Input.touchCount; index++)
        {
            Touch currentTouch = Input.GetTouch(index);
            TouchTracker tracker = null;

            if (currentTouch.phase == TouchPhase.Began)
            {
                tracker = new TouchTracker(currentTouch.fingerId, Time.time);
                tracker.OnBegan = OnBeganDefault;
                tracker.OnStationary = OnStationaryDefault;
                tracker.OnMoved = OnMovedDefault;
                tracker.OnEnded = OnEndedDefault;
                tracker.OnFrame = OnFrameDefault;
                touchTrackers.Add(tracker);
                OnTrackerCreated();

                //Debug.Log("Added touch." + currentTouch.fingerId);
            }
            else if (currentTouch.phase == TouchPhase.Moved)
            {

            }
            else if (currentTouch.phase == TouchPhase.Ended)
            {
                tracker = touchTrackers.Find((TouchTracker el) => el.fingerID == currentTouch.fingerId);
                tracker.OnEnded(currentTouch);
                touchTrackers.Remove(tracker);
                OnTrackerLost(tracker.name);
                //Debug.Log("Removed touch." + currentTouch.fingerId);
            }
        }
    }

    public TouchTracker GetNewTouchTracker(string name)
    {
        List<TouchTracker> notUsedTouchTrackers = touchTrackers.FindAll((TouchTracker el) => el.name == "default");
        
        if (notUsedTouchTrackers.Count > 0)
        {
            notUsedTouchTrackers.Sort(CompareByTime);
            TouchTracker chosen = notUsedTouchTrackers[notUsedTouchTrackers.Count - 1];
            chosen.name = name;
            return chosen;
        }
        else
        {
            return null;
        }
    }

    public int HowManyUsedTouchTrackers()
    {
        int counter = 0;
        foreach(TouchTracker touchTracker in touchTrackers)
        {
            if(touchTracker.name != "default")
            {
                counter++;
            }
        }
        return counter;
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