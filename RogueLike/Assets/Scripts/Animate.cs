using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Class containing the animator attached to the character prefabs.
/// </summary>
public class Animate : MonoBehaviour
{
    public Animator animator;
    public float horizontal;

    private void Awake()
    {
       animator = GetComponentInChildren<Animator>(); 
    }

    private void Update()
    {
        animator.SetFloat("Horizontal", horizontal);
    }
}
