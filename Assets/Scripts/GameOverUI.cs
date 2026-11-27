using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    public GameObject panelDerrota;

    void Start()
    {
        if (panelDerrota != null)
        {
            panelDerrota.SetActive(false);
        }
    }
}
