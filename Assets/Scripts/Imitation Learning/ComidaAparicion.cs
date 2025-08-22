using UnityEngine;

public class ComidaAparicion : MonoBehaviour
{
    [Header("Prefab de comida")]
    [SerializeField] private GameObject comidaPrefab;

    [Header("Área LOCAL respecto a 'Environment'")]
    [SerializeField] private float minX = -7.5f, maxX = 7.5f;
    [SerializeField] private float minZ = -7.5f, maxZ = 7.5f;
    [SerializeField] private float y = 0f;          // altura a la que quieres la comida
    [SerializeField] private float margenBordes = 0f; // pon 0.5 si tienes muros pegados


    private Transform ultimaComida;

    public bool AparecioComida() => ultimaComida != null;
    public Transform GetUltimaComidaTransform() => ultimaComida;

    public void LimpiarTodo()
    {
        if (ultimaComida != null)
        {
            Destroy(ultimaComida.gameObject);
            ultimaComida = null;
        }
    }

    public void SpawnComida()
    {
        LimpiarTodo();

        float x = Random.Range(minX + margenBordes, maxX - margenBordes);
        float z = Random.Range(minZ + margenBordes, maxZ - margenBordes);

        // Posición en coordenadas LOCALES del Environment
        Vector3 posLocal = new Vector3(x, y, z);
        Vector3 posMundo = transform.TransformPoint(posLocal);

        ultimaComida = Instantiate(comidaPrefab, posMundo, Quaternion.identity, transform).transform;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 a = transform.TransformPoint(new Vector3(minX, y, minZ));
        Vector3 b = transform.TransformPoint(new Vector3(maxX, y, minZ));
        Vector3 c = transform.TransformPoint(new Vector3(maxX, y, maxZ));
        Vector3 d = transform.TransformPoint(new Vector3(minX, y, maxZ));
        Gizmos.DrawLine(a, b); Gizmos.DrawLine(b, c); Gizmos.DrawLine(c, d); Gizmos.DrawLine(d, a);
    }
}
