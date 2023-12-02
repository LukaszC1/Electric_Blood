using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class XPBankGem : NetworkBehaviour, iPickUpObject
{
    private float speed = 2.3f;
    private float speed2 = 3;
    Transform targetDestination;
    private float timer = 0.2f;
    float maxDistance = 30f;
    float distance;


    public void OnPickUp(Character character)
    {
        gameObject.SetActive(false);
        targetDestination = null;
        speed = 2.3f;
        speed2 = 3;
        timer = 0.2f;
        if (!IsOwner) return;
        GameManager.Instance.AddExperience(GameManager.Instance.xpBank);
        GameManager.Instance.xpBank = 0;
    }
    private void Update()
    {
        if (targetDestination != null && Time.timeScale == 1)
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

    private void FixedUpdate()
    {
        bool disableGem = true;
        foreach (var player in GameManager.Instance.listOfPlayers)
        {
            distance = Vector3.Distance(player.Value.position, transform.position);

            if (distance < maxDistance)
            {
                disableGem = false;
            }
        }
        if (gameObject.activeSelf && disableGem)
        {
            gameObject.SetActive(false);
        }
    }

    public void SetTargetDestination(Transform destination)
    {
        targetDestination = destination;
    }
}
