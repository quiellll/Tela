using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Clase que controla la simulación de una tela mediante un sistema de partículas.
/// Convierte cada vértice de la malla de un objeto en un nodo y cada arista de los triángulos de su malla en un muelle.
/// Debe ser añadida a un objeto con un componente MeshFilter. Dicho objeto debe tener un Mesh asociado.
/// </summary>
public class MassSpringCloth : MonoBehaviour
{

    public MassSpringCloth()                                                                // Clase que almacena las variables para controlar la simulación de la tela.
    {
        _paused = false;
        _timeStep = 0.01f;
        _mass = 10;
        _stiffness = 1000;
        _gravity = new Vector3(0.0f, -9.81f, 0.0f);
    }

    #region InEditorVariables
    public List<Node> NodeList { get; private set; }                                        // Lista de nodos de la tela.
    public List<Spring> SpringList { get; private set; }                                    // Lista de muelles de la tela.

    [SerializeField] private bool _paused;                                                  // Variable que controla si la simulación está pausada.

    [SerializeField] private float _timeStep;                                               // Paso de tiempo de la simulación.
    [SerializeField] private float _mass;
    [SerializeField] private float _stiffness;
    [SerializeField] private Vector3 _gravity;                                              // Gravedad de la simulación.

    #endregion

    #region OtherVariables

    [SerializeField] private ChangeTrackingWrapper<float> _massTracker;                     // Masa de los nodos.
    [SerializeField] private ChangeTrackingWrapper<float> _stiffnessTracker;                // Constante de rigidez de los muelles.

    private SpringManager _springManager;                                                   // Manager de muelles.

    private Mesh _mesh;                                                                     // Malla del objeto.

    private Vector3[] _vertices;                                                            // Lista de vértices de la malla.

    #endregion

    #region MonoBehaviour

    public void Awake()
    {
        InitializeFromMesh();                                                               // Inicializa los nodos y muelles a partir de la malla del objeto.

        _massTracker = new(_mass);                                                          // Inicializa el tracker de la masa.
        _stiffnessTracker = new(_stiffness);                                                // Inicializa el tracker de la rigidez.
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))                                                      // Pausa la simulación si se pulsa la tecla P.
            _paused = !_paused;

        _massTracker.Value = _mass;                                                         // Actualiza el tracker de la masa.
        _stiffnessTracker.Value = _stiffness;                                               // Actualiza el tracker de la rigidez.

        CheckPhysicsParametersUpdates();                                                    // Comprueba si se han actualizado los parámetros de la simulación.
    }

    public void FixedUpdate()
    {
        if (!_paused) StepSymplectic();                                                     // NOTA: Se elimina la selección de método de integración puesto que se ha prescindido del método explícito en clase.
    }

    #endregion

    private void InitializeFromMesh()                                                       // Función que inicializa los nodos y muelles a partir de la malla del objeto.
    {
        _mesh = GetComponent<MeshFilter>().mesh;                                            // Inicialización de la malla.

        _vertices = _mesh.vertices;                                                         // Inicialización de la lista de vértices.
        int[] triangles = _mesh.triangles;                                                  // Inicialización de la lista de triángulos de la malla.

        _springManager = new();                                                             // Inicialización del manager de muelles.
        NodeList = new List<Node>();                                                        // Inicialización de la lista de nodos.
        SpringList = new List<Spring>();                                                    // Inicialización de la lista de muelles.

        for (int i = 0; i < _vertices.Length; i++)                                          // Inicialización de nodos por cada vértice de la malla.
        {
            Node newNode = new(transform.TransformPoint(_vertices[i]), _mass, _gravity);
            NodeList.Add(newNode);
        }


        for (int i = 0; i < triangles.Length; i += 3)                                       // Inicialización de muelles por cada triángulo de la malla.
        {
            InitializeMeshSpring(NodeList[triangles[i]], NodeList[triangles[i + 1]]);       // Muelle entre vértices 0 y 1.
            InitializeMeshSpring(NodeList[triangles[i + 1]], NodeList[triangles[i + 2]]);   // Muelle entre vértices 1 y 2.
            InitializeMeshSpring(NodeList[triangles[i + 2]], NodeList[triangles[i]]);       // Muelle entre vértices 2 y 0.
        }
    }

    private void InitializeMeshSpring(Node nodeA, Node nodeB)                               // Función que inicializa un muelle entre dos nodos.
    {
        _springManager.CreateSpring(nodeA, nodeB, _stiffness, SpringList, NodeList);
    }

    private void StepSymplectic()                                                           // Función que realiza un paso de la simulación mediante el método de Euler.
    {
        foreach (Node node in NodeList)                                                     // Calcula las fuerzas de los nodos.
        {
            node.ComputeForces();
        }

        foreach (Spring spring in SpringList)                                               // Calcula las fuerzas de los muelles.
        {
            spring.ComputeSpringForces();
        }

        for (int i = 0; i < NodeList.Count; i++)                                            // Actualiza las posiciones y velocidades de los nodos.
        {
            var node = NodeList[i];                                                         // Obtiene el nodo actual.

            node.NodeSymplecticEulerIntegration(_timeStep);                                 // Realiza el cálculo de integración de Euler (Método Simpléctico). 

            _vertices[i] = transform.InverseTransformPoint(node.Position);                  // Actualiza la posición de los vértices de coordenadas globales a coordenadas locales.
        }

        _mesh.vertices = _vertices;                                                         // Actualiza la malla con las nuevas posiciones calculadas para cada nodo.

        foreach (Spring spring in SpringList)                                               // Actualiza las longitudes de los muelles.
        {
            spring.UpdateSpringLength();
        }
    }

    private void CheckPhysicsParametersUpdates()                                            // Función que comprueba si se han actualizado los parámetros de la simulación.
    {
        if (_massTracker.HasChanged)                                                        // Comprueba si la masa ha cambiado.
        {
            foreach (Node node in NodeList)                                                 // Actualiza la masa de los nodos.
            {
                node.ModifyNodeMass(_mass);
            }

            _massTracker.ResetChangedFlag();                                                // Restablece el flag de cambio de masa.
        }

        if (_stiffnessTracker.HasChanged)                                                   // Comprueba si la rigidez ha cambiado.
        {
            foreach (Spring spring in SpringList)                                           // Actualiza la rigidez de los muelles.
            {
                spring.ModifySpringStiffness(_stiffness);
            }

            _stiffnessTracker.ResetChangedFlag();                                           // Restablece el flag de cambio de rigidez.
        }
    }
}
