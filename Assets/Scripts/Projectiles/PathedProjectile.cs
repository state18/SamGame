﻿using System;
using UnityEngine;

public class PathedProjectile : MonoBehaviour, ITakeDamage {
    private Transform _destination;
    private float _speed;

    public GameObject DestroyEffect;

    public void Initialize(Transform destination, float speed) {
        _destination = destination;
        _speed = speed;
    }

    public void Update() {
        transform.position = Vector3.MoveTowards(transform.position, _destination.position, Time.deltaTime * _speed);

        var distanceSquared = (_destination.transform.position - transform.position).sqrMagnitude;
        if (distanceSquared > .01f * .01f)
            return;

        // Is there a particle effect upon particle death?
        if (DestroyEffect != null)
            Instantiate(DestroyEffect, transform.position, transform.rotation);

        Destroy(gameObject);
    }

    public void TakeDamage(int damage, GameObject instigator) {
        if (DestroyEffect != null)
            Instantiate(DestroyEffect, transform.position, transform.rotation);

        Destroy(gameObject);

        // Could award points to player here if their fired projectile destroys this projectile.
    }
}
