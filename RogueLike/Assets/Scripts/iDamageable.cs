using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for damageable objects.
/// </summary>
public interface iDamageable
{
    /// <summary>
    /// Determines if the object has taken damage.
    /// </summary>
    public bool TookDamage { get; set; }

    /// <summary>
    /// Method for taking damage.
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(float damage);

    /// <summary>
    /// Method for applying a slow effect.
    /// </summary>
    public void ApplySlow();
}
