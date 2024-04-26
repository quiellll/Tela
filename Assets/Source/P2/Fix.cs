using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fix : MonoBehaviour
{
    // Possibilities of the Fixer
    void Start()
    {
        // Test if pos is inside
        Vector3 pos = new Vector3(0.0f, 0.0f, 0.0f);
        Bounds bounds = GetComponent<Collider>().bounds;
        bool isInside = bounds.Contains(pos);

        // Transform points with fixer
        Vector3 localPos = transform.InverseTransformPoint(pos);
        Vector3 globalPos = transform.TransformPoint(localPos);

    }
}
