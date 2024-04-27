using System.Collections.Generic;

public class SpringManager
{
    public readonly HashSet<(int, int)> existingSprings = new();                            // Conjunto de muelles existentes.

    private Dictionary<(int, int), Node> _oppositeNodes = new();                            // Diccionario de nodos opuestos.

    public bool CreateSpring(Node nodeA, Node nodeB, Node nodeC, List<Node> nodeList)        // Crea un muelle entre dos nodos.
    {
        int idA = nodeList.IndexOf(nodeA);                                                  // Obtiene el índice del nodo A.
        int idB = nodeList.IndexOf(nodeB);                                                  // Obtiene el índice del nodo B.
        
        var springKey = (idA < idB) ? (idA, idB) : (idB, idA);                              // Ordena los nodos para evitar duplicados.

        if (!existingSprings.Contains(springKey))                                           // Si el muelle no existe, se almacenan sus datos.
        {
            existingSprings.Add(springKey);                                                 // Se añade el muelle al conjunto de muelles existentes de esta clase.
            _oppositeNodes[springKey] = nodeC;                                              // Se añade el nodo opuesto al diccionario de nodos opuestos.
            return true;
        }       
        return false;                                                                       // Si el muelle ya existe, no se almacena nada.
    }

    public Node GetOppositeNode(Node nodeA, Node nodeB, List<Node> nodeList)                // Obtiene el nodo opuesto a dos nodos.
    {
        int idA = nodeList.IndexOf(nodeA);                                                  // Obtiene el índice del nodo A.
        int idB = nodeList.IndexOf(nodeB);                                                  // Obtiene el índice del nodo B.
        
        var springKey = (idA < idB) ? (idA, idB) : (idB, idA);                              // Ordena los nodos para evitar duplicados.

        return _oppositeNodes[springKey];                                                   // Devuelve el nodo opuesto.
    }
}