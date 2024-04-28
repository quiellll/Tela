using UnityEngine;
using System.Collections.Generic;
using System;

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
        _timeStep = 0.0085f;
        _mass = 1f;
        _traction = 100f;
        _flexion = 180f;
        _nodeDamping = 1.35f;
        _springDamping = 15f;
        _gravity = new Vector3(0.0f, -9.81f, 0.0f);
        _windEnabled = false;
        _windVariation = 2f;
        _windForce = new Vector3(0f, 0f, 0f);
    }

    #region InEditorVariables
  

    [SerializeField] private bool _paused;                                                  // Variable que controla si la simulación está pausada.

    [SerializeField] [Range (0.001f, 0.01f)]    private float _timeStep;                    // Paso de tiempo de la simulación.
    [SerializeField] [Range (0.01f, 10f)]       private float _mass;                        // Masa de los nodos.
    [SerializeField] [Range (50f, 200f)]        private float _traction;                    // Constante de rigidez de los muelles.
    [SerializeField] [Range (100f, 400f)]       private float _flexion;                     // Constante de flexión de los muelles.
    [SerializeField] [Range (0.1f, 5f)]         private float _nodeDamping;                 // Constante de amortiguación de los nodos.
    [SerializeField] [Range (5f, 40f)]          private float _springDamping;               // Constante de amortiguación de los muelles.
    [SerializeField]                            private Vector3 _gravity;                   // Gravedad de la simulación.
    [SerializeField]                            private bool _windEnabled;
    [SerializeField] [Range (0.1f, 5f)]         private float _windVariation;               // Variación de la fuerza del viento.
    [SerializeField]                            private Vector3 _windForce;                 // Fuerza del viento de la simulación.

    #endregion

    #region OtherVariables

    private ChangeTrackingWrapper<float> _massTracker;                                      // Tracker del valor de la masa.
    private ChangeTrackingWrapper<float> _stiffnessTracker;                                 // Tracker del valor de la rigidez.
    private ChangeTrackingWrapper<float> _flexionTracker;                                   // Tracker del valor de la flexión.
    private ChangeTrackingWrapper<float> _nodeDampingTracker;                               // Tracker del valor de la amortiguación de los nodos.
    private ChangeTrackingWrapper<float> _springDampingTracker;                             // Tracker del valor de la amortiguación de los muelles.
    private ChangeTrackingWrapper<float> _windVariationTracker;                             // Tracker de la variación de la fuerza del viento.
    private ChangeTrackingWrapper<Vector3> _gravityTracker;                                 // Tracker del valor de la gravedad.
    private ChangeTrackingWrapper<Vector3> _windForceTracker;                               // Tracker del valor de la fuerza del viento.

    private Wind _wind;                                                                     // Viento de la simulación.

    public List<Node> NodeList { get; private set; }                                        // Lista de nodos de la tela.
    private List<Spring> SpringList { get;  set; }                                          // Lista de muelles de la tela.
    private List<Spring> FlexionSpringList { get; set; }                                    // Lista de muelles de flexión de la tela.

    private SpringManager _springManager;                                                   // Manager de muelles.

    private Mesh _mesh;                                                                     // Malla del objeto.

    private Vector3[] _vertices;                                                            // Lista de vértices de la malla.

    #endregion

    #region MonoBehaviour

    public void Awake()
    {
        InitializeFromMesh();                                                               // Inicializa los nodos y muelles a partir de la malla del objeto.

        InitializeTrackers();                                                               // Inicializa los trackers de los parámetros de la simulación.

        _wind = new(_windForce);                                                            // Inicializa el viento de la simulación.
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))                                                      // Pausa la simulación si se pulsa la tecla P.
            _paused = !_paused;

        UpdateParameterTrackers();                                                          // Actualiza los trackers de los parámetros de la simulación.

        CheckPhysicsParametersUpdates();                                                    // Comprueba si se han actualizado los parámetros de la simulación.            
    }

    public void FixedUpdate()
    {
        if (!_paused)
        {
            StepSymplectic();                                                               // NOTA: Se elimina la selección de método de integración puesto que se ha prescindido del método explícito en clase.

            if (_windEnabled) _wind.UpdateWind(Time.deltaTime);                             // Actualiza la fuerza del viento. 
        }
    }

    #endregion

    public bool WindEnabled() { return _windEnabled; }                                      // Devuelve si el viento está activado.

    private void InitializeFromMesh()                                                       // Función que inicializa los nodos y muelles a partir de la malla del objeto.
    {
        _mesh = GetComponent<MeshFilter>().mesh;                                            // Inicialización de la malla.

        _vertices = _mesh.vertices;                                                         // Inicialización de la lista de vértices.
        int[] triangles = _mesh.triangles;                                                  // Inicialización de la lista de triángulos de la malla.

        _springManager = new();                                                             // Inicialización del manager de muelles.
        NodeList = new List<Node>();                                                        // Inicialización de la lista de nodos.
        SpringList = new List<Spring>();                                                    // Inicialización de la lista de muelles.
        FlexionSpringList = new List<Spring>();                                             // Inicialización de la lista de muelles de flexión.

        for (int i = 0; i < _vertices.Length; i++)                                          // Inicialización de nodos por cada vértice de la malla.
        {
            Node newNode = new(this, transform.TransformPoint(_vertices[i]),
                                                        _mass, _nodeDamping, 
                                                        _gravity, _windForce);
            NodeList.Add(newNode);
        }


        for (int i = 0; i < triangles.Length; i += 3)                                       // Inicialización de muelles por cada triángulo de la malla.
        {
            Node node0 = NodeList[triangles[i]];                                            // Obtiene el nodo 0.
            Node node1 = NodeList[triangles[i + 1]];                                        // Obtiene el nodo 1.
            Node node2 = NodeList[triangles[i + 2]];                                        // Obtiene el nodo 2.

            InitializeMeshSpring(node0, node1, node2);                                      // Muelle entre vértices 0 y 1.
            InitializeMeshSpring(node1, node2, node0);                                      // Muelle entre vértices 1 y 2.
            InitializeMeshSpring(node2, node0, node1);                                      // Muelle entre vértices 2 y 0.
        }
    }

    private void InitializeMeshSpring(Node nodeA, Node nodeB, Node nodeC)                   // Función que inicializa un muelle entre dos nodos.
    {
        if (_springManager.CreateSpring(nodeA, nodeB, nodeC, NodeList))                     // Si el muelle no existe, se crea.
        {
            Spring newSpring = new(nodeA, nodeB, _traction, _springDamping);
            SpringList.Add(newSpring);
        }
        else if (!_springManager.CreateSpring(nodeA, nodeB, nodeC, NodeList))               // Si el muelle ya existe, se crea un muelle entre el nodo C y el nodo opuesto.
        {
            Node targetNode = _springManager.GetOppositeNode(nodeA, nodeB, NodeList);       // Obtiene el nodo opuesto.
            Spring newSpring = new(nodeC, targetNode, _flexion, _springDamping);            // Crea el muelle de flexión.
            FlexionSpringList.Add(newSpring);
        }
    }

    private void InitializeTrackers()                                                       // Función que inicializa los trackers de los parámetros de la simulación.
    {
        _massTracker = new(_mass);                                                          // Inicializa el tracker de la masa.
        _stiffnessTracker = new(_traction);                                                 // Inicializa el tracker de la rigidez.
        _flexionTracker = new(_flexion);                                                    // Inicializa el tracker de la flexión.
        _nodeDampingTracker = new(_nodeDamping);                                            // Inicializa el tracker de la amortiguación de los nodos.
        _springDampingTracker = new(_springDamping);                                        // Inicializa el tracker de la amortiguación de los muelles.
        _windVariationTracker = new(_windVariation);                                        // Inicializa el tracker de la variación de la fuerza del viento.
        _gravityTracker = new(_gravity);                                                    // Inicializa el tracker de la gravedad.
        _windForceTracker = new(_windForce);                                                // Inicializa el tracker de la fuerza del viento.
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

    private void UpdateParameterTrackers()                                                  // Función que actualiza los trackers de los parámetros de la simulación.
    {
        _massTracker.Value = _mass;                                                         // Actualiza el tracker de la masa.
        _stiffnessTracker.Value = _traction;                                                // Actualiza el tracker de la rigidez.
        _flexionTracker.Value = _flexion;                                                   // Actualiza el tracker de la flexión.
        _nodeDampingTracker.Value = _nodeDamping;                                           // Actualiza el tracker de la amortiguación de los nodos.
        _springDampingTracker.Value = _springDamping;                                       // Actualiza el tracker de la amortiguación de los muelles.
        _gravityTracker.Value = _gravity;                                                   // Actualiza el tracker de la gravedad.
        _windForceTracker.Value = _windForce;                                               // Actualiza el tracker de la gravedad.
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
                spring.ModifySpringStiffness(_traction);
            }

            _stiffnessTracker.ResetChangedFlag();                                           // Restablece el flag de cambio de rigidez.
        }

        if (_flexionTracker.HasChanged)                                                     // Comprueba si la flexión ha cambiado.
        {
            foreach (Spring spring in FlexionSpringList)                                    // Actualiza la flexión de los muelles.
            {
                spring.ModifySpringStiffness(_flexion);
            }

            _flexionTracker.ResetChangedFlag();                                             // Restablece el flag de cambio de flexión.
        }

        if (_nodeDampingTracker.HasChanged)                                                 // Comprueba si la amortiguación de los nodos ha cambiado.
        {
            foreach (Node node in NodeList)                                                 // Actualiza la amortiguación de los nodos.
            {
                node.ModifyNodeDamping(_nodeDamping);
            }

            _nodeDampingTracker.ResetChangedFlag();                                         // Restablece el flag de cambio de amortiguación de los nodos.
        }

        if (_springDampingTracker.HasChanged)                                               // Comprueba si la amortiguación de los muelles ha cambiado.
        {
            foreach (Spring spring in SpringList)                                           // Actualiza la amortiguación de los muelles.
            {
                spring.ModifySpringDamping(_springDamping);
            }

            _springDampingTracker.ResetChangedFlag();                                       // Restablece el flag de cambio de amortiguación de los muelles.
        }

        if (_windVariationTracker.HasChanged)                                               // Comprueba si la variación de la fuerza del viento ha cambiado.
        {
            _wind.ModifyVariation(_windVariation);                                          // Actualiza la variación de la fuerza del viento.

            _windVariationTracker.ResetChangedFlag();                                       // Restablece el flag de cambio de la variación de la fuerza del viento.
        }

        if (_gravityTracker.HasChanged)                                                     // Comprueba si la gravedad ha cambiado.
        {
            foreach (Node node in NodeList)                                                 // Actualiza la gravedad de los nodos.
            {
                node.ModifyNodeGravity(_gravity);
            }

            _gravityTracker.ResetChangedFlag();                                             // Restablece el flag de cambio de gravedad.
        }

        if (_windForceTracker.HasChanged)                                                   // Comprueba si el viento ha cambiado.
        {
            foreach (Node node in NodeList)                                                 // Actualiza la fuerza del viento de los nodos.
            {
                node.ModifyNodeGravity(_windForce);
            }

            _windForceTracker.ResetChangedFlag();                                           // Restablece el flag de cambio de viento.
        }
    }
}
