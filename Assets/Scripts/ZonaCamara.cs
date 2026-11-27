using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonaCamara : MonoBehaviour
{
    public CamaraPorSala camara;
    public Transform puntoCamaraSala;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (camara != null && puntoCamaraSala != null)
            {
                camara.CambiarSala(puntoCamaraSala);
            }
        }
    }
}
