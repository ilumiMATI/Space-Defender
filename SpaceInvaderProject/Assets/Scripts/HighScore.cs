using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HighScore : MonoBehaviour
{
    Display highScoreDisplay = null;
    NotificationDisplay highScoreNotificationDisplay = null;
    [SerializeField] float blinkingPeriodInSeconds = 0.5f;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        FindDisplays();
        HandleDisplays();
        GameSession theGameSession = FindObjectOfType<GameSession>();
        if (theGameSession)
        {
            if (HandleHighScoreCheck(FindObjectOfType<GameSession>().GetCurrentScore()))
            {
                highScoreNotificationDisplay.StartBlinking(blinkingPeriodInSeconds);
            }
        }
    }

    private void HandleDisplays()
    {
        if (highScoreDisplay)
        {
            highScoreDisplay.Set(PlayerPrefs.GetInt("HighScore", 0));
        }
        if (highScoreNotificationDisplay)
        {
            highScoreNotificationDisplay.Hide();
        }
    }

    public void ResetHighScore()
    {
        PlayerPrefs.SetInt("HighScore", 0);
    }

    public bool HandleHighScoreCheck(int scoreToCheck)
    {
        if(PlayerPrefs.GetInt("HighScore", 0) < scoreToCheck)
        {
            PlayerPrefs.SetInt("HighScore", scoreToCheck);
            return true;
        }
        return false;
    }


    private void FindDisplays()
    {
        Display[] displays = FindObjectsOfType<Display>();

        Debug.Log("HighScore found " + displays.Length + " Displays");

        foreach (Display display in displays)
        {
            switch (display.Name.ToLower())
            {
                case "highscore":
                    Debug.Log("HS: Display: highscore");
                    highScoreDisplay = display;
                    break;
                case "highscorenotification":
                    Debug.Log("HS: NotificationDisplay: highscore notification");
                    highScoreNotificationDisplay = (NotificationDisplay)display;
                    break;
            }
        }
    }
}
