using UnityEngine;

public class AparicionComidaEscenario : MonoBehaviour
{
    [SerializeField] private ComidaAparicion comidaAparicion;
    [SerializeField] private BotonComida botonComida;      // arrastra AQUÍ el objeto que tiene el collider/trigger
    [SerializeField] private AgenteComida agenteComida;

    [Header("Área para colocar el botón")]
    [SerializeField] private float minX = -7f, maxX = 7f;
    [SerializeField] private float minZ = -7f, maxZ = 7f;

    private void OnEnable()
    {
        if (agenteComida != null)
        {
            agenteComida.OnEpisodeBeginEvent += HandleEpisodeBegin;
            agenteComida.OnComerAlimento += HandleComio;
        }
    }
    private void OnDisable()
    {
        if (agenteComida != null)
        {
            agenteComida.OnEpisodeBeginEvent -= HandleEpisodeBegin;
            agenteComida.OnComerAlimento -= HandleComio;
        }
    }

    private void HandleEpisodeBegin(object s, System.EventArgs e)
    {
        // Limpia comida
        comidaAparicion?.LimpiarTodo();

        // Mueve el MISMO botón (el que tiene el trigger / script BotonComida)
        if (botonComida != null)
        {
            var pos = new Vector3(Random.Range(minX, maxX), -0.5f, Random.Range(minZ, maxZ));
            botonComida.transform.position = pos;
            botonComida.Resetear(); // vuelve a "listo" (material encendido)
        }
    }

    private void HandleComio(object s, System.EventArgs e)
    {
        // opcional: limpiar restos al comer
        comidaAparicion?.LimpiarTodo();
    }

}
