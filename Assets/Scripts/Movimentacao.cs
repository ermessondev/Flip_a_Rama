using System.Collections;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

// Caso o objeto não tenha o componente, ele é criado em tempo de compilação
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(BoxCollider2D))]
public class Movimentacao : MonoBehaviour
{

    public float vida = 100;

    private PlayerInput playerInput;

    // Variéveis  dedicadas a mecenica de movimentação
    protected Rigidbody2D rb;
    protected Vector2 direcao;
    [SerializeField] private float velocidade = 3f;
    [SerializeField] private float forcaPulo = 3f;

    // Variéveis dedicadas a mecánica de dash
    private bool puloDuploHabilitado = false;
    private bool dashDisponivel = true;
    private bool emDash = false;
    [SerializeField] private float forcaDash = 15f;

    // Variéveis de controle de plataforma/chão/layers
    public bool estaNoChao = false;
    protected bool estaNaPlataforma = false;
    Transform peDoPersonagem;
    private LayerMask Ground;
    private LayerMask Platform;
    private LayerMask PlayerMask;
    RaycastHit2D hitPlataforma;
    [SerializeField] private float raioVerificaChao = 0.30f;
    [SerializeField] private float distanciaVerificaPlataforma = 0.20f;

    // Variéveis de Animação
    [SerializeField] protected Animator oAnimator;

    [Header("Configurações do Combo")]
    [SerializeField] private float tempoEntreGolpes = 1f;   // Tempo máximo entre ataques para manter o combo
    private float duracaoGolpe;                             // Duração da animação do golpe
    [SerializeField] private int maximoCombo = 3;           // Quantos golpes no combo

    [Header("Configurações do Ataque")]
    [SerializeField] private Transform pontoAtaque;         // Centro da hitbox do ataque
    [SerializeField] private Vector2 tamanhoAtaque;         // Tamanho da hitbox do ataque
    [SerializeField] private BoxCollider2D defesaInimigo;   // Hitbox específica de defesa do inimigo

    private int comboIndice = 0;                            // Indica qual golpe do combo está ativo
    private bool podeAtacar = true;                         // Evita spam de ataques
    private Coroutine resetComboCoroutine;                  // Coroutine que reseta o combo por tempo

    public bool acertouDammy = false;

    // Variéveis  de controle para queda através da plataforma
    private bool descendoDaPlataforma = false;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        if (oAnimator == null)
        {
            oAnimator = GetComponent<Animator>();
        }

        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Referência ao "pé" do personagem para checar contato com o chão/plataforma
        peDoPersonagem = transform.Find("pePlayer");

