using System;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Classing handling player movement.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : NetworkBehaviour
{
    //Public fields
    /// <summary>
    /// Event that is invoked when the player presses the pause button (ESC).
    /// </summary>
    public static event EventHandler OnPauseAction;
    [SerializeField] public float speed = 3f;
    public Rigidbody2D rgbd2d;
    public SpriteRenderer sprite;
    [HideInInspector] public Vector3 movementVector;
    [HideInInspector] public NetworkVariable<float> lastHorizontalVector = new NetworkVariable<float>(1f,NetworkVariableReadPermission.Everyone ,NetworkVariableWritePermission.Owner);
    [HideInInspector] public NetworkVariable<float> lastVerticalVector = new NetworkVariable<float>(1f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [HideInInspector] public NetworkVariable<float> lastHorizontalVectorProjectiles = new NetworkVariable<float>(1f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [HideInInspector] public NetworkVariable<float> lastVerticalVectorProjectiles = new NetworkVariable<float>(1f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    //Private fields
    bool facingRight = true;
    Animate animate;

    private void Awake()
    {
        rgbd2d = GetComponent<Rigidbody2D>();
        movementVector = new Vector3();
        animate = GetComponent<Animate>();
    }
    // Update is called once per frame
    private void Update()
    {
        if (!IsOwner) //only the local player can move this character
        {
            return;
        }

            movementVector.x = Input.GetAxisRaw("Horizontal");
            movementVector.y = Input.GetAxisRaw("Vertical");

            if (movementVector.x != 0)
            {
                lastHorizontalVector.Value = movementVector.x;
                lastHorizontalVectorProjectiles.Value = movementVector.x;

            }
            else if (movementVector.y != 0)
                lastHorizontalVectorProjectiles.Value = 0;


            if (movementVector.x > 0 && !facingRight)
            {                 
                flipPlayerServerRpc();         
                facingRight = !facingRight;
            }
            if (movementVector.x < 0 && facingRight)
            {                       
                flipPlayerServerRpc();               
                facingRight = !facingRight;
            }

            if (movementVector.x == 0 && movementVector.y == 0)
            {
                animate.animator.SetBool("isMoving", false);
            }
            else
            {
                animate.animator.SetBool("isMoving", true);
            }


            if (movementVector.y != 0)
            {
                lastVerticalVector.Value = movementVector.y;
                lastVerticalVectorProjectiles.Value = movementVector.y;

            }
            else if (movementVector.x != 0)
                lastVerticalVectorProjectiles.Value = 0;

            animate.horizontal = movementVector.x;

            movementVector *= speed;
            rgbd2d.velocity = movementVector;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnPauseAction?.Invoke(this, EventArgs.Empty);
        }
    }

    [ClientRpc]
    private void flipPlayerClientRpc()
    {    
     sprite.flipX = !sprite.flipX;

    }

    [ServerRpc(RequireOwnership = false)]
    private void flipPlayerServerRpc()
    {
        flipPlayerClientRpc();
    }
}



