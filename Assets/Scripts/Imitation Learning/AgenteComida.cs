using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgenteComida : Agent
{
    public event EventHandler OnComerAlimento;
    public event EventHandler OnEpisodeBeginEvent;

    [SerializeField] private ComidaAparicion comidaAparicion;
    [SerializeField] private BotonComida botonComida;

    private Rigidbody agenteRigidbody;

    private void Awake()
    {
        agenteRigidbody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(UnityEngine.Random.Range(-7f, +7f), 0, UnityEngine.Random.Range(-7.5f, 7.5f)); // Reiniciar la posición del agente a una ubicación aleatoria dentro de un rango
        OnEpisodeBeginEvent?.Invoke(this, EventArgs.Empty); // Invocar el evento al inicio del episodio
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(botonComida.PuedeUsarBoton() ? 1 : 0);

        Vector3 direccionBotonComida = (botonComida.transform.localPosition - transform.localPosition).normalized;
        sensor.AddObservation(direccionBotonComida.x);
        sensor.AddObservation(direccionBotonComida.z);

        sensor.AddObservation(comidaAparicion.AparecioComida() ? 1 : 0);

        if (comidaAparicion.AparecioComida())
        {
            Vector3 direccionComida = (comidaAparicion.GetUltimaComidaTransform().localPosition - transform.localPosition).normalized;
            sensor.AddObservation(direccionComida.x);
            sensor.AddObservation(direccionComida.z);
        }
        else
        {
            sensor.AddObservation(0f);
            sensor.AddObservation(0f);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 addForce = Vector3.zero;

        // Rama X: 0 quieto, 1 izq, 2 der
        switch (actions.DiscreteActions[0])
        {
            case 1: addForce.x = -1f; break;
            case 2: addForce.x = 1f; break;
            default: addForce.x = 0f; break;
        }

        // Rama Z: 0 quieto, 1 atrás, 2 adelante
        switch (actions.DiscreteActions[1])
        {
            case 1: addForce.z = -1f; break;
            case 2: addForce.z = 1f; break;
            default: addForce.z = 0f; break;
        }

        float velocidad = 5f;
        agenteRigidbody.linearVelocity = new Vector3(
            addForce.x * velocidad,
            agenteRigidbody.linearVelocity.y,
            addForce.z * velocidad
        );

        // Botón (0/1)
        if (actions.DiscreteActions[2] == 1)
        {
            foreach (var col in Physics.OverlapBox(transform.position, Vector3.one * 0.5f))
            {
                if (col.TryGetComponent<BotonComida>(out var btn) && botonComida.PuedeUsarBoton())
                {
                    botonComida.PulsarBoton();
                    AddReward(1f);
                    break;
                }
            }
        }

        AddReward(-1f / MaxStep);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> d = actionsOut.DiscreteActions;

        // reset
        d[0] = 0; // X: 0 quieto, 1 izq, 2 der
        d[1] = 0; // Z: 0 quieto, 1 atrás, 2 adelante
        d[2] = 0; // botón

        var kb = Keyboard.current;
        if (kb == null) return; // por si no hay teclado (build headless, etc.)

        // eje X (A/D)
        if (kb.aKey.isPressed && !kb.dKey.isPressed) d[0] = 1;
        else if (kb.dKey.isPressed && !kb.aKey.isPressed) d[0] = 2;

        // eje Z (S/W)
        if (kb.sKey.isPressed && !kb.wKey.isPressed) d[1] = 1;
        else if (kb.wKey.isPressed && !kb.sKey.isPressed) d[1] = 2;

        // botón (Space)
        d[2] = kb.spaceKey.wasPressedThisFrame ? 1 : 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Comida>(out Comida comida))
        {
            AddReward(1f); // Añadir recompensa al recoger comida
            Destroy(comida.gameObject); // Destruir la comida recogida
            OnComerAlimento?.Invoke(this, EventArgs.Empty); // Invocar el evento al comer alimento
            EndEpisode(); // Terminar el episodio al recoger comida
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Pared>(out Pared pared))
        {
            SetReward(-1.0f); // Penalización por chocar con una pared
            EndEpisode(); // Terminar el episodio al chocar con una pared
        }
    }
}