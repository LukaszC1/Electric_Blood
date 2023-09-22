using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPickUpObject : MonoBehaviour, iPickUpObject
{
    [SerializeField] int healAmount;
    private float speed = 3;
    private float speed2 = 3;
    private float timer = 0.2f;
    Transform targetDestination;
    public void OnPickUp(Character character)
    {
        character.Heal(healAmount);
        Destroy(gameObject);
    }

    private void Update()
    {
        if (targetDestination != null)
        {
            timer -= Time.deltaTime;
            if (timer >= 0)
            {
                Vector3 direction = (targetDestination.position - transform.position).normalized;
                transform.position -= speed2 * Time.deltaTime * direction.normalized;
                speed2 *= 0.99f;
            }
            else
            {
                Vector3 direction = (targetDestination.position - transform.position).normalized;
                transform.position += speed * Time.deltaTime * direction.normalized;
                speed *= 1.001f;
            }
        }
    }

    public void setTargetDestination(Transform destination)
    {
        targetDestination = destination;
    }

}
