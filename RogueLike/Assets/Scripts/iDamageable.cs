using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iDamageable
{
    public bool TookDamage { get; set; }
    public void TakeDamage(float damage);
    public void ApplySlow();
}
