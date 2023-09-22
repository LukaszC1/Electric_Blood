using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    [SerializeField ]float timeToLive = 1f;
    float ttl = 1f;
    TMPro.TextMeshPro textMeshPro;
    AudioSource enemyHit;

    public bool isActive = false;

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshPro>();
        enemyHit = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        ttl = timeToLive;
        enemyHit.Play();
    }

    private void Update()
    {
        if (Time.timeScale == 1)
        {
            ttl -= Time.deltaTime;

            if (ttl > 0.5f)
            {
                transform.localScale = new Vector3(transform.localScale.x + Time.deltaTime, transform.localScale.y + Time.deltaTime, transform.localScale.z);
            }
            else if (ttl > 0)
            {
                transform.localScale = new Vector3(transform.localScale.x - Time.deltaTime, transform.localScale.y - Time.deltaTime, transform.localScale.z);
                textMeshPro.alpha *= 0.988f;
            }
            else if (ttl < 0)
            {
                gameObject.SetActive(false);
            }
        }

    }

}
