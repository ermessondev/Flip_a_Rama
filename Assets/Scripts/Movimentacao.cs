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

    // Variáveis de Áudio e Som
    [SerializeField] AudioSource som;
    [SerializeField] private AudioClip sairDaArenaSFX;
    [SerializeField] private AudioClip socoSFX;
    [SerializeField] public AudioClip dashSFX;
    
    // Variéveis  dedicadas a mecenica de movimentação
    protected Rigidbody2D rb;
    protected Vector2 direcao;
    [SerializeField] private float velocidade = 3f;
    [SerializeField] private float forcaPulo = 5f;

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
    [SerializeField] private float tempoEntreGolpes = 1f;       // Tempo máximo entre ataques para manter o combo
    [SerializeField] private int maximoCombo = 3;               // Quantos golpes no combo
    private float duracaoGolpe;                                 // Duração da animação do golpe
    private float frameParaAcerto;
    private float esperaAtaque;

    [Header("Configurações do Ataque")]
    [SerializeField] private Transform hitboxPunch_01;         // Centro da hitbox do ataque
    [SerializeField] private Transform hitboxPunch_02;
    [SerializeField] private Transform hitboxPunch_03;
    [SerializeField] private Vector2 tamanhoAtaque_01;         // Tamanho da hitbox do ataque
    [SerializeField] private Vector2 tamanhoAtaque_02;
    [SerializeField] private Vector2 tamanhoAtaque_03;
    [SerializeField] private BoxCollider2D defesaInimigo;   // Hitbox específica de defesa do inimigo 
    [SerializeField] private BoxCollider2D hitboxCorpoPlayer;     
    [SerializeField] private BoxCollider2D hitboxPePlayer;
    [SerializeField] private BoxCollider2D hitboxCabecaPlayer;
    private bool ehJogador1;
    private string outroNome;
    public bool acertouDammy = false;
    private bool emBlock = false;
    private int golpeTomado;

    [Header("Configurações do Shake/Frame freeze")]
    [SerializeField] private float shakeDuracao = 0.1f;
    [SerializeField] private float shakeMagnitude = 0.2f;
    [SerializeField] private CameraAutoFit cameraAutoFit;
    [SerializeField] private float duracaoFreezeFrame = 0.1f;   // tempo de freeze frame em segundos
    
    private int comboIndice = 0;                            // Indica qual golpe do combo está ativo
    private bool podeAtacar = true;                         // Evita spam de ataques
    private Coroutine resetComboCoroutine;                  // Coroutine que reseta o combo por tempo

    // Variéveis  de controle para queda através da plataforma
    private bool descendoDaPlataforma = false;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        if (oAnimator == null)
        {
            oAnimator = GetComponentInChildren<Animator>();
            if (oAnimator == null)
            {
                Debug.Log("Nenhum Animator encontrado no Player ou nos filhos!");
            }
        }

        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Referência ao "pé" do personagem para checar contato com o chão/plataforma
        peDoPersonagem = transform.Find("pePlayer");

        // Máscaras de layer usadas nas verificaçães de colisão
        Ground = LayerMask.GetMask("Ground");
        Platform = LayerMask.GetMask("Platform");
        PlayerMask = LayerMask.GetMask("Player");

        if (cameraAutoFit == null)
        {
            cameraAutoFit = FindFirstObjectByType<CameraAutoFit>();
        }
    }

    void Start()
    {
        //LoL
        if (this.name != "Jogador1")
        {
            this.transform.localScale = new Vector3(-1f, 1f, 1f);
        }


        if (defesaInimigo == null)
        {
            if (gameObject.name == "Jogador1")
            {
                // Procura o jogador 2 e usa o BoxCollider2D dele como hitbox de defesa
                GameObject jogador2 = GameObject.Find("Jogador2");
                if (jogador2 == null)
                {
                    jogador2 = GameObject.Find("Jogador2 (IA)");
                }

                if (jogador2 != null)
                {
                    defesaInimigo = jogador2.GetComponentInChildren<BoxCollider2D>();
                }
            }
            else
            {
                // Procura o jogador 1 e usa o BoxCollider2D dele como hitbox de defesa
                GameObject jogador1 = GameObject.Find("Jogador1");
                if (jogador1 != null)
                {
                    defesaInimigo = jogador1.GetComponentInChildren<BoxCollider2D>();
                }
            }
        }

        ehJogador1 = gameObject.name.Contains("Jogador1");

        if (ehJogador1)
            outroNome = "Jogador2";
        else
            outroNome = "Jogador1";

        GameObject outroJogador = GameObject.Find(outroNome);

        if (outroJogador == null) return;

        Transform spriteOutro = outroJogador.transform.Find("Sprite");
        if (spriteOutro == null) return;

        Transform hitboxOutro = spriteOutro.Find("Hitbox");
        if (hitboxOutro == null) return;

        hitboxCorpoPlayer = hitboxOutro.Find("Hitbox_Corpo")?.GetComponent<BoxCollider2D>();
        hitboxPePlayer = hitboxOutro.Find("Hitbox_Pe")?.GetComponent<BoxCollider2D>();
        hitboxCabecaPlayer = hitboxOutro.Find("Hitbox_Cabeca")?.GetComponent<BoxCollider2D>();
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

    #region Move
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
        SFX.instance.TocarSFX(dashSFX, transform, 1f, 1.0f);
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
            oAnimator.SetBool("Dash", emDash);
            rb.AddForce(Vector2.left * forcaDash, ForceMode2D.Impulse);
        }
        else if (direcao.x == 1)
        {
            oAnimator.SetBool("Dash", emDash);
            rb.AddForce(Vector2.right * forcaDash, ForceMode2D.Impulse);
        }

        // Dura��o do dash
        yield return new WaitForSeconds(0.3f);

        // Finaliza o dash, restaura a gravidade e zera a velocidade
        emDash = false;
        rb.gravityScale = 1;
        rb.linearVelocity = Vector2.zero;
        oAnimator.SetBool("Dash", emDash);

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
    #endregion
    
    #region Animações/Ataque
    void RodarAnimacoesHorizontal()
    {
        // Roda as animcações Horizontais - Animções de Walk e Idle
        oAnimator.SetFloat("AnimHorizontal", rb.linearVelocity.x);
    }

    void RodarAnimacoesVertical()
    {
        // Roda as animcações Verticais - Animções de Jump e ve se o pesrongem esta no chão pra voltar a animção de Idle
        oAnimator.SetBool("EstaNoChao", estaNoChao || estaNaPlataforma);
        oAnimator.SetFloat("AnimVertical", rb.linearVelocity.y);
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

        public void OnBlock()
    {
        // Inicia a corrotina quando o jogador clica para bloquear
        StartCoroutine(ExecutarBlock());
    }

    private IEnumerator ExecutarBlock()
    {
        // Ativa a animação de bloqueio
        oAnimator.SetBool("Block", true);
        emBlock = true;

        // Espera 5 segundos
        yield return new WaitForSeconds(0.5f);

        // Volta para o Idle
        oAnimator.SetBool("Block", false);
        emBlock = false;
    }
    
    void OnAttack()
    {   
        if (emDash)
        {
            Debug.Log("Não pode atacar durante o Dash");
            return;
        }

        if (emBlock)
        {
            Debug.Log("Não pode atacar durante o Block");
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
    
        // Inicia animação e configura duração de cada golpe
        switch (golpe)
        {
            case 1:
                oAnimator.SetBool("Punch", true);
                duracaoGolpe = 0.5f;
                frameParaAcerto = 0.3f;
                
                break;
            case 2:
                oAnimator.SetBool("Punch_2", true);
                duracaoGolpe = 0.4f;
                frameParaAcerto = 0.2f;
                break;
            case 3:
                oAnimator.SetBool("Punch_3", true);
                duracaoGolpe = 0.4f;
                frameParaAcerto = 0.3f;
                break;
        }

        // Espera até o frame específico do impacto
        yield return new WaitForSeconds(frameParaAcerto);

        // Ativa somente a hitbox do golpe atual
        Collider2D[] acertos = null;
        switch (golpe)
        {
            case 1:
                acertos = Physics2D.OverlapBoxAll(hitboxPunch_01.position, tamanhoAtaque_01, 0f);
                break;
            case 2:
                acertos = Physics2D.OverlapBoxAll(hitboxPunch_02.position, tamanhoAtaque_02, 0f);
                break;
            case 3:
                acertos = Physics2D.OverlapBoxAll(hitboxPunch_03.position, tamanhoAtaque_03, 0f);
                break;
        }

        // Verifica se o ataque acertou a defesa do inimigo
        if (acertos != null)
        {
            foreach (Collider2D hitboxDefesa in acertos)
            {

                if (hitboxDefesa == defesaInimigo)
                {
                    acertouDammy = true;
                    ComboBloqueado();
                    Debug.Log($"Defesa inimigo atingida no golpe {golpe}");
                    break;
                }
            }
        }

        if (acertos != null)
        {
            foreach (Collider2D hitboxCorpo in acertos)
            {

                if (hitboxCorpo == hitboxCorpoPlayer || hitboxCorpo == hitboxCabecaPlayer || hitboxCorpo == hitboxPePlayer)
                {
                    Debug.Log($"O inimigo foi atingido no golpe {golpe}");
                    golpeTomado = golpe;
                    break;
                }
            }
        }
        

        // Espera o restante da animação antes de liberar outro ataque
        yield return new WaitForSeconds(duracaoGolpe - frameParaAcerto);

        // Desativa a animação do golpe
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

        // Se for o último golpe do combo, reseta
        if (comboIndice >= maximoCombo)
        {
            Debug.Log("Combo finalizado");
            comboIndice = 0;

            if (resetComboCoroutine != null)
            {
                StopCoroutine(resetComboCoroutine);
                resetComboCoroutine = null;
            }
        }
    }
    
    public void AtivarFreezeFrame()
    {
        // Escolhe a hitbox de acordo com o golpe atual
        Transform hitboxAtual = null;
        Vector2 tamanhoAtual = Vector2.zero;

        switch (comboIndice)
        {
            case 1:
                hitboxAtual = hitboxPunch_01;
                tamanhoAtual = tamanhoAtaque_01;
                esperaAtaque = 0.25f;

                SFX.instance.TocarSFX(socoSFX, transform, 1f, 1.2f); //tem que tocar só quando acertar

                break;
            case 2:
                hitboxAtual = hitboxPunch_02;
                tamanhoAtual = tamanhoAtaque_02;
                esperaAtaque = 0.2f;

                SFX.instance.TocarSFX(socoSFX, transform, 1f, 1.0f);

                break;
            case 3:
                hitboxAtual = hitboxPunch_03;
                tamanhoAtual = tamanhoAtaque_03;
                esperaAtaque = 0.25f;

                SFX.instance.TocarSFX(socoSFX, transform, 1f, 0.8f);

                break;
            default:
                return;
        }

        // Faz a checagem de colisão no exato momento do evento
        Collider2D[] acertos = Physics2D.OverlapBoxAll(hitboxAtual.position, tamanhoAtual, 0f);

        foreach (Collider2D hitboxInimigo in acertos)
        {
            if (hitboxInimigo == defesaInimigo)
            {
                StartCoroutine(cameraAutoFit.Shake(shakeDuracao, shakeMagnitude)); // duração e intensidade
                StartCoroutine(FreezeFrame(duracaoFreezeFrame)); // Ativa o freeze frame apenas se o golpe acertar a defesa
                Debug.Log("Freeze Frame ativado");
                return;
            }
        }

        // Se não acertou defesa, não faz nada
        Debug.Log("Sem impacto na defesa — sem freeze.");
    }

    private IEnumerator FreezeFrame(float duracao)
    {
        yield return new WaitForSeconds(esperaAtaque);

        podeAtacar = false;
        rb.linearVelocity = Vector2.zero;           // Para a física momentaneamente
        oAnimator.enabled = false;                  // Congela a animação

        yield return new WaitForSeconds(duracao);   // Espera o tempo do "freeze frame"

        oAnimator.enabled = true;                   // Reativa movimentação e animação
        podeAtacar = true;
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
        if (hitboxPunch_01 != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(hitboxPunch_01.position, tamanhoAtaque_01);
        }

        if (hitboxPunch_02 != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(hitboxPunch_02.position, tamanhoAtaque_02);
        }

        if (hitboxPunch_03 != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(hitboxPunch_03.position, tamanhoAtaque_03);
        }
    }
    #endregion
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Limitador"))
        {
            SFX.instance.TocarSFX(sairDaArenaSFX, transform, 0.5f, 1f);

            if (this.name == "Jogador1")
            {
                Debug.Log($"{gameObject.name} entrou no limitador!");

                rb.linearVelocity = Vector2.zero;
                transform.position = new Vector3(0f, 0f, 20f);
            }
            else
            {
                Debug.Log($"{gameObject.name} entrou no limitador!");
                rb.linearVelocity = Vector2.zero;
                transform.position = new Vector3(0f, 0f, 20f);
            }

        }
    }
}