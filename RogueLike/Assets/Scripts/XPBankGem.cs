using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPBankGem : MonoBehaviour, iPickUpObject
{
    private float speed = 2.3f;
    private float speed2 = 3;
    Transform targetDestination;
    private float timer = 0.2f;
    float maxDistance = 30f;
    float distance;

    public void OnPickUp(Character character)
    {
        character.AddExperience(GameManager.Instance.xpBank);
        GameManager.Instance.xpBank = 0;
        gameObject.SetActive(false);
        targetDestination = null;
        speed = 2.3f;
        speed2 = 3;
        timer = 0.2f;
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

        distance = Vector3.Distance(transform.position, GameManager.Instance.playerTransform.position);

        if (gameObject.activeSelf && distance > maxDistance)
        {
            gameObject.SetActive(false);
        }
    }

    public void setTargetDestination(Transform destination)
    {
        targetDestination = destination;
    }
}
