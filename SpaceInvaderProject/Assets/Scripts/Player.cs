using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 8f;
    [SerializeField] float padding = 1f;

    float xMin;
    float xMax;
    float yMin;
    float yMax;

    // Start is called before the first frame update
    void Start()
    {
        SetUpMoveBoundaries();
    }

    private void SetUpMoveBoundaries()
    {
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + padding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - padding;
        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + padding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - padding;
    }

    // Update is called once per frame
    void Update()
    {
        SmoothMove();
        //TouchMove();
    }

    private void RapidMove()
    {
        var deltaX = Input.GetAxisRaw("Horizontal") * Time.deltaTime * moveSpeed;
        var deltaY = Input.GetAxisRaw("Vertical") * Time.deltaTime * moveSpeed;

        var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);
        var newYPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);
        transform.position = new Vector2(newXPos, newYPos);
    }
    private void SmoothMove()
    {
        var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;

        var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);
        var newYPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);
        transform.position = new Vector2(newXPos, newYPos);
    }

    private void TouchMove()
    {
        if (Input.touchCount > 0)
        {
            var touchScreenPos = Input.GetTouch(0).position;
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(touchScreenPos);
            Vector2 deltaPos = (touchPos - (Vector2)transform.position).normalized * Time.deltaTime * moveSpeed;

            Vector2 newPos = (Vector2)transform.position + deltaPos;

            newPos.x = Mathf.Clamp(newPos.x, xMin, xMax);
            newPos.y = Mathf.Clamp(newPos.y, yMin, yMax);

            transform.position = newPos;
        }
    }
}
