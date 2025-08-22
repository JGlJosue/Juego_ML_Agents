using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovimientoAMeta : Agent
{
    [SerializeField] private Transform transformObjetivo; // Transform del objetivo al que se mover� el agente
    [SerializeField] private Material victoriaMaterial; // Material para cambiar el color al ganar
    [SerializeField] private Material derrotaMaterial; // Material para cambiar el color al perder
    [SerializeField] private MeshRenderer sueloMeshRenderer; // MeshRenderer del suelo para cambiar el color

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-7.5f, +7.5f), 0, Random.Range(-7.5f, 7.5f)); // Reiniciar la posici�n del agente a una ubicaci�n aleatoria dentro de un rango
        transformObjetivo.localPosition = new Vector3(Random.Range(-7.5f, +7.5f), 0, Random.Range(-7.5f, 7.5f)); // Reiniciar la posici�n del objetivo a una ubicaci�n aleatoria dentro de un rango
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // A�adir la posici�n del agente y del objetivo como observaciones
        sensor.AddObservation(transform.position);
        sensor.AddObservation(transformObjetivo.position);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float movimientoX = actions.ContinuousActions[0]; // Acci�n continua para el movimiento en X
        float movimientoZ = actions.ContinuousActions[1]; // Acci�n continua para el movimiento en Z

        float velocidad = 5.0f; // Velocidad de movimiento del agente
        transform.position += new Vector3(movimientoX, 0, movimientoZ) * velocidad * Time.deltaTime; // Mover el agente
        //base.OnActionReceived(actions);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Permitir el control manual del agente para pruebas
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        float h = 0f, v = 0f; // Inicializar las variables de movimiento horizontal y vertical
        var kb = Keyboard.current; // Obtener el teclado actual para entradas manuales
        if (kb != null) // Verificar si el teclado est� disponible
        {
            // Movimiento Horizontal: A/D o flecha izquierda/derecha
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) h -= 1f;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) h += 1f;

            // Movimiento Vertical: W/S o flecha arriba/abajo
            if (kb.sKey.isPressed || kb.downArrowKey.isPressed) v -= 1f;
            if (kb.wKey.isPressed || kb.upArrowKey.isPressed) v += 1f;
        }

        continuousActions[0] = Mathf.Clamp(h, -1f, 1f); // Clamping para limitar el movimiento horizontal entre -1 y 1
        continuousActions[1] = Mathf.Clamp(v, -1f, 1f); // Clamping para limitar el movimiento vertical entre -1 y 1
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Meta>(out Meta meta))
        {
            SetReward(1.0f); // Recompensa por alcanzar el objetivo
            sueloMeshRenderer.material = victoriaMaterial; // Cambiar el color del suelo al ganar
            EndEpisode(); // Terminar el episodio al alcanzar el objetivo
        }
        if (other.TryGetComponent<Pared>(out Pared pared))
        {
            SetReward(-1.0f); // Penalizaci�n por chocar con una pared
            sueloMeshRenderer.material = derrotaMaterial; // Cambiar el color del suelo al perder
            EndEpisode(); // Terminar el episodio al chocar con una pared
        }
    }
}
