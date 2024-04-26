using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Fix : MonoBehaviour
{
    [FormerlySerializedAs("_cloth")] [SerializeField] MassSpringCloth cloth;
    private Bounds _fixerBounds;

    private readonly List<Node> _fixedNodes = new List<Node>();
    private Vector3 _lastPosition;
    
    private void Start()
    {
        _fixerBounds = GetComponent<Collider>().bounds;
        
        foreach (var node in cloth.nodes)
        {
            if (!_fixerBounds.Contains(node.pos)) continue;
            
            _fixedNodes.Add(node);
            node.isFixed = true;
        }

        _lastPosition = transform.position;
    }

    private void Update()
    {
        if (!transform.hasChanged) return;


        foreach (var fixedNode in _fixedNodes)
        {
            fixedNode.pos += (transform.position - _lastPosition);
        }

        _lastPosition = transform.position;
        transform.hasChanged = false;
    }
}
