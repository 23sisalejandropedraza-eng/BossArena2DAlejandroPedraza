using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JugadorControlador : MonoBehaviour, IDaniable
{
    [Header("Movimiento")]
    public float velocidad = 6f;
    public float fuerzaSalto = 11f;

    [Header("Comprobacion de suelo")]
    public Transform puntoSuelo;
    public float radioSuelo = 0.2f;
    public LayerMask capaSuelo;

    [Header("Ataque")]
    public Transform puntoAtaque;
    public float radioAtaque = 0.7f;
    public int danoAtaque = 1;
    public float tiempoEntreAtaques = 0.35f;

    [Header("Vida")]
    public int vidaMaxima = 5;
    public float tiempoInvulnerable = 1f;
    private int vidaActual;
    private bool estaMuerto;
    private float finInvulnerabilidad;

    private Rigidbody2D rb;
    private Animator anim;
    private float moverX;
    private bool enSuelo;
    private float siguienteAtaque;
    private bool enAgua;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        vidaActual = vidaMaxima;
    }

    void Update()
    {
        if (estaMuerto)
        {
            moverX = 0f;
            ActualizarAnimaciones();
            return;
        }

        moverX = Input.GetAxisRaw("Horizontal");

        if (moverX > 0.01f)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (moverX < -0.01f)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        if (puntoSuelo != null)
        {
            enSuelo = Physics2D.OverlapCircle(puntoSuelo.position, radioSuelo, capaSuelo);
        }

        if (Input.GetKeyDown(KeyCode.Space) && enSuelo)
        {
            rb.velocity = new Vector2(rb.velocity.x, fuerzaSalto);
        }

        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.J)) && Time.time >= siguienteAtaque)
        {
            siguienteAtaque = Time.time + tiempoEntreAtaques;
            Atacar();
        }

        ActualizarAnimaciones();
    }

    void FixedUpdate()
    {
        if (estaMuerto)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            return;
        }

        rb.velocity = new Vector2(moverX * velocidad, rb.velocity.y);
    }

    void Atacar()
    {
        if (anim != null)
        {
            anim.SetTrigger("Atacar");
        }

        if (puntoAtaque == null) return;

        Collider2D[] golpeados = Physics2D.OverlapCircleAll(puntoAtaque.position, radioAtaque);
        foreach (Collider2D col in golpeados)
        {
            if (col.gameObject == gameObject) continue;

            if (col.TryGetComponent<IDaniable>(out IDaniable objetivo))
            {
                objetivo.RecibirDano(danoAtaque);
            }
        }
    }

    public void RecibirDano(int cantidad)
    {
        if (estaMuerto || Time.time < finInvulnerabilidad) return;

        finInvulnerabilidad = Time.time + tiempoInvulnerable;
        vidaActual -= cantidad;

        if (vidaActual <= 0)
        {
            vidaActual = 0;
            Morir();
        }
    }

    public void Morir()
    {
        if (estaMuerto) return;

        estaMuerto = true;
        moverX = 0f;

        if (rb != null)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }

        if (anim != null)
        {
            anim.SetBool("Corriendo", false);
            anim.SetBool("Muerto", true);
            anim.SetTrigger("Morir");
        }

        StartCoroutine(FrenarCaidaMuerte());
    }

    public void MarcarEnAgua(bool estado)
    {
        enAgua = estado;
    }

    private IEnumerator FrenarCaidaMuerte()
    {
        float tiempoEspera = 0f;
        while (tiempoEspera < 2f)
        {
            if (enAgua && rb != null && rb.velocity.y < -0.5f)
            {
                yield return new WaitForSeconds(0.07f);
                break;
            }
            tiempoEspera += Time.deltaTime;
            yield return null;
        }

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
        }
    }

    public bool EstaMuerto()
    {
        return estaMuerto;
    }

    void ActualizarAnimaciones()
    {
        if (anim == null || estaMuerto) return;

        anim.SetBool("Corriendo", Mathf.Abs(moverX) > 0.01f);
        anim.SetBool("EnSuelo", enSuelo);
    }

    void OnDrawGizmosSelected()
    {
        if (puntoSuelo != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(puntoSuelo.position, radioSuelo);
        }

        if (puntoAtaque != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(puntoAtaque.position, radioAtaque);
        }
    }
}