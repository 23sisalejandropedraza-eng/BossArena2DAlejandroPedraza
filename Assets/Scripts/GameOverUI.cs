using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public JugadorControlador jugador;
    public GameObject panelDerrota;

    private bool mostrado;

    void Start()
    {
        if (panelDerrota != null)
        {
            panelDerrota.SetActive(false);
        }
    }

    void Update()
    {
        if (!mostrado && jugador != null && jugador.EstaMuerto())
        {
            mostrado = true;
            if (panelDerrota != null)
            {
                panelDerrota.SetActive(true);
            }
        }
    }

    public void Reintentar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
