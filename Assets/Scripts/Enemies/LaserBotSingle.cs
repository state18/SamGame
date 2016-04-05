using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// This enemy rotates towards the player and fires a laser bolt from a single point.
/// </summary>
public class LaserBotSingle : MonoBehaviour {
    public float rotationSpeed;
    public float shotDelay;
    public float detectionRange;

    public LayerMask player;

    bool playerInRange;
    float shotDelayCounter;

    Transform playerPosition;
    Quaternion defaultRotation;
    AudioSource fireSound;

    public Transform firePoint;
    public Projectile laserPrefab;

    // Use this for initialization
    void Start() {
        playerPosition = FindObjectOfType<Player>().transform;
        defaultRotation = transform.rotation;
        fireSound = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update() {
        if (shotDelayCounter > 0) {
            shotDelayCounter -= Time.deltaTime;
        }
        playerInRange = Physics2D.OverlapCircle(transform.position, detectionRange, player);
        //Debug.Log ("Player in range" + playerInRange);

        if (playerInRange) {

            RotateToFace(playerPosition.position);

            if (shotDelayCounter <= 0)
                Shoot();

        } else if (transform.rotation != defaultRotation) {

            RotateToDefault(defaultRotation);
        }


    }

    /// <summary>
    /// Rotates until the firing point is pointing towards the player.
    /// </summary>
    /// <param name="targetPosition"></param>
	void RotateToFace(Vector3 targetPosition) {
        Vector3 dir = targetPosition - transform.position;
        dir.Normalize();

        float zAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;

        Quaternion desiredRot = Quaternion.Euler(0, 0, zAngle);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRot, rotationSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Rotates back to the initial rotation.
    /// </summary>
    /// <param name="targetPosition"></param>
	void RotateToDefault(Quaternion targetPosition) {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, defaultRotation, rotationSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Fire a projectile in the direction the fire point is facing.
    /// </summary>
	void Shoot() {
        var newLaser = (SimpleProjectile)Instantiate(laserPrefab, firePoint.position, transform.rotation);
        newLaser.Initialize(gameObject, transform.up);
        fireSound.Play();
        shotDelayCounter = shotDelay;
    }
}