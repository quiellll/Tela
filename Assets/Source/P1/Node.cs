using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class Node{
    //A modifica nodo para que al pasar un objeto encuentre los verices del objeto a usar
    //aqui solo tener los nodos al gameobject y el calculo de las fuerzas
    public Vector3 pos;
    public Vector3 vel;
    public Vector3 force;
    public Vector3 gravity;

    public float mass;
    public bool isFixed;

    public Node(Vector3 position, float mass)
    {
        pos = position;
        vel = Vector3.zero;
        force = Vector3.zero;
        this.mass = mass;
        isFixed = false;
        gravity = new Vector3(0.0f, -9.81f, 0.0f);
    }

    // Use this for initialization
    private void AwakeNode()
    {
        //pos = transform.position;
        //vel = Vector3.zero;
    }

    public void StartNode()
    {
    }

    // Update is called once per frame
    public void UpdateNode( float newMass)
    {
        //transform.position = pos;
        
        //ACTUALIZAR MASA
        mass = newMass;

    }

    public void ComputeForces()
    {
        force += mass * gravity;
    }
}
