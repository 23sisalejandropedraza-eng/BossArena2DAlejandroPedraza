using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Identificador del checkpoint (0, 1 o 2)")]
    public int indiceCheckpoint = 0;

    [Header("Posicion exacta de reaparicion")]
    public Transform puntoReaparicion;

    void Awake()
    {
        if (puntoReaparicion == null)
        {
            puntoReaparicion = transform;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<JugadorControlador>(out JugadorControlador jugador))
        {
            if (!jugador.EstaMuerto() && GestorProgresoJugador.Instancia != null)
            {
                GestorProgresoJugador.Instancia.ActivarCheckpoint(indiceCheckpoint, puntoReaparicion.position);
            }
        }
    }
}