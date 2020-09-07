using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Display : MonoBehaviour
{
    [SerializeField] protected string _name = "Display";

    public string Name { get => _name; }

    protected TextMeshProUGUI textObject = null;

    void Awake()
    {
        textObject = GetComponent<TextMeshProUGUI>();
    }

    public void Set(string newText)
    {
        textObject.text = newText;
    }

    public void Set(int newNumber)
    {
        textObject.text = newNumber.ToString();
    }
}
