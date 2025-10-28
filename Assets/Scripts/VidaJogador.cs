using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VidaJogador : MonoBehaviour
{
    public Toggle danoLeve;
    public Toggle danoMedio;
    public Toggle danoAlto;
    public float golpe1;
    public float golpe2;
    public float golpe3;

    public Image barraVida;
    public Image barraDano;
    public float vidaMaxima;
    public float vidaAtual;

    void Start()
    {
        vidaAtual = vidaMaxima;
        barraVida.fillAmount = vidaAtual;

        AtualizarBarraDeVida();
    }

    public void SetdanoLeve(float golpe)
    {
        if (danoLeve != true)
        { 
            vidaAtual -= 0.15f;
        }
    }
    public void SetdanoMedio(float golpe)
    {
        if (danoMedio != true)
        {
            vidaAtual -= 0.25f;
        }
    }
    public void SetdanoAlto(float golpe)
    {
        if (danoAlto != true)
        {
            vidaAtual -= 0.40f;
        }
    }

    public void SetDano(int dano)
    {
        barraDano.fillAmount = dano;
        if (barraVida.fillAmount <= 1)
        {
            vidaMaxima = dano;
        }
    }

    public void ReceberDano(float golpe)
    {
        vidaAtual -= golpe;
        vidaAtual = Mathf.Clamp(vidaAtual, 0, vidaMaxima);

        AtualizarBarraDeVida();
        if (vidaAtual <= 0)
        {
            KO();
        }
        AtualizarBarraDeDano();
        if (barraVida.fillAmount == 0)
        {
            KO();
        }
    }

    void AtualizarBarraDeVida()
    {
        if (barraVida != null)
        {
            barraVida.fillAmount = vidaAtual / vidaMaxima;
        }
    }

    void AtualizarBarraDeDano()
    {
        if (barraVida != null)
        {
            barraDano.fillAmount = vidaAtual / vidaMaxima;
        }
    }

    void KO()
    {
        Debug.Log("KnockOut!");
    }
    
}
