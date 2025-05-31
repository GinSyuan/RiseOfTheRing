using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float offsetY = 2f;

    private void LateUpdate()
    {
        if (target != null)
        {
            Vector3 pos = transform.position;
            if (target.position.y + offsetY > pos.y)
            {
                transform.position = new Vector3(pos.x, target.position.y + offsetY, pos.z);
            }
        }
    }
}
