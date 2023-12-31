using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/// <summary>
/// THe XP pick up object is a script that is attached to the XP pick up object prefab.
/// </summary>
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

    /// <summary>
    /// Method that is called when the player picks up the XP pick up object.
    /// </summary>
    /// <param name="character"></param>
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

    /// <summary>
    /// Method that sets the target destination of the XP pick up object.
    /// </summary>
    /// <param name="destination"></param>
    public void SetTargetDestination(Transform destination)
    {
        targetDestination = destination;
    }

    /// <summary>
    /// ServerRpc method that destroys the XP pick up object.
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    private void DestroyObjectServerRpc()
    {
        Destroy(gameObject);
    }
}
