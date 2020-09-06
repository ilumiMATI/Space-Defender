using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreObjects : MonoBehaviour
{
    void Awake()
    {
        int elementsCount = FindObjectsOfType<CoreObjects>().Length;
        if (elementsCount > 1)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
