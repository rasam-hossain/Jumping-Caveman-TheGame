using UnityEngine;
using System.Collections;

public class MoonMover : MonoBehaviour
{
    float speed = -0.05f;    // the cloud will be moving backward at a slow speed !
    void FixedUpdate()
    {
        Vector3 pos = transform.position;
        pos.x += speed * Time.deltaTime;
        transform.position = pos;
    }
}

