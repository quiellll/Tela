using UnityEditor.Experimental.GraphView;
using UnityEngine;

/// <summary>
/// Clase nodo que representa una partícula en el sistema de partículas de la tela.
/// </summary>
public class Node{

    public Vector3 Position { get; private set; }                                           // Posición del nodo.
    public Vector3 Velocity { get;  set; }                                                  // Velocidad del nodo.

    private Vector3 _force;                                                                 // Fuerza aplicada al nodo.
    private Vector3 _gravity;                                                               // Gravedad del sistema.
    private Vector3 _windForce;                                                             // Fuerza del viento. 

    private float _damping;                                                                 // Amortiguación del nodo.
    private float _mass;                                                                    // Masa del nodo.

    private bool _isFixed;                                                                  // Flag que indica si el nodo está fijado.

    private MassSpringCloth _cloth;                                                         // Referencia a la tela.

    public Node(MassSpringCloth cloth, Vector3 position, 
                float mass, float damping, Vector3 gravity, Vector3 wind)                   // Constructor de la clase Node.
    {
        Position = position;
        _mass = mass;
        _damping = damping;
        _gravity = gravity;
        _windForce = wind;

        Velocity = Vector3.zero;
        _force = Vector3.zero;
        _isFixed = false;
        _cloth = cloth;
    }

    public void SetNodePosition(Vector3 newPosition)                                        // Establece la posición del nodo.
    {
        Position = newPosition;
    }

    public void FixNode()                                                                   // Fija el nodo.
    {
        _isFixed = true;
    }

    public void ModifyNodeMass(float newMass) { _mass = newMass; }                                              // Modifica la masa del nodo.
    public void ModifyNodeDamping(float newDamping) { _damping = newDamping; }                                      // Modifica la amortiguación del nodo.
    public void ModifyNodeGravity(Vector3 newGravity) { _gravity = newGravity; }                                       // Modifica la gravedad del nodo.
    public void ModifyNodeWind(Vector3 newWind) { _windForce = newWind; }                                            // Modifica la fuerza del viento.

    public void ComputeForces()
    {
        _force = Vector3.zero;                                                              // Se reinicia la fuerza.
        _force += _mass * _gravity;                                                         // Se añade la fuerza de la gravedad.
        if (_cloth.WindEnabled()) _force += _windForce;                                     // Se añade la fuerza del viento si está activada.
        _force += _windForce;
    }

    public void AddSpringForce(Vector3 force)                                               // Añade una fuerza de muelle al nodo.
    {
        _force += force;
    }

    public void NodeSymplecticEulerIntegration(float timeStep)                              // Método de integración de Euler.
    {
        if (_isFixed) return;

        Vector3 dampingForce = -_damping * Velocity;                                        // Fuerza de amortiguación.
        Vector3 totalForce = _force + dampingForce;                                         // Fuerza total.

        Velocity += timeStep * totalForce / _mass;
        Position += timeStep * Velocity;
    }
}
