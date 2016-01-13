using System;
using UnityEngine;

/// <summary>
/// The base class for enemies. Bosses will operate independently from this class but every other enemy will inherit from it.
/// </summary>
public abstract class Enemy : MonoBehaviour, ITakeDamage, IRespawnable {

    public float Speed;
    //public Projectile Projectile;
    public GameObject DestroyedEffect;
    public float MaxHealth;
    public float Health { get; protected set; }
    public bool IsDead { get; protected set; }

    protected Vector2 startPosition;
    protected Vector2 direction;
    protected AudioSource onHitSound;

    public virtual void TakeDamage(int damage, GameObject instigator) {
        Health -= damage;
        if (onHitSound != null)
            onHitSound.Play();
        if (Health <= 0)
            KillMe();
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

