using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRot : MonoBehaviour
{
    public float speed = 1f;

    private void LateUpdate()
    {
        transform.Rotate(0f, speed * Time.deltaTime, 0f);
    }
}
