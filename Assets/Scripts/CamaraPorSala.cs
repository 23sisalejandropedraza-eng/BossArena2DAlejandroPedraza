using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraPorSala : MonoBehaviour
{
    public Transform objetivoActual;
    public float velocidadSuavizado = 8f;

    void Start()
    {
        if (objetivoActual != null)
        {
            transform.position = new Vector3(
                objetivoActual.position.x,
                objetivoActual.position.y,
                transform.position.z
            );
        }
    }

    void LateUpdate()
    {
        if (objetivoActual == null) return;

        Vector3 destino = new Vector3(
            objetivoActual.position.x,
            objetivoActual.position.y,
            transform.position.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            destino,
            velocidadSuavizado * Time.deltaTime
        );
    }

    public void CambiarSala(Transform nuevoObjetivo)
    {
        objetivoActual = nuevoObjetivo;
    }
}
