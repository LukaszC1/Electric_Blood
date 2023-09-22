using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPPickUpObject : MonoBehaviour, iPickUpObject
{
    [SerializeField] float xpAmount;
    private float speed = 2.3f;
    private float speed2 = 3;
    Transform targetDestination;
    private float timer = 0.2f;


    private void Start()
    {
        if (GameManager.Instance.xpGemAmount >= 400)
        {
            GameManager.Instance.xpBank += xpAmount;
            Destroy(gameObject);
        }
        else
            GameManager.Instance.xpGemAmount++;
    }

    public void OnPickUp(Character character)
    {
        character.AddExperience(xpAmount);
        GameManager.Instance.xpGemAmount--;
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
