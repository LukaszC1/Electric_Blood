using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Magnet is a script that is attached to the player and is used to attract coins and other objects to the player.
/// </summary>
public class Magnet : NetworkBehaviour
{
    private float Size;
    Character character;

    private void Awake()
    {
        character = GetComponentInParent<Character>();
        Size = character.magnetSize.Value;
    }

    private void Update()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, Size);
        for (int i = 0; i < colliders.Length; i++)
        {
            iPickUpObject e = colliders[i].GetComponent<iPickUpObject>();
            if (e != null)
            {
                e.SetTargetDestination(transform);
            }
        }
    }

    /// <summary>
    /// Function which levels up the magnet size.
    /// </summary>
    public void LevelUpUpdate()
    {
        Size = character.magnetSize.Value;
    }
}
