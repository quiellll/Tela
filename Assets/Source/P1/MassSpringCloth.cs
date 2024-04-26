using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Basic physics manager capable of simulating a given ISimulable
/// implementation using diverse integration methods: explicit,
/// implicit, Verlet and semi-implicit.
/// </summary>
public class MassSpringCloth : MonoBehaviour 
{
    /// <summary>
    /// Default constructor. Zero all. 
    /// </summary>
    public MassSpringCloth()
    {
        this.Paused = true;
        this.TimeStep = 0.01f;
        this.Gravity = new Vector3(0.0f, -9.81f, 0.0f);
        this.IntegrationMethod = Integration.Symplectic;
        this.IntegrationMethod = Integration.Explicit;
    }

    public enum Integration
    {
        Explicit = 0,
        Symplectic = 1,
    };

    #region InEditorVariables

    public bool Paused;
    public float TimeStep;
    public Vector3 Gravity;
    public Integration IntegrationMethod;
    public List<Node> nodes;
    public List<Spring> springs;

    public float Mass;
    public float Stiffness;

    #endregion

    #region OtherVariables

    private Mesh mesh;
    private Vector3[] vertices;
    
    #endregion

    #region MonoBehaviour

    public void Awake()
    {
        InitializeFromMesh();   // Inicializa los nodos y muelles desde el mesh

    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
            this.Paused = !this.Paused;

        /*foreach (Spring spring in springs)
        {
            spring.UpdateSpring(Stiffness);
        }

        foreach (Node node in nodes)
        {
            node.UpdateNode(Mass);
        }*/
    }

    public void FixedUpdate()
    {
        if (this.Paused)
            return; // Not simulating

        // Select integration method
        switch (this.IntegrationMethod)
        {
            case Integration.Explicit:
                this.stepExplicit();
                break;
            case Integration.Symplectic:
                this.stepSymplectic();
                break;
            default:
                throw new System.Exception("[ERROR] Should never happen!");
        }

    }

    #endregion

    private void InitializeFromMesh()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        nodes = new List<Node>();
        springs = new List<Spring>();

        // Inicializamos nodos
        for (int i = 0; i < vertices.Length; i++)
        {
            Node newNode = new Node(transform.TransformPoint(vertices[i]), Mass);
            nodes.Add(newNode);
        }

        // Inicializamos muelles
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int index1 = triangles[i];
            int index2 = triangles[i + 1];
            int index3 = triangles[i + 2];

            Spring newSpring1 = new Spring(nodes[index1], nodes[index2], Stiffness);
            Spring newSpring2 = new Spring(nodes[index2], nodes[index3], Stiffness);
            Spring newSpring3 = new Spring(nodes[index3], nodes[index1], Stiffness);

            springs.Add(newSpring1);
            springs.Add(newSpring2);
            springs.Add(newSpring3);
        }
        nodes[0].isFixed = true;
    }



    private void stepExplicit()
    {
        foreach (Node node in nodes)
        {
            node.force = Vector3.zero;
            node.ComputeForces();
        }

        foreach (Spring spring in springs)
        {
            spring.ComputeForces();
        }

        foreach (Node node in nodes)
        {
            if (!node.isFixed)
            {
                // // Integracion explicita de Euler 
                node.pos += TimeStep * node.vel;
                node.vel += TimeStep / node.mass * node.force;
            }

            foreach (Spring spring in springs)
            {
                spring.UpdateLength();
            }
        }
    }

    private void stepSymplectic()
    {
        foreach (Node node in nodes)
        {
            node.force = Vector3.zero;
            node.ComputeForces();
        }

        foreach (Spring spring in springs)
        {
            spring.ComputeForces();
            //spring.UpdateStiffness(Stiffness);
        }

        for(int i = 0; i < nodes.Count; i++)
        {
            var node = nodes[i];
            if (!node.isFixed)
            {
                // Integracion simplectica de Euler
                node.vel += TimeStep / node.mass * node.force;
                node.pos += TimeStep * node.vel;
            }
            vertices[i] = transform.InverseTransformPoint(nodes[i].pos);
            
        }

        mesh.vertices = vertices;

        foreach (Spring spring in springs)
        {
            spring.UpdateLength();
        }
    }

}
