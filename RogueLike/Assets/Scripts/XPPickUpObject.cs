using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class XPPickUpObject : NetworkBehaviour, iPickUpObject
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
            DestroyObjectServerRpc();
        }
        else
            GameManager.Instance.xpGemAmount++;
    }

    public void OnPickUp(Character character)
    {
        if (!IsOwner) return;
        GameManager.Instance.AddExperience(xpAmount);
        GameManager.Instance.xpGemAmount--;
        DestroyObjectServerRpc();
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

    public void SetTargetDestination(Transform destination)
    {
        targetDestination = destination;
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyObjectServerRpc()
    {
        Destroy(gameObject);
    }
}
