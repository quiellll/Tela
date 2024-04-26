using UnityEngine;

/// <summary>
/// Clase muelle que representa una conexión entre dos nodos de la tela.
/// Contiene la información necesaria para calcular la fuerza de Hooke.
/// </summary>
public class Spring
{
    private readonly Node _nodeA;                                                           // Nodo A del muelle.
    private readonly Node _nodeB;                                                           // Nodo B del muelle.

    private readonly float _length0;                                                        // Longitud de equilibrio del muelle.
    private float _length;                                                                  // Longitud actual del muelle.

    private float _stiffness;                                                               // Constante de rigidez del muelle.

    public Spring(Node node1, Node node2, float stiffness)                                  // Constructor de la clase Spring.
    {
        _nodeA = node1;
        _nodeB = node2;
        _stiffness = stiffness;

        _length0 = Vector3.Distance(node1.Position, node2.Position);
    }

    public void ModifySpringStiffness(float newStiffness)                                   // Modifica la constante de rigidez del muelle.
    {
        _stiffness = newStiffness;
    }

    public void UpdateSpringLength()                                                        // Actualiza la longitud actual del muelle.
    {
        _length = (_nodeA.Position - _nodeB.Position).magnitude;
    }

    public void ComputeSpringForces()                                                       // Calcula la fuerza de los muelles.
    {
        Vector3 u = Vector3.Normalize(_nodeA.Position - _nodeB.Position);                   // Vector unitario que apunta de B a A.
        Vector3 force = -_stiffness * (_length - _length0) * u;                             // Fuerza de Hooke.
        _nodeA.AddSpringForce(force);                                                       // Aplica la fuerza al nodo A.
        _nodeB.AddSpringForce(-force);                                                      // Aplica la fuerza al nodo B.
    }
}
