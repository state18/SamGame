using UnityEngine;
/// <summary>
/// This abstract class serves as a template for projectile behavior, specifically the
/// OnTriggerEnter2D method since it is stubbed out with some logic here.
/// </summary>
public abstract class Projectile : MonoBehaviour {
    public float Speed;
    public LayerMask CollisionMask;

    public GameObject Owner { get; private set; }
    public Vector2 Direction { get; private set; }
    public Vector2 InitialVelocity { get; private set; }

    // TODO go back and watch tutorial. May have missed something as this method is never called!
    public void Initialize(GameObject owner, Vector2 direction, Vector2 initialVelocity) {
        transform.right = direction;
        Owner = owner;
        Direction = direction;
        InitialVelocity = initialVelocity;
        OnInitialized();
    }

    // Child classes may implement this method, but are not required to do so.
    protected virtual void OnInitialized() {

    }

    public virtual void OnTriggerEnter2D(Collider2D other) {

        // Bitwise logic determines if what this projectile hit is in the LayerMask.
        if ((CollisionMask.value & (1 << other.gameObject.layer)) == 0) {
            OnNotCollideWith(other);
            return;
        }

        // If this projectile collides with the owner, call OnCollideOwner.
        var isOwner = other.gameObject == Owner;
        if (isOwner) {
            OnCollideOwner();
            return;
        }

        // If whatever this projectile collided with implements ITakeDamage interface, invoke OnCollideTakeDamage.
        var takeDamage = (ITakeDamage)other.GetComponent(typeof(ITakeDamage));
        if (takeDamage != null) {
            OnCollideTakeDamage(other, takeDamage);
            return;
        }

        // Call this if what is collided with isn't the owner and doesn't take damage.
        OnCollideOther(other);
    }

    protected virtual void OnNotCollideWith(Collider2D other) {

    }

    protected virtual void OnCollideOwner() {

    }

    protected virtual void OnCollideTakeDamage(Collider2D other, ITakeDamage takeDamage) {

    }

    protected virtual void OnCollideOther(Collider2D other) {

    }
}