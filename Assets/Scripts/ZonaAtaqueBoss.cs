using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonaAtaqueBoss : MonoBehaviour
{
    public EnemigoBoss boss;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (boss == null) return;

        JugadorControlador jugador = other.GetComponent<JugadorControlador>();
        if (jugador != null)
        {
            boss.JugadorEntroEnZona(jugador);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (boss == null) return;

        JugadorControlador jugador = other.GetComponent<JugadorControlador>();
        if (jugador != null)
        {
            boss.JugadorSalioDeZona(jugador);
        }
    }
}