using System.Collections;
using UnityEngine;

public class Ataque : Movimentacao
{
    [Header("Configurações do Combo")]
    [SerializeField] private float tempoEntreGolpes = 1f;   // Tempo máximo entre ataques para manter o combo
    [SerializeField] private float duracaoGolpe = 0.4f;     // Duração da animação do golpe
    [SerializeField] private int maximoCombo = 3;           // Quantos golpes no combo

    [Header("Configurações do Ataque")]
    [SerializeField] private Transform pontoAtaque;         // Centro da hitbox do ataque
    [SerializeField] private Vector2 tamanhoAtaque;         // Tamanho da hitbox do ataque
    [SerializeField] private BoxCollider2D defesaInimigo;   // Hitbox específica de defesa do inimigo

    private int comboIndice = 0;                            // Indica qual golpe do combo está ativo
    private bool podeAtacar = true;                         // Evita spam de ataques
    private Coroutine resetComboCoroutine;                  // Coroutine que reseta o combo por tempo

    // Todos os Debug.Log são para testes
    
    void OnAttack()
    {
        if (emDash)
        {
            Debug.Log("Não pode atacar durante o dash");
            return;
        }

        if (!podeAtacar)
        {
            return;
        }

        comboIndice++;
        if (comboIndice > maximoCombo)
        {
            comboIndice = 1;
        }

        Debug.Log("Ataque pressionado - Golpe atual: " + comboIndice);

        // Inicia a coroutine do golpe
        StartCoroutine(ExecutarGolpe(comboIndice));

        // Reseta o timer do combo
        if (resetComboCoroutine != null)
        {
            StopCoroutine(resetComboCoroutine);
        }
        resetComboCoroutine = StartCoroutine(ResetarComboDepoisDoTempo());
    }

    private IEnumerator ExecutarGolpe(int golpe)
    {
        podeAtacar = false;
        oAnimator.SetBool("Punch", true); // Inicia animação do soco

        // Checa se o ataque acertou a hitbox de defesa do inimigo
        Collider2D defendeu = Physics2D.OverlapBox(pontoAtaque.position, tamanhoAtaque, 0f);

        // Se acertou ativa o ComboBloqueado
        if (defendeu == defesaInimigo)
        {
            ComboBloqueado();
        }

        yield return new WaitForSeconds(duracaoGolpe);  // Espera o tempo da animação mesmo se bloqueado

        oAnimator.SetBool("Punch", false); // Para animação
        podeAtacar = true;

        // Se chegou no último golpe do combo, reseta o combo automaticamente
        if (comboIndice >= maximoCombo)
        {
            Debug.Log("Combo finalizado");
            comboIndice = 0;

            // Para a coroutine de tempo entre golpes, se estiver rodando
            if (resetComboCoroutine != null)
            {
                StopCoroutine(resetComboCoroutine);
                resetComboCoroutine = null;
            }
        }
    }

    private IEnumerator ResetarComboDepoisDoTempo()
    {
        yield return new WaitForSeconds(tempoEntreGolpes);
        comboIndice = 0;
        Debug.Log("Combo resetado (tempo expirou)");
    }

    // Reseta o combo se o ataque for bloquedo
    private void ComboBloqueado()
    {
        Debug.Log("Ataque bloqueado - Combo resetado");
        comboIndice = 0;

        // Para a coroutine de tempo entre golpes, se estiver rodando
        if (resetComboCoroutine != null)
        {
            StopCoroutine(resetComboCoroutine);
            resetComboCoroutine = null;
        }
    }

    // Desenha a hitbox do ataque no editor
    private void OnDrawGizmos()
    {
        if (pontoAtaque != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(pontoAtaque.position, tamanhoAtaque);
        }
    }
}
