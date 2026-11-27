using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GestorProgresoJugador : MonoBehaviour
{
    public static GestorProgresoJugador Instancia { get; private set; }

    [Header("Referencias")]
    public JugadorControlador jugador;
    public VialesJugador vialesJugador;

    [Header("3 Checkpoints (0: Inicio, 1: Segundo, 2: Tercero)")]
    public Transform[] puntosCheckpoint = new Transform[3];

    [Header("Reaparicion y muertes")]
    public int maxMuertes = 3;
    public float retrasoReaparicion = 1.5f;

    [Header("UI Vidas y Game Over (3 muertes)")]
    public TMP_Text textoVidas;
    public string prefijoTextoVidas = "Numero de vidas: ";
    public GameObject panelGameOver;
    public Button botonReiniciarGameOver;

    private int checkpointActual = 0;
    private Vector3 posicionReaparicionActual;
    private int muertesActuales = 0;
    private bool procesandoMuerte = false;

    void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;

        if (jugador == null)
        {
            jugador = FindObjectOfType<JugadorControlador>();
        }

        if (vialesJugador == null)
        {
            vialesJugador = FindObjectOfType<VialesJugador>();
        }
    }

    void Start()
    {
        checkpointActual = 0;
        muertesActuales = 0;

        if (puntosCheckpoint != null && puntosCheckpoint.Length > 0 && puntosCheckpoint[0] != null)
        {
            posicionReaparicionActual = puntosCheckpoint[0].position;
        }
        else if (jugador != null)
        {
            posicionReaparicionActual = jugador.transform.position;
        }

        if (panelGameOver != null)
        {
            panelGameOver.SetActive(false);
        }

        if (botonReiniciarGameOver != null)
        {
            botonReiniciarGameOver.onClick.AddListener(ReiniciarDesdeCero);
        }

        ActualizarTextoVidas();
    }

    public void ActivarCheckpoint(int indice, Vector3 posicion)
    {
        if (indice >= checkpointActual)
        {
            checkpointActual = indice;
            posicionReaparicionActual = posicion;
        }
    }

    public void NotificarMuerteJugador()
    {
        if (procesandoMuerte) return;
        StartCoroutine(SecuenciaMuerteJugador());
    }

    private IEnumerator SecuenciaMuerteJugador()
    {
        procesandoMuerte = true;
        muertesActuales++;
        ActualizarTextoVidas();

        yield return new WaitForSeconds(retrasoReaparicion);

        if (muertesActuales >= maxMuertes)
        {
            if (panelGameOver != null)
            {
                panelGameOver.SetActive(true);
            }
            Time.timeScale = 0f;
            procesandoMuerte = false;
            yield break;
        }

        if (jugador != null)
        {
            jugador.ReaparecerEn(posicionReaparicionActual);
        }

        if (vialesJugador != null)
        {
            vialesJugador.RestaurarViales();
        }

        procesandoMuerte = false;
    }

    private void ActualizarTextoVidas()
    {
        if (textoVidas == null) return;

        int vidasRestantes = Mathf.Max(0, maxMuertes - muertesActuales);
        textoVidas.text = prefijoTextoVidas + vidasRestantes;
    }

    public void ReiniciarDesdeCero()
    {
        Time.timeScale = 1f;
        muertesActuales = 0;
        checkpointActual = 0;
        procesandoMuerte = false;

        if (puntosCheckpoint != null && puntosCheckpoint.Length > 0 && puntosCheckpoint[0] != null)
        {
            posicionReaparicionActual = puntosCheckpoint[0].position;
        }

        if (vialesJugador != null)
        {
            vialesJugador.RestaurarViales();
        }

        ActualizarTextoVidas();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}