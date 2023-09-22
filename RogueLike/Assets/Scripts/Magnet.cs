using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    private float Size;

    Character character;

    private void Awake()
    {
        character = GetComponentInParent<Character>();
        Size = character.magnetSize;
    }

    private void Update()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, Size);
        for (int i = 0; i < colliders.Length; i++)
        {
            iPickUpObject e = colliders[i].GetComponent<iPickUpObject>();
            if (e != null)
            {
                e.setTargetDestination(transform);
            }
        }
    }

    public void LevelUpUpdate()
    {
        Size = character.magnetSize;
    }
}
