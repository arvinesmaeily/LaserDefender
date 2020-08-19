using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] float health = 100;
    [SerializeField] int points = 150;
    float shotCounter;
    [SerializeField] float minAttackDelay = 0.2f;
    [SerializeField] float maxAttackDelay = 3f;
    [Header("Particle")]
    [SerializeField] GameObject LaserParticle;
    [SerializeField] float particleSpeed = 10f;
    [Header("Death Effects")]
    [SerializeField] GameObject deathVFX;
    [SerializeField] float durationOfExplosion = 1f;
    [SerializeField] AudioClip deathSound;
    [SerializeField] [Range(0, 1)] float deathSoundVolume = 0.7f;
    [Header("Shoot Effects")]
    [SerializeField] [Range(0, 1)] float shootSoundVolume = 0.25f;
    [SerializeField] AudioClip shootSound;

    // Start is called before the first frame update
    void Start()
    {
        shotCounter = UnityEngine.Random.Range(minAttackDelay, maxAttackDelay);
    }

    // Update is called once per frame
    void Update()
    {
        ShootingProcess();
    }

    private void ShootingProcess()
    {
        shotCounter -= Time.deltaTime;
        if (shotCounter <= 0f)
        {
            Fire();
            shotCounter = UnityEngine.Random.Range(minAttackDelay, maxAttackDelay);

        }
    }

    private void Fire()
    {
        GameObject laserParticle = Instantiate(LaserParticle, transform.position, Quaternion.identity) as GameObject;
        laserParticle.GetComponent<Rigidbody2D>().velocity = new Vector2(0, - particleSpeed);
        AudioSource.PlayClipAtPoint(shootSound, transform.position, shootSoundVolume);
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
        Destroy(gameObject);
        FindObjectOfType<GameSession>().AddToScore(points);
        GameObject explosion = Instantiate(deathVFX, transform.position, transform.rotation);
        AudioSource.PlayClipAtPoint(deathSound, transform.position, deathSoundVolume);
        Destroy(explosion, durationOfExplosion);
    }
}