        // Máscaras de layer usadas nas verificaçães de colisão
        Ground = LayerMask.GetMask("Ground");
        Platform = LayerMask.GetMask("Platform");
        PlayerMask = LayerMask.GetMask("Player");
    }


    void Update()
    {
        // Inicia as animações
        RodarAnimacoesHorizontal();
        RodarAnimacoesVertical();
        EspelharJogador();

        // Verifica se o personagem está no chão (retorna true se houver um colisor no raio definido)
        estaNoChao = Physics2D.OverlapCircle(peDoPersonagem.position, raioVerificaChao, Ground) != null;

        // Verifica se o personagem está encostando em uma plataforma logo abaixo do pé
        hitPlataforma = Physics2D.Linecast(
            peDoPersonagem.position,
            (Vector2)peDoPersonagem.position + (Vector2.down * distanciaVerificaPlataforma),
            Platform
        );
        estaNaPlataforma = hitPlataforma.collider != null;

        // Controle do pulo duplo: recarrega ao tocar o chão ou uma plataforma
        if (estaNaPlataforma || estaNoChao)
        {
            puloDuploHabilitado = true;
        }

        // Controle de colis�o com as plataformas:
        // - Se não est� sobre a plataforma, ignora colisão (permite atravessar de baixo para cima)
        // - Se está sobre a plataforma e n�o está descendo, reativa a colisão
        if (!estaNaPlataforma)
        {
            Physics2D.IgnoreLayerCollision(7, 8, true);
        }
        else if (!descendoDaPlataforma)
        {
            Physics2D.IgnoreLayerCollision(7, 8, false);
        }
    }

    private void FixedUpdate()
    {
        // Se na estiver em dash, aplica a movimentação horizontal usando o valor armazenado em 'direcao' (setado em OnMove)
        if (!emDash)
        {
            rb.linearVelocity = new Vector2(direcao.x * velocidade, rb.linearVelocity.y);
        }
    }

    void OnMove(InputValue valor)
    {
        // L� o valor vindo do Input System e armazena a direção desejada
        direcao = valor.Get<Vector2>();
    }

    void OnJump()
    {
        // Pula se estiver no chão ou sobre uma plataforma; caso contrário, usa o pulo duplo se estiver habilitado
        if (estaNoChao || estaNaPlataforma)
        {
            pular();
        }
        else if (puloDuploHabilitado)
        {
            puloDuploHabilitado = false;
            pular();
        }
    }

    void OnDash()
    {
        // Inicia o dash se estiver disponível
        if (dashDisponivel == true)
        {
            StartCoroutine(usarDash());
        }
    }

    void OnFallOfPlatform()
    {
        // Inicia a descida atravessando a plataforma (temporariamente ignora colisão)
        StartCoroutine(descerPlataforma());
    }

    private void pular()
    {
        // Zera a velocidade vertical antes de aplicar o impulso de pulo (para padronizar a altura)
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * forcaPulo, ForceMode2D.Impulse);
    }

    private IEnumerator usarDash()
    {
        dashDisponivel = false;
        emDash = true;

        // Desliga a gravidade durante o dash para um impulso mais reto
        rb.gravityScale = 0f;

        // Se o pulo duplo foi gasto, recarrega ao usar o dash (mecânica intencional)
        if (!puloDuploHabilitado)
        {
            puloDuploHabilitado = true;
        }

        // Aplica o dash para a esquerda/direita conforme a direção horizontal
        if (direcao.x == -1)
        {
            rb.AddForce(Vector2.left * forcaDash, ForceMode2D.Impulse);
        }
        else if (direcao.x == 1)
        {
            rb.AddForce(Vector2.right * forcaDash, ForceMode2D.Impulse);
        }

        // Dura��o do dash
        yield return new WaitForSeconds(0.3f);

        // Finaliza o dash, restaura a gravidade e zera a velocidade
        emDash = false;
        rb.gravityScale = 1;
        rb.linearVelocity = Vector2.zero;

        // Tempo de recarga até o próximo dash
        yield return new WaitForSeconds(2f);
        dashDisponivel = true;
    }

    private IEnumerator descerPlataforma()
    {
        // Se está sobre a plataforma, permite cair por ela ignorando a colisão por um curto período
        if (estaNaPlataforma == true)
        {
            descendoDaPlataforma = true;
            Physics2D.IgnoreLayerCollision(7, 8, true);
            yield return new WaitForSeconds(0.5f);
            descendoDaPlataforma = false;
        }
    }

    void RodarAnimacoesHorizontal()
    {
        // Roda as animcações Horizontais - Animções de Walk e Idle
        oAnimator.SetFloat("AnimHorizontal", rb.linearVelocity.x);
    }

    void RodarAnimacoesVertical()
    {
        // Roda as animcações Verticais - Animções de Jump e ve se o pesrongem esta no chão pra voltar a animção de Idle
        oAnimator.SetFloat("AnimVertical", rb.linearVelocity.y);
        oAnimator.SetBool("EstaNoChao", estaNoChao || estaNaPlataforma);
    }

    void EspelharJogador()
    {

        // Faz o Jogador olhar para a direção que esta andando - Espelha o Sprite (direita / esquerda)

        if (direcao.x == 1)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (direcao.x == -1)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }

    }

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
        // Inicia animação do soco
            switch (golpe)
        {
        case 1:
            oAnimator.SetBool("Punch", true);
            duracaoGolpe = 0.6f;
            break;
        case 2:
            oAnimator.SetBool("Punch_2", true);
            duracaoGolpe = 0.8f;
            break;
        case 3:
            oAnimator.SetBool("Punch_3", true);
            duracaoGolpe = 0.8f;
            break;
        }

        // Checa se o ataque acertou a hitbox de defesa do inimigo
        Collider2D defendeu = Physics2D.OverlapBox(pontoAtaque.position, tamanhoAtaque, 0f);

        // Se acertou ativa o ComboBloqueado 
        if (defendeu == defesaInimigo)
        {
            ComboBloqueado();
            acertouDammy = true;
        } 

        yield return new WaitForSeconds(duracaoGolpe);  // Espera o tempo da animação mesmo se bloqueado

        // Para animação
        switch (golpe)
        {
        case 1:
            oAnimator.SetBool("Punch", false);
            break;
        case 2:
            oAnimator.SetBool("Punch_2", false);
            break;
        case 3:
            oAnimator.SetBool("Punch_3", false);
            break;
        }

        podeAtacar = true;
        acertouDammy = false;

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("limitador"))
        {
            Debug.Log("O player entrou no trigger do limitador!");

            rb.linearVelocity = Vector2.zero;

        }
    }
}