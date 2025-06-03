using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // The Transform of the player (or object) the camera will follow
    public Transform target;
    // Vertical offset so the camera stays a little above the target's position
    public float offsetY = 2f;

    // Speed at which the camera smoothly follows when the target is falling
    public float fallSmoothSpeed = 2f;

    private void LateUpdate()
    {
        // If there's no assigned target, do nothing
        if (target == null) return;

        // Current camera position
        Vector3 pos = transform.position;
        // Calculate the desired Y position: target's Y plus the offset
        float desiredY = target.position.y + offsetY;

        // If the target has moved up past the current camera height
        if (desiredY > pos.y)
        {
            // Snap the camera instantly to the new height
            transform.position = new Vector3(pos.x, desiredY, pos.z);
        }
        else
        {
            // Otherwise (the target is falling), smoothly interpolate down
            float newY = Mathf.Lerp(pos.y, desiredY, Time.deltaTime * fallSmoothSpeed);
            transform.position = new Vector3(pos.x, newY, pos.z);
        }
    }
}
