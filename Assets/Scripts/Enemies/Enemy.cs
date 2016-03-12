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

    protected void Initialize() {
        // Populate the enemy's AudioSource array and set the clips.
        enemySounds = new AudioSource[1];

        enemySounds[0] = gameObject.AddComponent<AudioSource>();
        enemySounds[0].clip = onHitSoundClip;

        for (int i = 0; i < enemySounds.Length; i++) {
            enemySounds[i].playOnAwake = false;
        }

    }

    public virtual void TakeDamage(int damage, GameObject instigator) {
        Health -= damage;
        if (Health <= 0)
            KillMe();
        else if (enemySounds[0] != null)
            enemySounds[0].Play();
    }

    public virtual void RespawnMe() {
        transform.position = startPosition;
        IsDead = false;
        Health = MaxHealth;
    }

    public virtual void KillMe() {
        IsDead = true;
        if (DestroyedEffect != null)
            Instantiate(DestroyedEffect, transform.position, transform.rotation);

        LevelManager.Instance.AddDeadEnemy(gameObject);
        gameObject.SetActive(false);
    }

    protected virtual void Flip() {
        direction = -direction;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
}

