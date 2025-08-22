using UnityEngine;

public class BotonComida : MonoBehaviour
{
    [SerializeField] private ComidaAparicion spawner;

    [Header("Feedback (Materiales)")]
    [SerializeField] private Renderer targetRenderer;   // MeshRenderer de este hijo
    [SerializeField] private Material materialListo;
    [SerializeField] private Material materialApagado;
    [SerializeField] private Transform visual;          // este hijo
    [SerializeField] private float hundirY = -0.05f;

    [Header("Detección")]
    [SerializeField] private Transform pivot;           // de dónde medir distancia (si null, usa 'transform')
    [SerializeField] private float pressRadio = 0.8f;   // fallback si el trigger no entra

    bool disponible = true;
    bool agenteDentro = false;
    Vector3 visualPosLocal0;

    void Awake()
    {
        if (!pivot) pivot = transform;
        if (visual) visualPosLocal0 = visual.localPosition;
        ActualizarVisual();
    }

    public bool PuedeUsarBoton() => disponible;

    public bool PuedePresionar(Transform agente)
    {
        if (!disponible) return false;
        if (agenteDentro) return true; // ideal: dentro del trigger
        // fallback por distancia al pivot
        return (agente.position - pivot.position).sqrMagnitude <= pressRadio * pressRadio;
    }

    public void PulsarBoton()
    {
        if (!disponible) return;
        disponible = false;

        if (visual) visual.localPosition = visualPosLocal0 + new Vector3(0f, hundirY, 0f);
        if (spawner) spawner.SpawnComida(); // aparece la comida

        ActualizarVisual();
    }

    public void Resetear()
    {
        disponible = true;
        agenteDentro = false;
        if (visual) visual.localPosition = visualPosLocal0;
        ActualizarVisual();
    }

    void ActualizarVisual()
    {
        if (targetRenderer)
            targetRenderer.sharedMaterial = disponible ? materialListo : materialApagado;
    }

    // Estos tres solo se usan si pusiste el trigger en ESTE hijo.
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<AgenteComida>() != null) agenteDentro = true;
    }
    void OnTriggerStay(Collider other)
    {
        if (other.GetComponentInParent<AgenteComida>() != null) agenteDentro = true;
    }
    void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<AgenteComida>() != null) agenteDentro = false;
    }
}
