using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumPickup : MonoBehaviour, iPickUpObject
{
    private float speed = 2.3f;
    private float speed2 = 3;
    Transform targetDestination;
    private float timer = 0.2f;

    public void OnPickUp(Character character)
    {
        StartCoroutine(magnetIncrease(character));
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

    private IEnumerator magnetIncrease(Character character)
    {
        character.magnetSize += 10000;
        character.magnet.LevelUpUpdate();
        yield return new WaitForSeconds(0.01f);
        character.magnetSize -= 10000;
        character.magnet.LevelUpUpdate();
        Destroy(gameObject);
    }
}
