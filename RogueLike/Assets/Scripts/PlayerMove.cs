using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : NetworkBehaviour
{
    Rigidbody2D rgbd2d;

    [HideInInspector] public Vector3 movementVector;
    [HideInInspector] public NetworkVariable<float> lastHorizontalVector = new NetworkVariable<float>(1,NetworkVariableReadPermission.Everyone ,NetworkVariableWritePermission.Owner);
    [HideInInspector] public NetworkVariable<float> lastVerticalVector = new NetworkVariable<float>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [HideInInspector] public NetworkVariable<float> lastHorizontalVectorProjectiles = new NetworkVariable<float>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [HideInInspector] public NetworkVariable<float> lastVerticalVectorProjectiles = new NetworkVariable<float>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


    [SerializeField] float speed = 3f;
    bool facingRight = true;
    public SpriteRenderer sprite;


    Animate animate;
    private void Awake()
    {
        rgbd2d = GetComponent<Rigidbody2D>();
        movementVector = new Vector3();
        animate = GetComponent<Animate>();
       
    }
    private void Start()
    {
       //lastHorizontalVector.Value = 1f; //initial value of the vector (for projectile weapons) -> rhis line generates error related to writing to NV by client
        lastHorizontalVectorProjectiles.Value = 1f;
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

        }

    [ClientRpc]
    private void flipPlayerClientRpc()
    {    
        sprite.flipX = !sprite.flipX;

    }

    [ServerRpc]
    private void flipPlayerServerRpc()
    {
        flipPlayerClientRpc();
    }
}



