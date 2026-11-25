using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VidaHUD : MonoBehaviour
{
    public JugadorControlador jugador;
    public Slider barraVida;

    void Start()
    {
        if (jugador != null && barraVida != null)
        {
            barraVida.minValue = 0;
            barraVida.maxValue = jugador.vidaMaxima;
            barraVida.value = jugador.VidaActual;
        }
    }

    void Update()
    {
        if (jugador != null && barraVida != null)
        {
            barraVida.value = jugador.VidaActual;
        }
    }
}