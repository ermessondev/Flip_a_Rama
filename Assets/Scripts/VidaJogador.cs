using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VidaJogador : MonoBehaviour
{
    public Image barraVida;
    public Image barraDano;

    //tem q ser publico ai fiz esse HideInspector
    [HideInInspector] public float vida;
    [SerializeField] private float vidaMaxima;
    [SerializeField] private float velocidadeDano = 0.005f;


    private void Start()
    {
        vida = vidaMaxima;
    }
    private void Update()
    {
        barraVida.fillAmount = vida / vidaMaxima;
        if (barraDano.fillAmount > barraVida.fillAmount)
        {
            barraDano.fillAmount -= velocidadeDano;
        }
        else
        {
            barraDano.fillAmount = barraVida.fillAmount;
        }
    }
}
