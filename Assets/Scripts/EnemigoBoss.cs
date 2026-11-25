using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemigoBoss : MonoBehaviour, IDaniable
{
    [Header("Vida y danio")]
    public int vidaMaxima = 100;
    public int danoAlJugador = 1;
    public float cooldownGolpe = 1.2f;
    public float duracionGolpe = 0.5f;

    [Header("Patrulla y orientacion")]
    public float velocidadPatrulla = 2f;
    public float distanciaPatrulla = 2f;
    public bool spriteMiraIzquierdaPorDefecto = true;

    private int vidaActual;
    private float puntoInicialX;
    private int direccion = 1;
    private float temporizadorGolpe = 0f;
    private float temporizadorAnimGolpe = 0f;
    private bool estaGolpeando = false;
    private bool estaMuerto = false;

    private JugadorControlador jugadorEnZona = null;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        vidaActual = vidaMaxima;
        puntoInicialX = transform.position.x;
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        AplicarFlip();
    }

    void Update()
    {
        if (estaMuerto) return;

        if (temporizadorGolpe > 0f)
        {
            temporizadorGolpe -= Time.deltaTime;
        }

        if (estaGolpeando)
        {
            temporizadorAnimGolpe -= Time.deltaTime;
            if (temporizadorAnimGolpe <= 0f)
            {
                estaGolpeando = false;
            }

            if (anim != null)
            {
                anim.SetBool("Camina", false);
            }
            return;
        }

        if (jugadorEnZona != null && temporizadorGolpe <= 0f)
        {
            EjecutarGolpe(jugadorEnZona);
            return;
        }

        Patrullar();
    }

    public void JugadorEntroEnZona(JugadorControlador jugador)
    {
        jugadorEnZona = jugador;
    }

    public void JugadorSalioDeZona(JugadorControlador jugador)
    {
        if (jugadorEnZona == jugador)
        {
            jugadorEnZona = null;
        }
    }

    void EjecutarGolpe(JugadorControlador jugador)
    {
        temporizadorGolpe = cooldownGolpe;
        temporizadorAnimGolpe = duracionGolpe;
        estaGolpeando = true;

        if (anim != null)
        {
            anim.SetBool("Camina", false);
            anim.ResetTrigger("Golpear");
            anim.SetTrigger("Golpear");
        }

        jugador.RecibirDano(danoAlJugador);
    }

    void Patrullar()
    {
        transform.Translate(Vector2.right * direccion * velocidadPatrulla * Time.deltaTime);

        if (anim != null)
        {
            anim.SetBool("Camina", velocidadPatrulla > 0.01f);
        }

        if (transform.position.x >= puntoInicialX + distanciaPatrulla)
        {
            direccion = -1;
            AplicarFlip();
        }
        else if (transform.position.x <= puntoInicialX - distanciaPatrulla)
        {
            direccion = 1;
            AplicarFlip();
        }
    }

    void AplicarFlip()
    {
        if (spriteRenderer == null) return;

        if (spriteMiraIzquierdaPorDefecto)
        {
            spriteRenderer.flipX = (direccion == 1);
        }
        else
        {
            spriteRenderer.flipX = (direccion == -1);
        }
    }

    public void RecibirDano(int cantidad)
    {
        if (estaMuerto) return;

        vidaActual -= cantidad;

        if (vidaActual <= 0)
        {
            vidaActual = 0;
            Morir();
        }
        else if (anim != null)
        {
            anim.ResetTrigger("Golpeado");
            anim.SetTrigger("Golpeado");
        }
    }

    void Morir()
    {
        estaMuerto = true;
        estaGolpeando = false;
        jugadorEnZona = null;

        if (anim != null)
        {
            anim.SetBool("Camina", false);
            anim.ResetTrigger("Golpear");
            anim.ResetTrigger("Golpeado");
            anim.SetTrigger("Morir");
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        Invoke(nameof(DesactivarBoss), 1.0f);
    }

    void DesactivarBoss()
    {
        gameObject.SetActive(false);
    }
}