using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keeps the GameObject floating above a specified water height.
/// If it dips below, it's pushed back up to the water level.
/// </summary>
public class WaterFloat : MonoBehaviour
{
    [Tooltip("Minimum Y height object should stay above (simulated water surface)")]
    public float WaterHeight = 15.5f;

    void Update()
    {
        // If the object dips below the water height, push it back up
        if (transform.position.y < WaterHeight)
        {
            transform.position = new Vector3(
                transform.position.x,
                WaterHeight,
                transform.position.z
            );
        }
    }
}
