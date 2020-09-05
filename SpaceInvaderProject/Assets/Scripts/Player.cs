using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    Coroutine firingCoroutine;
    float xMin;
    float xMax;
    float yMin;
    float yMax;

    // Start is called before the first frame update
    void Start()
    {
        SetUpMoveBoundaries();
    }

    // Update is called once per frame
    void Update()
    {
        // Computer controls
        RapidMove();
        Fire();
        // Touch controls
        //TouchMove();
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
        damageDealer.Hit();
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GameObject spawnedExplosionVFX = Instantiate(
                        explosionVFX,
                        new Vector3(transform.position.x, transform.position.y, -1f),
                        Quaternion.identity) as GameObject;
        Destroy(spawnedExplosionVFX, timeDestroyVFX);
        Destroy(gameObject, timeToDestroyObject);

        if(deathSFX) { AudioSource.PlayClipAtPoint(deathSFX, Camera.main.transform.position, deathVolume); }
    }

    private void Fire()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            if (Time.timeSinceLevelLoad - timeLastShot > projectileFiringPeriod)
            {
                firingCoroutine = StartCoroutine(FireContinuously());
            }
        }
        else if(Input.GetButtonUp("Fire1"))
        {
            StopCoroutine(firingCoroutine);
        }
    }

    private IEnumerator FireContinuously()
    {
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
    private void SetUpMoveBoundaries()
    {
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + padding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - padding;
        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + padding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - padding;
    }
}
