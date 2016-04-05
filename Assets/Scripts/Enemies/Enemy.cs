using System;
using UnityEngine;

/// <summary>
/// The base class for enemies. Bosses will operate independently from this class but every other enemy will inherit from it.
/// </summary>
public abstract class Enemy : MonoBehaviour, ITakeDamage, IRespawnable {

    public float Speed;
    public GameObject DestroyedEffect;
    public float MaxHealth;
    public float Health { get; protected set; }
    public bool IsDead { get; protected set; }

    protected Vector2 startPosition;
    protected Vector2 direction;
    [SerializeField]
    protected AudioClip onHitSoundClip;
    protected AudioSource[] enemySounds;

    /// <summary>
    /// Called to initialize sound components and health (Usually called by derived classes in Start method)
    /// </summary>
    protected void Initialize() {
        // Populate the enemy's AudioSource array and set the clips.
        enemySounds = new AudioSource[1];

        enemySounds[0] = gameObject.AddComponent<AudioSource>();
        enemySounds[0].clip = onHitSoundClip;

        for (int i = 0; i < enemySounds.Length; i++) {
            enemySounds[i].playOnAwake = false;
        }

        Health = MaxHealth;

    }

    /// <summary>
    /// This enemy will receive a deduction in its health, possibly killing it.
    /// </summary>
    /// <param name="damage">amount of damage to receive</param>
    /// <param name="instigator">entity causing the damage</param>
    public virtual void TakeDamage(int damage, GameObject instigator) {
        Health -= damage;
        if (Health <= 0)
            KillMe();
        else if (enemySounds[0] != null)
            enemySounds[0].Play();
    }

    /// <summary>
    /// This enemy will come back to life and be in a similar state as it was upon instantiation
    /// </summary>
    public virtual void RespawnMe() {
        transform.position = startPosition;
        IsDead = false;
        Health = MaxHealth;
    }

    /// <summary>
    /// This enemy dies, disabling the GameObject.
    /// </summary>
    public virtual void KillMe() {
        IsDead = true;
        if (DestroyedEffect != null)
            Instantiate(DestroyedEffect, transform.position, transform.rotation);

        LevelManager.Instance.AddDeadEnemy(gameObject);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Changes the sign on the x component of local scale
    /// </summary>
    protected virtual void Flip() {
        direction = -direction;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
}

