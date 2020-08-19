using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player")]
    [Range(1,100)][SerializeField] float MovementSpeed = 10f;
    [SerializeField] float Padding = 0.5f;
    [SerializeField] int health = 100;
    [SerializeField] float screenWidthInUnits = 16f;
    [SerializeField] float screenHeightInUnits = 16f;
    [Header("Particle")]
    [SerializeField] GameObject LaserParticle;
    [SerializeField] float particleSpeed = 30f;
    [SerializeField] float projectileFiringPeriod = 0.01f;
    [Header("Death Effects")]
    [SerializeField] GameObject deathVFX;
    [SerializeField] float durationOfExplosion = 1f;
    [SerializeField] AudioClip deathSound;
    [SerializeField] [Range(0,1)] float deathSoundVolume = 0.7f;
    [Header("Shoot Effects")]
    [SerializeField] [Range(0, 1)] float shootSoundVolume = 0.25f;
    [SerializeField] AudioClip shootSound;

    Coroutine firingCoroutine;

    float xMin, xMax;
    float yMin, yMax;

    // Start is called before the first frame update
    void Start()
    {
        SetUpMoveBoundaries();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Fire();
    }

    private void Fire()
    {
        if (Input.GetTouch(0).phase == TouchPhase.Began)
        {
           firingCoroutine =  StartCoroutine(FireContinuosly());
        }
        else if(Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            StopCoroutine(firingCoroutine);
           
        }
    }

    //this is a coroutine
    IEnumerator FireContinuosly()
    {
        while (true)
        {
            GameObject laserParticle = Instantiate(LaserParticle, transform.position, Quaternion.identity) as GameObject;
            laserParticle.GetComponent<Rigidbody2D>().velocity = new Vector2(0, particleSpeed);
             AudioSource.PlayClipAtPoint(shootSound, transform.position, shootSoundVolume);
            yield return new WaitForSeconds(projectileFiringPeriod);
        }
    }

    private void Move()
    {
        var deltaX = Input.GetTouch(0).deltaPosition.x / Screen.width * screenWidthInUnits;
        var deltaY = Input.GetTouch(0).deltaPosition.y / Screen.height * screenHeightInUnits;
        
        var newXPos = Mathf.Clamp(transform.position.x + deltaX * Time.deltaTime * MovementSpeed, xMin, xMax);
        var newYPos = Mathf.Clamp(transform.position.y + deltaY * Time.deltaTime * MovementSpeed, yMin, yMax);
        
        transform.position = new Vector2(newXPos, newYPos);
    }

    private void SetUpMoveBoundaries()
    {
        Camera gameCamera = Camera.main;

        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + Padding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - Padding;
        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + Padding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - Padding;

    }

    private void OnTriggerEnter2D(Collider2D otherGameObject)
    {
        DamageDealer damageDealer = otherGameObject.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer)
            return;
        Hit(damageDealer);
    }

    private void Hit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        damageDealer.Hit();
        if (health <= 0)
            Explode();
    }

    private void Explode()
    {
        FindObjectOfType<Level>().LoadGameOver();
        Destroy(gameObject);
        GameObject explosion = Instantiate(deathVFX, transform.position, transform.rotation);
        AudioSource.PlayClipAtPoint(deathSound, transform.position,deathSoundVolume);
        Destroy(explosion, durationOfExplosion);
    }

    public int Gethealth()
    {
        return health;
    }
}
