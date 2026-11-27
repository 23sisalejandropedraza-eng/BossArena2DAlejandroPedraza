using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonaAtaqueBoss : MonoBehaviour
{
    public EnemigoBoss boss;
    public BossUndead bossUndead;

    private void OnTriggerEnter2D(Collider2D other)
    {
        JugadorControlador jugador = other.GetComponent<JugadorControlador>();
        if (jugador == null) return;

        if (boss != null)
        {
            boss.JugadorEntroEnZona(jugador);
        }

        if (bossUndead != null)
        {
            bossUndead.JugadorEntroEnZona(jugador);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        JugadorControlador jugador = other.GetComponent<JugadorControlador>();
        if (jugador == null) return;

        if (boss != null)
        {
            boss.JugadorSalioDeZona(jugador);
        }

        if (bossUndead != null)
        {
            bossUndead.JugadorSalioDeZona(jugador);
        }
    }
}