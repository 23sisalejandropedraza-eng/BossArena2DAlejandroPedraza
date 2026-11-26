using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraAvanceDerecha : MonoBehaviour
{
    public Transform jugador;
    public float xMin = 0f;   // Punto X a partir del cual empieza a avanzar a la derecha
    public float xMax = 20f;  // Límite X máximo en la segunda zona (ajústalo a tu nivel)
    public float suavizado = 5f;

    private float yFija;

    void Start()
    {
        yFija = transform.position.y;
    }

    void LateUpdate()
    {
        if (jugador == null) return;

        float objetivoX = transform.position.x;

        if (jugador.position.x > xMin)
        {
            objetivoX = Mathf.Clamp(jugador.position.x, xMin, xMax);
        }

        Vector3 posicionDeseada = new Vector3(objetivoX, yFija, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, posicionDeseada, suavizado * Time.deltaTime);
    }
}