using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Fija los nodos de una tela que se encuentren dentro de un objeto con un collider. 
/// Para vincular este script, se debe colocar el objeto que contiene este script como hijo
/// del objeto que contenga el componente MassSpringCloth.
/// </summary>
public class Fixer : MonoBehaviour
{    
    private MassSpringCloth _cloth;                                                         // Tela a la que se le aplicar� la fijaci�n.

    private Bounds _fixerBounds;                                                            // �rea de fijaci�n, coincidente con el Collider del objeto que contiene este script.
    private Vector3 _fixerLastPosition;                                                     // Almacena la �ltima posici�n del Fixer.        

    private readonly List<Node> _fixedNodes = new();                                        // Lista de nodos fijados.
    
    private void Start()
    {
        if (GetComponentInParent<MassSpringCloth>() != null)                                // Si no se encuentra un componente MassSpringCloth3 en el padre, se omite.
            _cloth = GetComponentInParent<MassSpringCloth>();                               // Se obtienen el componente padre de tipo MassSpringCloth.
        
        _fixerBounds = GetComponent<Collider>().bounds;                                     // Se obtienen los l�mites del Collider del objeto que contiene este script.
        
        foreach (var node in _cloth.NodeList)                                               // Por cada nodo en la tela.
        {
            if (!_fixerBounds.Contains(node.Position)) continue;                            // Si no se encuentra dentro de los l�mites del Collider, se omite.
                                                                                            
            _fixedNodes.Add(node);                                                          // Si se encuentra dentro de los l�mites del Collider, se a�ade a la lista.
            node.FixNode();                                                                 // Y se fija el nodo.
        }

        _fixerLastPosition = transform.position;                                            // Se almacena la posici�n inicial del Fixer.
    }

    private void Update()
    {
        if (!transform.hasChanged) return;                                                  // Si el objeto que contiene este script no ha cambiado de posici�n, se omite.


        foreach (var fixedNode in _fixedNodes)                                              // Por cada nodo fijado.
        {   
            Vector3 movement = (transform.position - _fixerLastPosition);                   // Se calcula el vector que apunta de la posici�n anterior a la actual del Fixer.
            Vector3 newPosition = fixedNode.Position + (movement);                          // Se calcula la nueva posici�n del nodo.
            
            fixedNode.SetNodePosition(newPosition);                                         // Se desplaza la posici�n del nodo en funci�n del desplazamiento del Fixer.
        }

        _fixerLastPosition = transform.position;                                            // Se actualiza la �ltima posici�n del Fixer.
        transform.hasChanged = false;                                                       // Se restaura el flag de cambio de posici�n del Fixer.
    }
}
