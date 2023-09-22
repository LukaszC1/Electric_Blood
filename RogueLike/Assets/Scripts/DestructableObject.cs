using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject : MonoBehaviour, iDamageable
{
    private bool tookDamage = false;
    public bool TookDamage { get => tookDamage; set => tookDamage = value; }

    private bool isGettingDestroyed = false;
    private float dissolveAmount = 1;

    [SerializeField] List<DropOnDestroy> dropOnDestroy = new List<DropOnDestroy>();

    public void ApplySlow()
    {
        return;
    }

    private void Update()
    {
        if (isGettingDestroyed)
        {
            dissolveAmount -= Time.deltaTime * 1.5f;
            GetComponent<Renderer>().material.SetFloat("_Dissolve_Amount", dissolveAmount);
            if (dissolveAmount < 0)
            {
                Destroy(gameObject);
            }
        }
    }
    public void TakeDamage(float damage)
    {
        if (!isGettingDestroyed)
        {
            isGettingDestroyed = true;
            GetComponent<BoxCollider2D>().enabled = false;
            foreach (DropOnDestroy dropOnDestroy in dropOnDestroy)
                dropOnDestroy.CheckDrop();  
        }
    }
}
