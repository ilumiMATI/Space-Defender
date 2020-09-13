using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    // configuration parameters
    [Header("Player")]
    [SerializeField] float health = 200f;
    [SerializeField] float moveSpeed = 8f;
    [SerializeField] float padding = 1f;

    [Header("SFX")]
    [SerializeField] AudioClip shotSFX;
    [SerializeField] [Range(0f, 1f)] float shotVolume = 1f;
    [SerializeField] AudioClip hitSFX;
    [SerializeField] [Range(0f, 1f)] float hitVolume = 1f;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] [Range(0f, 1f)] float deathVolume = 1f;

    [Header("VFX")]
    [SerializeField] GameObject explosionVFX;
    [SerializeField] float timeDestroyVFX = 1.5f;
    [SerializeField] float timeToDestroyObject = 0.15f;

    [Header("Projectile")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] float projectileFiringPeriod = .1f;
    [SerializeField] float timeLastShot;

    [Header("Touch Controls")]
    [SerializeField] float sensitivity = 1.3f;

    GameSession theGameSession;

    // Touch controll
    TouchManager theTouchManager;
    TouchTracker moveTracker = null;
    TouchTracker shotTracker = null;
    Vector2 destination;
    Vector2 newPos;
    Vector2 startTouchPos;
    Vector2 startObjectPos;
    Vector2 deltaPos;
    Coroutine firingCoroutine;

    float xMin;
    float xMax;
    float yMin;
    float yMax;
    float maxHealth;

    // Start is called before the first frame update
    void Start()
    {
        // Init
        maxHealth = health;
        SetUpMoveBoundaries();
        // Finding
        theGameSession = FindObjectOfType<GameSession>();
        theTouchManager = FindObjectOfType<TouchManager>();
        // After
        theGameSession.UpdateHealth(Convert.ToInt32(health));
        // touch manager
        theTouchManager.OnTrackerCreated = OnTrackerCreated;
        theTouchManager.OnTrackerLost = OnTrackerLost;
    }
    
    private void OnTrackerCreated()
    {
        if (moveTracker == null)
        {
            moveTracker = theTouchManager.GetNewTouchTracker("move");
            moveTracker.OnBegan = (Touch touch, ref Player player) => 
            {
                player.SetupTouchOffset(touch);
            };
                
            moveTracker.OnStationary = moveTracker.OnMoved = (Touch touch, ref Player player) =>
            {
                player.CalculateDestination(touch.position);
            };

            moveTracker.OnFrame = (Touch touch, ref Player player) =>
            {
                player.Move();
            };
        }
        else if (shotTracker == null)
        {
            shotTracker = theTouchManager.GetNewTouchTracker("shot");
            shotTracker.OnBegan = (Touch touch, ref Player player) =>
            {
                player.StartShooting();
            };
            shotTracker.OnEnded = (Touch touch, ref Player player) =>
            {
                player.StopShooting();
            };
        }
    }
    private void OnTrackerLost(string trackerName)
    {
        switch (trackerName)
        {
            case "move":
                moveTracker = null;
                break;
            case "shot":
                shotTracker = null;
                break;
        }
    }
    public void SetupTouchOffset(Touch touch)
    {
        newPos = transform.position;
        startTouchPos = Camera.main.ScreenToWorldPoint(touch.position);
        startObjectPos = transform.position;
    }

    public void CalculateDestination(Vector2 position)
    {
        deltaPos = (Vector2)Camera.main.ScreenToWorldPoint(position) - startTouchPos;
        destination = startObjectPos + deltaPos * sensitivity;
    }

    public void Move()
    {
        newPos = Vector2.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
        newPos.x = Mathf.Clamp(newPos.x, xMin, xMax);
        newPos.y = Mathf.Clamp(newPos.y, yMin, yMax);

        transform.position = newPos;
    }
    public void StartShooting()
    {
        float deltaTimeLastShot = Time.time - timeLastShot;
        if(deltaTimeLastShot < projectileFiringPeriod)
        {
            float fireDelay = projectileFiringPeriod - deltaTimeLastShot;
            firingCoroutine = StartCoroutine(FireContinuously(fireDelay));
            return;
        }

        firingCoroutine = StartCoroutine(FireContinuously(0f));
    }
    public void StopShooting()
    {
        StopCoroutine(firingCoroutine);
    }
    void ResetTrackers()
    {
        theTouchManager.OnTrackerCreated = null;
        theTouchManager.OnTrackerLost = null;
        if(moveTracker != null)
        {
            moveTracker.OnBegan = null;
            moveTracker.OnFrame = null;
            moveTracker.OnMoved = null;
            moveTracker.OnStationary = null;
        }
        if(shotTracker != null)
        {
            shotTracker.OnBegan = null;
            shotTracker.OnEnded = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        if (hitSFX) { AudioSource.PlayClipAtPoint(hitSFX, Camera.main.transform.position, hitVolume); }
        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        theGameSession.UpdateHealth( Convert.ToInt32( Mathf.Clamp(health, 0, maxHealth) ) );
        damageDealer.Hit();
        if (health <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        ResetTrackers();
        HandleVFX();
        HandleDestroying();
        if (deathSFX) { AudioSource.PlayClipAtPoint(deathSFX, Camera.main.transform.position, deathVolume); }
        FindObjectOfType<SceneController>().LoadGameOverScene();
    }
    private void HandleDestroying()
    {
        Destroy(gameObject, timeToDestroyObject);
        enabled = false;
    }
    private void HandleVFX()
    {
        GameObject spawnedExplosionVFX = Instantiate(
            explosionVFX,
            new Vector3(transform.position.x, transform.position.y, -1f),
            Quaternion.identity) as GameObject;
        Destroy(spawnedExplosionVFX, timeDestroyVFX);
    }
    private IEnumerator FireContinuously(float delayInSeconds)
    {
        yield return new WaitForSeconds(delayInSeconds);
        for(;;)
        {
            GameObject projectile = Instantiate(
                projectilePrefab, 
                transform.position, 
                Quaternion.identity) as GameObject;
            projectile.GetComponent<Rigidbody2D>().velocity = new Vector2(0, projectileSpeed);
            timeLastShot = Time.timeSinceLevelLoad;

            if (shotSFX) { AudioSource.PlayClipAtPoint(shotSFX, Camera.main.transform.position, shotVolume); }

            yield return new WaitForSeconds(projectileFiringPeriod);
        }
    }
    private void SetUpMoveBoundaries()
    {
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + padding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - padding;
        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + padding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - padding;
    }
}
