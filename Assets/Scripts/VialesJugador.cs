using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VialesJugador : MonoBehaviour
{
    [Header("Viales")]
    public int viales = 2;
    public int maxViales = 2;
    public int curacion = 2;
    public KeyCode teclaUsar = KeyCode.H;

    [Header("Referencia y UI")]
    public JugadorControlador jugador;
    public Slider barraVida;
    public TMP_Text textoViales; 

    void Awake()
    {
        if (jugador == null)
        {
            jugador = GetComponent<JugadorControlador>();
        }
    }

    void Start()
    {
        viales = Mathf.Clamp(viales, 0, maxViales);
        ActualizarUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(teclaUsar))
        {
            UsarVial();
        }
    }

    public void UsarVial()
    {
        if (jugador == null || viales <= 0) return;
        if (jugador.EstaMuerto() || jugador.VidaActual >= jugador.vidaMaxima) return;

        if (jugador.Curar(curacion))
        {
            viales--;
            ActualizarUI();
        }
    }

    public void RestaurarViales()
    {
        viales = maxViales;
        ActualizarUI();
    }

    public void ActualizarUI()
    {
        if (barraVida != null && jugador != null)
        {
            barraVida.maxValue = jugador.vidaMaxima;
            barraVida.value = jugador.VidaActual;
        }

        if (textoViales != null)
        {
            textoViales.text = "Viales: " + viales;
        }
    }
}