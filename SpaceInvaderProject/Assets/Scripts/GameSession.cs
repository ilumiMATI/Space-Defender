using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEditor.Build.Content;

public class GameSession : MonoBehaviour
{
    // state
    //ScoreDisplay scoreDisplay = null;
    Display scoreDisplay = null;
    [SerializeField]Display healthDisplay = null;
    [SerializeField] int currentScore = 0;

    void OnEnable()
    {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        FindDisplays();
    }
    void Awake()
    {
        SetupSingleton();
        Debug.LogWarning("GS SINGLETON READY");
    }
    void Start()
    {
        
    }
    
    private void FindDisplays()
    {
        Display[] displays = FindObjectsOfType<Display>();

        Debug.Log("GameSession found " + displays.Length + " Displays");

        foreach (Display display in displays)
        {
            switch (display.Name.ToLower())
            {
                case "score":
                    Debug.Log("GS: Display: score");
                    scoreDisplay = display;
                    UpdateScore();
                    break;
                case "health":
                    Debug.Log("GS: Display: health");
                    healthDisplay = display;
                    break;
            }
        }
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public void AddScore(int points)
    {
        currentScore += points;
        UpdateScore();
    }

    public void UpdateScore()
    {
        if(scoreDisplay)
        {
            scoreDisplay.Set(currentScore);
        }
    }

    public void UpdateHealth(int currentHealth)
    {
        Debug.LogWarning("HP Update");
        if(healthDisplay)
        {
            healthDisplay.Set(currentHealth);
        }
    }

    public void Reset()
    {
        Destroy(gameObject);
    }

    private bool SetupSingleton()
    {
        if(FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
            return false;
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            return true;
        }
    }
}
