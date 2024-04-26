using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring {

    public Node nodeA, nodeB;

    public float Length0;
    public float Length;

    public float stiffness;

    public Spring(Node node1, Node node2, float stiffness)
    {
        nodeA = node1;
        nodeB = node2;
        Length0 = Vector3.Distance(node1.pos, node2.pos);
        this.stiffness = stiffness;
    }

    // Use this for initialization
    public void StartSpring()
    {
        UpdateLength();
        Length0 = Length;
    }

    // Update is called once per frame
    public void UpdateSpring(float newStiffness)
    {

        //actualizar el stiffness
        stiffness = newStiffness;

    }

    public void UpdateLength()
    {
        Length = (nodeA.pos - nodeB.pos).magnitude;

    }

    public void ComputeForces()
    {
        Vector3 u = nodeA.pos - nodeB.pos;
        u.Normalize();
        Vector3 force = -stiffness * (Length - Length0) * u;
        nodeA.force += force;
        nodeB.force -= force;
    }
    //amortiguamiento se crea con la formula. dumping es la constante de amortiguamiento que se crea en computeforce. 
    //Todo esto para node y spring y se suma a la fuerza
}
