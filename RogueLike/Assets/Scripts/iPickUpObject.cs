using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iPickUpObject
{
    public void OnPickUp(Character character);
    public void setTargetDestination(Transform destination);

}
