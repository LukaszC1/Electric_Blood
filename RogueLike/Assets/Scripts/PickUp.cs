using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PickUp script handling collision with player.
/// </summary>
public class PickUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Character c = collision.GetComponent<Character>();
        if (c != null)
        {
            GetComponent<iPickUpObject>().OnPickUp(c);
        }
    }

}
