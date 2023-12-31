using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Inteface for objects that can be picked up by the character
/// </summary>
public interface iPickUpObject
{
    /// <summary>
    /// Method to be called when the character picks up the object
    /// </summary>
    /// <param name="character"></param>
    public void OnPickUp(Character character);

    /// <summary>
    /// Method for setting the destination of the object.
    /// </summary>
    /// <param name="destination"></param>
    public void SetTargetDestination(Transform destination);

}
