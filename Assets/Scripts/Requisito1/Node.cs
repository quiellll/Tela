using UnityEditor.Experimental.GraphView;
using UnityEngine;

/// <summary>
/// Clase nodo que representa una partícula en el sistema de partículas de la tela.
/// </summary>
public class Node{

    public Vector3 Position { get; private set; }                                           // Posición del nodo.

    private Vector3 _velocity;                                                              // Velocidad del nodo.
    private Vector3 _force;                                                                 // Fuerza aplicada al nodo.
    private Vector3 _gravity;                                                               // Gravedad del sistema.

    private float _mass;                                                                    // Masa del nodo.

    private bool _isFixed;                                                                  // Flag que indica si el nodo está fijado.

    public Node(Vector3 position, float mass, Vector3 gravity)                              // Constructor de la clase Node.
    {
        Position = position;
        _velocity = Vector3.zero;
        _force = Vector3.zero;
        _gravity = gravity;
        _mass = mass;
        _isFixed = false;
    }

    public void SetNodePosition(Vector3 newPosition)                                        // Establece la posición del nodo.
    {
        Position = newPosition;
    }

    public void FixNode()                                                                   // Fija el nodo.
    {
        _isFixed = true;
    }
                                                                                                    
    public void ModifyNodeMass(float newMass)                                               // Modifica la masa del nodo.
    {
        _mass = newMass;
    }

    public void ComputeForces()
    {
        _force = Vector3.zero;                                                              // Se reinicia la fuerza.
        _force += _mass * _gravity;                                                         // Se añade la fuerza de la gravedad.
    }

    public void AddSpringForce(Vector3 force)                                               // Añade una fuerza de muelle al nodo.
    {
        _force += force;
    }

    public void NodeSymplecticEulerIntegration(float timeStep)                              // Método de integración de Euler.
    {
        if (_isFixed) return;

        _velocity += timeStep * _force / _mass;
        Position += timeStep * _velocity;
    }
}
