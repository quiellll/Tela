using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Fija los nodos de una tela que se encuentren dentro de un objeto con un collider. 
/// Para vincular este script, se debe colocar el objeto que contiene este script como hijo
/// del objeto que contenga el componente MassSpringCloth.
/// </summary>
public class Fixer : MonoBehaviour
{    
    private MassSpringCloth _cloth;                                                         // Tela a la que se le aplicará la fijación.

    private Bounds _fixerBounds;                                                            // Área de fijación, coincidente con el Collider del objeto que contiene este script.
    private Vector3 _fixerLastPosition;                                                     // Almacena la última posición del Fixer.        

    private readonly List<Node> _fixedNodes = new();                                        // Lista de nodos fijados.
    
    private void Start()
    {
        if (GetComponentInParent<MassSpringCloth>() != null)                                // Si no se encuentra un componente MassSpringCloth3 en el padre, se omite.
            _cloth = GetComponentInParent<MassSpringCloth>();                               // Se obtienen el componente padre de tipo MassSpringCloth.
        
        _fixerBounds = GetComponent<Collider>().bounds;                                     // Se obtienen los límites del Collider del objeto que contiene este script.
        
        foreach (var node in _cloth.NodeList)                                               // Por cada nodo en la tela.
        {
            if (!_fixerBounds.Contains(node.Position)) continue;                            // Si no se encuentra dentro de los límites del Collider, se omite.
                                                                                            
            _fixedNodes.Add(node);                                                          // Si se encuentra dentro de los límites del Collider, se añade a la lista.
            node.FixNode();                                                                 // Y se fija el nodo.
        }

        _fixerLastPosition = transform.position;                                            // Se almacena la posición inicial del Fixer.
    }

    private void Update()
    {
        if (!transform.hasChanged) return;                                                  // Si el objeto que contiene este script no ha cambiado de posición, se omite.


        foreach (var fixedNode in _fixedNodes)                                              // Por cada nodo fijado.
        {   
            Vector3 movement = (transform.position - _fixerLastPosition);                   // Se calcula el vector que apunta de la posición anterior a la actual del Fixer.
            Vector3 newPosition = fixedNode.Position + (movement);                          // Se calcula la nueva posición del nodo.
            
            fixedNode.SetNodePosition(newPosition);                                         // Se desplaza la posición del nodo en función del desplazamiento del Fixer.
        }

        _fixerLastPosition = transform.position;                                            // Se actualiza la última posición del Fixer.
        transform.hasChanged = false;                                                       // Se restaura el flag de cambio de posición del Fixer.
    }
}
