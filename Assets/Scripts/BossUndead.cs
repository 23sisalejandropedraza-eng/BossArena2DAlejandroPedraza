using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BossUndead : MonoBehaviour, IDaniable
{
    [Header("Vida y danio")]
    public int vidaMaxima = 100;
    public int danoGolpeNormal = 1;
    public int danoEmbestida = 2;
    public float cooldownGolpe = 1.4f;
    public float duracionGolpe = 0.55f;
    public float retrasoDanoGolpe = 0.25f;

    [Header("Patrulla y orientacion")]
    public float velocidadPatrulla = 2f;
    public float distanciaPatrulla = 2f;
    public bool spriteMiraIzquierdaPorDefecto = true;

    [Header("Habilidad de embestida")]
    public float cooldownEmbestida = 5f;
    public float distanciaMinEmbestida = 2.5f;
    public float distanciaMaxEmbestida = 7f;
    public float velocidadEmbestida = 8f;
    public float duracionEmbestida = 0.35f;
    public float retrasoSalidaEmbestida = 0.2f;
    public float rangoDanoEmbestida = 2.8f;

    [Header("Audio")]
    public AudioClip sonidoMuerte;
    public float volumenMuerte = 1f;

    [Header("Fin del juego")]
    public GameObject panelJuegoTerminado;
    public Button botonReiniciar;

    private int vidaActual;
    private float puntoInicialX;
    private int direccion = 1;
    private float temporizadorGolpe = 0f;
    private float temporizadorEmbestida = 0f;
    private float temporizadorAnimAccion = 0f;
    private bool estaOcupado = false;
    private bool haciendoEmbestida = false;
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

        if (panelJuegoTerminado != null)
        {
            panelJuegoTerminado.SetActive(false);
        }

        if (botonReiniciar != null)
        {
            botonReiniciar.onClick.AddListener(ReiniciarJuego);
        }
    }

    void Update()
    {
        if (estaMuerto) return;

        if (temporizadorGolpe > 0f)
        {
            temporizadorGolpe -= Time.deltaTime;
        }

        if (temporizadorEmbestida > 0f)
        {
            temporizadorEmbestida -= Time.deltaTime;
        }

        if (haciendoEmbestida)
        {
            transform.Translate(Vector2.right * direccion * velocidadEmbestida * Time.deltaTime);

            if (anim != null)
            {
                anim.SetBool("Caminando", false);
            }
            return;
        }

        if (estaOcupado)
        {
            temporizadorAnimAccion -= Time.deltaTime;
            if (temporizadorAnimAccion <= 0f)
            {
                estaOcupado = false;
            }

            if (jugadorEnZona != null)
            {
                MirarHaciaJugador(jugadorEnZona);
            }

            if (anim != null)
            {
                anim.SetBool("Caminando", false);
            }
            return;
        }

        if (jugadorEnZona != null)
        {
            MirarHaciaJugador(jugadorEnZona);

            float distanciaX = Mathf.Abs(jugadorEnZona.transform.position.x - transform.position.x);

            if (temporizadorEmbestida <= 0f && distanciaX >= distanciaMinEmbestida && distanciaX <= distanciaMaxEmbestida)
            {
                EjecutarEmbestida(jugadorEnZona);
                return;
            }

            if (anim != null)
            {
                anim.SetBool("Caminando", false);
            }

            if (temporizadorGolpe <= 0f)
            {
                EjecutarGolpe(jugadorEnZona);
            }
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

    void MirarHaciaJugador(JugadorControlador jugador)
    {
        if (jugador == null) return;

        float diferenciaX = jugador.transform.position.x - transform.position.x;
        if (Mathf.Abs(diferenciaX) > 0.05f)
        {
            direccion = diferenciaX > 0f ? 1 : -1;
            AplicarFlip();
        }
    }

    void EjecutarGolpe(JugadorControlador jugador)
    {
        MirarHaciaJugador(jugador);
        temporizadorGolpe = cooldownGolpe;
        temporizadorAnimAccion = duracionGolpe;
        estaOcupado = true;

        if (anim != null)
        {
            anim.SetBool("Caminando", false);
            anim.ResetTrigger("Atacar");
            anim.SetTrigger("Atacar");
        }

        StartCoroutine(AplicarDanoTrasRetraso(jugador, danoGolpeNormal));
    }

    void EjecutarEmbestida(JugadorControlador jugador)
    {
        MirarHaciaJugador(jugador);
        temporizadorEmbestida = cooldownEmbestida;
        temporizadorAnimAccion = duracionEmbestida + retrasoSalidaEmbestida;
        estaOcupado = true;

        if (anim != null)
        {
            anim.SetBool("Caminando", false);
            anim.ResetTrigger("Habilidad");
            anim.SetTrigger("Habilidad");
        }

        StartCoroutine(SecuenciaEmbestida(jugador));
    }

    private IEnumerator SecuenciaEmbestida(JugadorControlador jugador)
    {
        yield return new WaitForSeconds(retrasoSalidaEmbestida);

        if (estaMuerto) yield break;

        haciendoEmbestida = true;
        bool danoAplicado = false;
        float tiempoRestante = duracionEmbestida;

        while (tiempoRestante > 0f && !estaMuerto)
        {
            if (!danoAplicado && jugador != null)
            {
                float distX = Mathf.Abs(transform.position.x - jugador.transform.position.x);

                if (distX <= rangoDanoEmbestida && jugador.EnSuelo)
                {
                    jugador.RecibirDano(danoEmbestida);
                    danoAplicado = true;
                }
            }

            tiempoRestante -= Time.deltaTime;
            yield return null;
        }

        haciendoEmbestida = false;
        estaOcupado = false;
    }

    private IEnumerator AplicarDanoTrasRetraso(JugadorControlador jugador, int cantidadDano)
    {
        yield return new WaitForSeconds(retrasoDanoGolpe);

        if (estaMuerto || jugador == null || jugadorEnZona == null) yield break;

        jugador.RecibirDano(cantidadDano);
    }

    void Patrullar()
    {
        transform.Translate(Vector2.right * direccion * velocidadPatrulla * Time.deltaTime);

        if (anim != null)
        {
            anim.SetBool("Caminando", velocidadPatrulla > 0.01f);
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
    }

    void Morir()
    {
        estaMuerto = true;
        estaOcupado = false;
        haciendoEmbestida = false;
        jugadorEnZona = null;

        float duracionAudio = 0f;

        if (sonidoMuerte != null)
        {
            AudioSource.PlayClipAtPoint(sonidoMuerte, transform.position, volumenMuerte);
            duracionAudio = sonidoMuerte.length;
        }

        if (anim != null)
        {
            anim.SetBool("Caminando", false);
            anim.ResetTrigger("Atacar");
            anim.ResetTrigger("Habilidad");
            anim.SetTrigger("Morir");
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        StartCoroutine(SecuenciaMuerteYFinJuego(duracionAudio));
    }

    private IEnumerator SecuenciaMuerteYFinJuego(float duracionAudio)
    {
        float espera = Mathf.Max(1.0f, duracionAudio);
        yield return new WaitForSeconds(espera);

        if (panelJuegoTerminado != null)
        {
            panelJuegoTerminado.SetActive(true);
            Time.timeScale = 0f;
        }

        gameObject.SetActive(false);
    }

    public void ReiniciarJuego()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}