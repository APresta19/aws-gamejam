using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target;

    public Vector3 offset;
    private Vector3 velocity = Vector3.zero;
    public float speed;
    private void LateUpdate()
    {
        Vector3 desiredPos = target.position + offset;
        Vector3 smoothedPos = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, speed);
        transform.position = smoothedPos;
    }
}
