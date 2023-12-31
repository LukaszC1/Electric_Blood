using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Trigger used to generate more map chunks.
/// </summary>
public class MapTrigger : MonoBehaviour
{
    /// <summary>
    /// Map reference.
    /// </summary>
    public GameObject targetMap;

    MapController controller;
  
    private void Start()
    {
        controller = FindObjectOfType<MapController>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (controller == null)
            controller = FindObjectOfType<MapController>();
        else if (collision.CompareTag("Player"))
        {
            controller.currentChunk = targetMap;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(controller.currentChunk == targetMap) {controller.currentChunk = null;} 
        }
    }
}
