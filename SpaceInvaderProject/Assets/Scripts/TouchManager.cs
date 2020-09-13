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

    // for interacting with player
    Player thePlayer;

    void Start()
    {
        thePlayer = FindObjectOfType<Player>();
    }

    void Update()
    {
        HandleMultipleTouch();
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
                touchTrackers.Add(tracker);

                OnTrackerCreated?.Invoke();

                tracker.OnBegan?.Invoke(currentTouch, ref thePlayer);
            }
            else if (currentTouch.phase == TouchPhase.Stationary)
            {
                tracker = touchTrackers.Find((TouchTracker touchTracker) => touchTracker.fingerID == currentTouch.fingerId);

                tracker.OnStationary?.Invoke(currentTouch, ref thePlayer);
            }
            else if (currentTouch.phase == TouchPhase.Moved)
            {
                tracker = touchTrackers.Find((TouchTracker touchTracker) => touchTracker.fingerID == currentTouch.fingerId);

                tracker.OnMoved?.Invoke(currentTouch, ref thePlayer);
            }
            else if (currentTouch.phase == TouchPhase.Ended)
            {
                tracker = touchTrackers.Find((TouchTracker touchTracker) => touchTracker.fingerID == currentTouch.fingerId);

                tracker.OnEnded?.Invoke(currentTouch, ref thePlayer);

                touchTrackers.Remove(tracker);
                OnTrackerLost?.Invoke(tracker.name);
            }
            tracker.OnFrame?.Invoke(currentTouch, ref thePlayer);
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