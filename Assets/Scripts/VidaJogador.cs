using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VidaJogador : MonoBehaviour
{
    public Image barraVida;
    public int vidaMaxima;
    public void SetVida(int vida)
    {
        barraVida.fillAmount = vida;
        if (barraVida.fillAmount >= 1)
        {
            vida = vidaMaxima;
        }
    }

    void SetVidaMaxima()
    {
        barraVida.fillAmount = 1;
    }
    
}
