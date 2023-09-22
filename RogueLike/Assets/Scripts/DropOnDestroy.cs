using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropOnDestroy : MonoBehaviour
{
    [SerializeField] GameObject droppedItem;
    [SerializeField] [Range(0,1)] float chanceToDrop=1f;
    public bool quitting = false;

    private void OnApplicationQuit()
    {
        quitting = true;       
    }

    public void CheckDrop()
    {
        if (quitting)
        {
            return;
        }

        if (Random.value < chanceToDrop)
        {
            Transform t = Instantiate(droppedItem).transform;
            t.position = transform.position;
        }
    }

  

}
