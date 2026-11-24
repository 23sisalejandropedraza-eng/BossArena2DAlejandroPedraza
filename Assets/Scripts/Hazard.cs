using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    public int dano = 1;
    public float empujeArriba = 7f;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        JugadorControlador jugador = other.GetComponent<JugadorControlador>();
        if (jugador != null)
        {
            jugador.MarcarEnAgua(true);
            if (jugador.EstaMuerto()) return;
        }

        if (other.TryGetComponent<IDaniable>(out IDaniable objetivo))
        {
            objetivo.RecibirDano(dano);
        }

        if (jugador != null && jugador.EstaMuerto()) return;

        if (other.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = new Vector2(rb.velocity.x, empujeArriba);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.TryGetComponent<JugadorControlador>(out JugadorControlador jugador))
        {
            jugador.MarcarEnAgua(false);
        }
    }
}