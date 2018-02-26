using UnityEngine;
using System.Collections;

public class CloudMover : MonoBehaviour
{
    float speed = -0.2f;    // the cloud will be moving backward at a slow speed !
    void FixedUpdate()
    {
        Vector3 pos = transform.position;
        pos.x += speed * Time.deltaTime;
        transform.position = pos;
    }
}

