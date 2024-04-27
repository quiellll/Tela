using System.Collections.Generic;

public class SpringManager
{
    readonly HashSet<(int, int)> existingSprings = new();                                   // Conjunto de muelles existentes.

    public void CreateSpring(Node nodeA, Node nodeB, float stiffness, 
                             List<Spring> springList, List<Node> nodeList)                  // Crea un muelle entre dos nodos.
    {
        int idA = nodeList.IndexOf(nodeA);                                                  // Obtiene el �ndice del nodo A.
        int idB = nodeList.IndexOf(nodeB);                                                  // Obtiene el �ndice del nodo B.
        
        var springKey = (idA < idB) ? (idA, idB) : (idB, idA);                              // Ordena los nodos para evitar duplicados.

        if(!existingSprings.Contains(springKey))                                            // Si el muelle no existe, se crea.
        {
            Spring newSpring = new(nodeA, nodeB, stiffness);
            springList.Add(newSpring);                                                      // Se a�ade el muelle a la lista de muelles de la tela.
            existingSprings.Add(springKey);                                                 // Se a�ade el muelle al conjunto de muelles existentes de esta clase.
        }
    }
}