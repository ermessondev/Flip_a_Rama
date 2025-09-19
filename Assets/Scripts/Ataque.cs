using System.Collections;
using UnityEngine;

public class Ataque : Movimentacao
{
    [Header("Configurações do Combo")]
    [SerializeField] private float tempoEntreGolpes = 1f; // Tempo máximo para apertar de novo / para o combo nao resetar
    [SerializeField] private float duracaoGolpe = 0.4f;   // Quanto tempo a animação dura
    [SerializeField] private int maximoCombo = 3;         // Limite de golpes no combo

    private int comboIndice = 0;         // Qual golpe está
    private bool podeAtacar = true;     // Evita spam
    private Coroutine resetComboCoroutine;

    // Todos os Debug.Log são de teste por enquanto 

    void OnAttack()
    {
        if (podeAtacar == false) 
        {
            return;
        }

        comboIndice++;

        if (comboIndice > maximoCombo)
        {
            comboIndice = 1;
        }

        Debug.Log("Ataque pressionado - Golpe atual: " + comboIndice);

        // Inicia a coroutine
        StartCoroutine(ExecutarGolpe(comboIndice));

        if (resetComboCoroutine != null)
        {
            StopCoroutine(resetComboCoroutine);
        }

        // Inicia uma nova coroutine para resetar o combo depois de (tempoEntreGolpes)
        // Coroutine reinicia o combo se o jogador não apertar ataque dentro do tempo (tempoEntreGolpes)
        resetComboCoroutine = StartCoroutine(ResetarComboDepoisDoTempo());
    }

    private IEnumerator ExecutarGolpe(int golpe)
    {
        podeAtacar = false;

        Debug.Log("Executando golpe " + golpe);

        // Inicia a animção de soco
        oAnimator.SetBool("Punch", true);

        // espera a duração do golpe para setar False ápos o tempo
        yield return new WaitForSeconds(duracaoGolpe);

        // Pausa a animção de soco
        oAnimator.SetBool("Punch", false);

        podeAtacar = true;

        // Se chegou no último golpe do combo reseta o combo
        if (golpe >= maximoCombo)
        {
            Debug.Log("Combo finalizado");
            comboIndice = 0;
        }
    }

    // Resata o combo ser passar do tempo entre os golpes 
    private IEnumerator ResetarComboDepoisDoTempo()
    {
        yield return new WaitForSeconds(tempoEntreGolpes);

        Debug.Log("Combo resetado (tempo expirou)");

        comboIndice = 0;
    }

    // Resetar combo se o adversário bloquear
    public void EstaBloqueado()
    {
        Debug.Log("Ataque bloqueado - Combo resetado");

        // Interrompe qualquer golpe em andamento
        StopAllCoroutines();

        // Reseta tudo
        comboIndice = 0;
        podeAtacar = true;

        // Pausa a animção de soco
        oAnimator.SetBool("Socando", false);
    }
}
