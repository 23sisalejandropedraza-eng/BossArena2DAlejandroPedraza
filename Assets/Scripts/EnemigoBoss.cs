using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemigoBoss : MonoBehaviour
{
    public int vidaMaxima = 100;
    public int danoAlJugador = 20;

    private int vidaActual;

    void Start()
    {
        vidaActual = vidaMaxima;
    }

    public void RecibirDano(int cantidad)
    {
        vidaActual -= cantidad;
        Debug.Log("Boss vida restante: " + vidaActual);

        if (vidaActual <= 0)
        {
            vidaActual = 0;
            Morir();
        }
    }

    void Morir()
    {
        gameObject.SetActive(false);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        JugadorControlador jugador = collision.gameObject.GetComponent<JugadorControlador>();
        if (jugador != null)
        {
            jugador.RecibirDano(danoAlJugador);
        }
    }
}
