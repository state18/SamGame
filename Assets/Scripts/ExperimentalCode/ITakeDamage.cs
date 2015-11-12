using UnityEngine;
/// <summary>
/// Any entity implementing this will be able to take damage
/// </summary>
public interface ITakeDamage {
    /// <summary>
    /// Damage is dealt to the entity.
    /// </summary>
    /// <param name="damage"> Amount of damage dealt to entity</param>
    /// <param name="instigator"> Who is responsible for this damage?</param>
    void TakeDamage(int damage, GameObject instigator);
}