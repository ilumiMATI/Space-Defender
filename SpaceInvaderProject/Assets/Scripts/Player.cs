using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 8f;

    // cache
    Camera theCamera;

    // Start is called before the first frame update
    void Start()
    {
        theCamera = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        SmoothMove();
        //TouchMove();
    }

    private void RapidMove()
    {
        var deltaX = Input.GetAxisRaw("Horizontal");
        var newXPos = transform.position.x + deltaX * Time.deltaTime * moveSpeed;
        var deltaY = Input.GetAxisRaw("Vertical");
        var newYPos = transform.position.y + deltaY * Time.deltaTime * moveSpeed;
        transform.position = new Vector2(newXPos, newYPos);
    }
    private void SmoothMove()
    {
        var deltaX = Input.GetAxis("Horizontal");
        var newXPos = transform.position.x + deltaX * Time.deltaTime * moveSpeed;
        var deltaY = Input.GetAxis("Vertical");
        var newYPos = transform.position.y + deltaY * Time.deltaTime * moveSpeed;
        transform.position = new Vector2(newXPos, newYPos);
    }

    private void TouchMove()
    {
        if (Input.touchCount > 0)
        {
            var touchPos = Input.GetTouch(0).position;
            touchPos = theCamera.ScreenToWorldPoint(touchPos);
            Vector2 deltaPos = touchPos - (Vector2)transform.position;
            deltaPos.Normalize();
            Vector2 newPos = (Vector2)transform.position + deltaPos * Time.deltaTime * moveSpeed;
            transform.position = newPos;
        }
    }
}
