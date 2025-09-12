using System.Collections;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Ataque : Movimentacao
{
    private bool tempoSoco = false;

    void OnAttack()
    {
        // Roda as animcação de Soco - Animção de Punch
        // Seta o tempo do soco true
        // Inicia o timer da animação do soco
        StartCoroutine(VoltarAnimacao());
    }

    private IEnumerator VoltarAnimacao()
    {
        // Espera 0.4s e volta a animcao para Idle/deixa o bool false
        // Volta a animação do soco para Idle/Walk ou Jump
        tempoSoco = true;
        oAnimator.SetBool("Socando", tempoSoco);
        yield return new WaitForSeconds(0.4f);
        tempoSoco = false;
        oAnimator.SetBool("Socando", tempoSoco);
    }
}
