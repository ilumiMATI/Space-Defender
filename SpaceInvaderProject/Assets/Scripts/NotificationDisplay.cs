using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationDisplay : Display
{
    [SerializeField] bool isBlinking = false;
    public void Show()
    {
        textObject.enabled = true;
    }

    public void Hide()
    {
        textObject.enabled = false;
    }

    public void StartBlinking(float periodInSeconds)
    {
        StartCoroutine(Blinking(periodInSeconds));
    }

    private IEnumerator Blinking(float periodInSeconds)
    {
        while(true)
        {
            Show();
            yield return new WaitForSeconds(periodInSeconds);
            Hide();
            yield return new WaitForSeconds(periodInSeconds);
        }
    }
}
