using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rgbd2d;

    [HideInInspector] public Vector3 movementVector;
    [HideInInspector] public float lastHorizontalVector;
    [HideInInspector] public float lastVerticalVector;
    [HideInInspector] public float lastHorizontalVectorProjectiles;
    [HideInInspector] public float lastVerticalVectorProjectiles;


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
        lastHorizontalVector = 1f; //initial value of the vector (for projectile weapons)
        lastHorizontalVectorProjectiles = 1f;
    }

    // Update is called once per frame
    void Update()
    {
     
        movementVector.x = Input.GetAxisRaw("Horizontal");
        movementVector.y = Input.GetAxisRaw("Vertical");

        if(movementVector.x != 0)
        {
            lastHorizontalVector = movementVector.x;
            lastHorizontalVectorProjectiles = movementVector.x;

        }
       else if (movementVector.y != 0)
            lastHorizontalVectorProjectiles = 0;


        if (movementVector.x > 0 && !facingRight)
        {
            sprite.flipX = false;
            facingRight = !facingRight;
        }
        if (movementVector.x < 0 && facingRight)
        {
            sprite.flipX = true;
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
            lastVerticalVector = movementVector.y;
            lastVerticalVectorProjectiles = movementVector.y;

        }
        else if (movementVector.x != 0)
            lastVerticalVectorProjectiles = 0;


        animate.horizontal = movementVector.x;

        movementVector *= speed;
        rgbd2d.velocity = movementVector;

    }

}



