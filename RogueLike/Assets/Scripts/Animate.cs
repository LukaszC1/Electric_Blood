using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Animate : NetworkBehaviour
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
