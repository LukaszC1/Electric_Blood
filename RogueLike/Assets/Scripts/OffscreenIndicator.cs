using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OffscreenIndicator : MonoBehaviour
{
    public Transform target;
    public float threshold = 10f;
    private Camera cam;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        var camGameObject = GameObject.FindWithTag("PlayerCamera");

        if (camGameObject != null)
        {
            cam = camGameObject.GetComponent<Camera>();
        }
       
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (target != null && cam != null)
        {
            Vector3 targetDir = target.position - transform.position;
            float distance = targetDir.magnitude;

            if (distance < threshold)
            {
                spriteRenderer.enabled = false;
            }
            else
            {
                Vector3 screenPos = cam.WorldToViewportPoint(target.position);

                if (screenPos.z > 0 && screenPos.x > 0 && screenPos.x < 1 && screenPos.y > 0 && screenPos.y < 1)
                {
                    spriteRenderer.enabled = false;
                }
                else
                {
                    spriteRenderer.enabled = true;
                    Vector3 screenEdge = cam.ViewportToWorldPoint(new Vector3(Mathf.Clamp(screenPos.x, 0.1f, 0.9f), Mathf.Clamp(screenPos.y, 0.1f, 0.9f), cam.nearClipPlane));
                    transform.position = screenEdge;
                    Vector3 dir = target.position - transform.position;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0, 0, angle+30);
                }
            }
        }
        else
            spriteRenderer.enabled = false;
    }
}
