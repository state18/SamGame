using System;
using UnityEngine;

public class SimpleProjectile : Projectile, ITakeDamage {
    public int Damage;
    public bool knockback;
    public GameObject DestroyedEffect;

    public float TimeToLive;

    public void Update() {
        if ((TimeToLive -= Time.deltaTime) <= 0) {
            DestroyProjectile();
            return;
        }
        // Probably not necessary for my game. Perhaps save this code for an overloaded constructor method.
        // transform.Translate(Direction * ((Mathf.Abs(InitialVelocity.x) + Speed) * Time.deltaTime), Space.World);

        transform.Translate(Direction * Speed * Time.deltaTime, Space.World);
    }
    public void TakeDamage(int damage, GameObject instigator) {
        // Possible area to award points to play for destroying this projectile
        DestroyProjectile();
    }

    protected override void OnCollideOther(Collider2D other) {
        DestroyProjectile();
    }

    protected override void OnCollideTakeDamage(Collider2D other, ITakeDamage takeDamage) {
        takeDamage.TakeDamage(Damage, gameObject);
        DestroyProjectile();
    }

    private void DestroyProjectile() {
        if (DestroyedEffect != null)
            Instantiate(DestroyedEffect, transform.position, transform.rotation);

        Destroy(gameObject);
    }
    // TODO remember to make a Projectiles layer or might run into issues
}