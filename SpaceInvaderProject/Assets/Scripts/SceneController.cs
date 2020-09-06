using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static void LoadStartScene()
    {
        SceneManager.LoadScene(0);
    }

    public static void LoadGameScene()
    {
        SceneManager.LoadScene(1);
    }
}
