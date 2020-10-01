using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionMarker : MonoBehaviour
{
    public float lifetime = 1.0f;

    void Start ()
    {
        Destroy(gameObject, lifetime);
    }
}