using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRelativePosition : MonoBehaviour
{
    private Vector3 offset;

    private void Awake()
    {
        offset = transform.position - transform.parent.position;
    }

    private void Update()
    {
        FixPosition();
    }

    private void FixedUpdate()
    {
        FixPosition();
    }

    private void LateUpdate()
    {
        FixPosition();
    }

    private void FixPosition()
    {
        transform.position = transform.parent.position + offset;
    }
}
