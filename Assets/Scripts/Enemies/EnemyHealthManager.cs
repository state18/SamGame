using System;
using UnityEngine;

/// <summary>
/// Acts as a general use Health Manager for most enemies. Any enemy with this component can take damage, respawn, and be killed.
/// </summary>
public class EnemyHealthManager : MonoBehaviour, ITakeDamage, IRespawnable {

    public float MaxHealth = 1;

    public float Health { get; private set; }
    public bool IsDead { get; private set; }

    private AudioSource OnHitEffect;
    public GameObject DestroyedEffect;

    private Vector3 startPosition;

    void Start() {
        OnHitEffect = GetComponent<AudioSource>();
        startPosition = transform.position;
        Health = MaxHealth;
    }
    // TODO find a way to get necessary information to re-initialize.
    public void RespawnMe() {
        transform.position = startPosition;
        IsDead = false;
        Health = MaxHealth;
    }

    public void TakeDamage(int damage, GameObject instigator) {
        // Possibly add points here, if killed by the player, possibly through a separate score manager
        Health -= damage;
        if (Health <= 0)
            KillMe();
        else
            OnHitEffect.Play();
    }

    public void KillMe() {
        IsDead = true;
        if (DestroyedEffect != null)
            Instantiate(DestroyedEffect, transform.position, transform.rotation);

        LevelManager.Instance.AddDeadEnemy(gameObject);
        gameObject.SetActive(false);
    }
}