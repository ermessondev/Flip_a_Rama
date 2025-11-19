using System.Collections;
using System.Data.Common;
using TreeEditor;
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
    [SerializeField] private AudioClip puloSFX;
    [SerializeField] private AudioClip bloqueioSFX;
    [SerializeField] private AudioClip socoSFX;
    [SerializeField] private AudioClip errarSocoSFX;
    [SerializeField] public AudioClip dashSFX;
    
    // Variéveis  dedicadas a mecenica de movimentação
    [SerializeField]public bool podeMover = true;
    protected Rigidbody2D rb;
    protected Vector2 direcao;
    [SerializeField] private float velocidade = 3f;
    [SerializeField] private float forcaPulo = 5f;

    // Variéveis dedicadas a mecánica de dash
    private bool puloDuploHabilitado = false;
    public bool dashDisponivel = true;
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
    private Collider2D meuCollider;
    private Collider2D plataformaAtual;

    // Variéveis de Animação
    [SerializeField] protected Animator oAnimator;

    [Header("Configurações do Combo")]
    private float tempoEntreGolpes = 0.9f;       // Tempo máximo entre ataques para manter o combo
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
    private string adversarioNome;
    public bool acertouDammy = false;
    public bool emBlock = false;
    public bool podeBloquear = true;
    private int golpeTomado;
    [SerializeField] private float distanciaArremesso = 10f;
    [SerializeField] private Movimentacao inimigo;
    private bool sendoArremessado = false;

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

    [SerializeField]private ArenaManager arenaManager;
    void Awake()
    {
        meuCollider = GetComponent<Collider2D>();


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
        inimigo = this.name == "Jogador1" ? GameObject.Find("Jogador2")?.GetComponent<Movimentacao>() : GameObject.Find("Jogador1")?.GetComponent<Movimentacao>();
        arenaManager = FindFirstObjectByType<ArenaManager>();

        if (this.name != "Jogador1")
        {
            this.transform.localScale = new Vector3(-1f, 1f, 1f);

            int layerJogador2 = LayerMask.NameToLayer("Player2");
            DefinirLayerEmTudo(this.gameObject, layerJogador2);
        }


        ehJogador1 = gameObject.name.Contains("Jogador1");      // Identificar se este script e do Jogador1

        if (ehJogador1)     // Se for o Jogador1, define o adversário como "Jogador2" e vice-versa
        {
            adversarioNome = "Jogador2";
        }
        else
        {
            adversarioNome = "Jogador1";
        }
    
        // Procura nos filhos ate achar as Hitbox
        GameObject outroJogador = GameObject.Find(adversarioNome);
        Transform spriteAdversario = outroJogador.transform.Find("Sprite");
        Transform hitboxAdversario = spriteAdversario.Find("Hitbox");

        // Passa as Hitbox para suas devidas posicoes 
        hitboxCorpoPlayer = hitboxAdversario.Find("Hitbox_Corpo").GetComponent<BoxCollider2D>();
        hitboxPePlayer = hitboxAdversario.Find("Hitbox_Pe").GetComponent<BoxCollider2D>();
        hitboxCabecaPlayer = hitboxAdversario.Find("Hitbox_Cabeca").GetComponent<BoxCollider2D>();

        Transform defesaHitbox = spriteAdversario.Find("Hitbox_Defesa"); 
        
        if (defesaHitbox != null) 
        { 
            defesaInimigo = defesaHitbox.GetComponent<BoxCollider2D>(); 
        } 
        else 
        { 
            Debug.LogError("Hitbox_Defesa não encontrada"); 
        }
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
        if (this.name == "Jogador1")
        {
            if (!estaNaPlataforma)
            {
                Physics2D.IgnoreLayerCollision(7, 8, true);
            }
            else if (!descendoDaPlataforma)
            {
                Physics2D.IgnoreLayerCollision(7, 8, false);
            }
        }
        else if (this.name == "Jogador2")
        {
            if (!estaNaPlataforma)
            {
                //Debug.Log($"[{gameObject.name}] Plataforma detectada: {estaNaPlataforma}, Layer: {gameObject.layer}");
                Physics2D.IgnoreLayerCollision(7, 10, true);
            }
            else if (!descendoDaPlataforma)
            {
                Debug.Log($"[{gameObject.name}] Plataforma detectada: {estaNaPlataforma}, Layer: {gameObject.layer}");
                Physics2D.IgnoreLayerCollision(7, 10, false);
            }

        }

    }

    private void FixedUpdate()
    {
        // Se na estiver em dash, aplica a movimentação horizontal usando o valor armazenado em 'direcao' (setado em OnMove)
        if (!emDash && !sendoArremessado && podeMover)
        {
            rb.linearVelocity = new Vector2(direcao.x * velocidade, rb.linearVelocity.y);
        }
    }

    void DefinirLayerEmTudo(GameObject objeto, int novaLayer)
    {
        objeto.layer = novaLayer;

        foreach (Transform filho in objeto.transform)
        {
            DefinirLayerEmTudo(filho.gameObject, novaLayer);
        }
    }

    #region Move
    void OnMove(InputValue valor)
    {
        // L� o valor vindo do Input System e armazena a direção desejada
        if (podeMover)
        {
            direcao = valor.Get<Vector2>();
        }
        
    }

    void OnJump()
    {
        // Pula se estiver no chão ou sobre uma plataforma; caso contrário, usa o pulo duplo se estiver habilitado
        if (podeMover)
        {
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
        
    }

    void OnDash()
    {
        // Inicia o dash se estiver disponível
        if (podeMover)
        {
            if (dashDisponivel == true)
            {
                StartCoroutine(usarDash());
            }
        }
    }

    void OnFallOfPlatform()
    {
        // Inicia a descida atravessando a plataforma (temporariamente ignora colisão)
        if (podeMover)
        {
            StartCoroutine(descerPlataforma());
        }
        
    }

    private void pular()
    {
        // Zera a velocidade vertical antes de aplicar o impulso de pulo (para padronizar a altura)
        SFX.instance.TocarSFX(puloSFX, transform, 1f, 1.0f);
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
            if (this.name == "Jogador1")
            {
                descendoDaPlataforma = true;
                Physics2D.IgnoreLayerCollision(7, 8, true);
                yield return new WaitForSeconds(0.5f);
                descendoDaPlataforma = false;
            }
            else
            {
                descendoDaPlataforma = true;
                Physics2D.IgnoreLayerCollision(7, 10, true);
                yield return new WaitForSeconds(0.5f);
                descendoDaPlataforma = false;
            }
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

        if (direcao.x > 0f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (direcao.x == -1f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }

    }

    public void OnBlock()
        {
            if (podeBloquear)
            {
                // Inicia a corrotina quando o jogador clica para bloquear
                StartCoroutine(ExecutarBlock());

            }
            
        }

    private IEnumerator ExecutarBlock()
    {
        podeAtacar = false;  
        // Ativa a animação de bloqueio
        oAnimator.SetBool("Block", true);
        emBlock = true;

        // Espera 5 segundos
        yield return new WaitForSeconds(1f);

        // Volta para o Idle
        oAnimator.SetBool("Block", false);
        emBlock = false;
        StartCoroutine(cotroleAtaqueDefesa("ataque", true));
    }
    
    void OnAttack()
    {   
        if (emDash || emBlock || !podeAtacar)
        {
            Debug.Log("Não pode atacar");
            return;
        }
        
        podeBloquear = false;
        sendoArremessado = false;

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
        StartCoroutine(cotroleAtaqueDefesa("defesa", true));
    }

    private IEnumerator ExecutarGolpe(int golpe)
    {
        podeAtacar = false;
        SFX.instance.TocarSFX(errarSocoSFX, transform, 1f, 1.0f);

        // Inicia animação e configura duração de cada golpe
        switch (golpe)
        {
            case 1:
                oAnimator.SetBool("Punch", true);
                duracaoGolpe = 0.4f;
                frameParaAcerto = 0.3f;
                
                break;
            case 2:
                oAnimator.SetBool("Punch_2", true);
                duracaoGolpe = 0.4f;
                frameParaAcerto = 0.25f;
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
                    Debug.Log($"Inimigo é {defesaInimigo.gameObject.name}");
                    break;
                }
            }
        }

        // Verifica se o ataque acertou o do inimigo/Hitbox dele
        if (acertos != null)
        {
            foreach (Collider2D hitboxCorpo in acertos)
            {

                if (hitboxCorpo == hitboxCorpoPlayer || hitboxCorpo == hitboxCabecaPlayer || hitboxCorpo == hitboxPePlayer)
                {
                    Debug.Log($"O inimigo foi atingido no golpe {golpe}");

                    string hitboxAtingida = "";
                    if (hitboxCorpo == hitboxCorpoPlayer)
                    {
                        hitboxAtingida = hitboxCorpoPlayer.gameObject.name;
                        //arenaManager.ControleDano(0.11f, defesaInimigo.gameObject.name);
                    }else if (hitboxCorpo == hitboxCabecaPlayer)
                    {
                        hitboxAtingida = hitboxCabecaPlayer.gameObject.name;
                        //.ControleDano(0.11f, defesaInimigo.gameObject.name);
                    }
                    else if (hitboxCorpo == hitboxPePlayer)
                    {
                        hitboxAtingida = hitboxPePlayer.gameObject.name;
                        //arenaManager.ControleDano(0.11f, defesaInimigo.gameObject.name);
                    }
                        

                    arenaManager.ControleDano(0.10f, adversarioNome);
                    StartCoroutine(inimigo.limitaAcoes());

                    if (golpe == 3)
                    {
                        bool inimigoDefendeu = defesaInimigo != null && defesaInimigo.enabled && inimigo.emBlock;

                        if (!inimigoDefendeu)
                        {
                            inimigo.Arremesso(transform.localScale);
                            arenaManager.ControleDano(0.05f, adversarioNome);
                        }
                    }

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

    public void Arremesso(Vector3 direcao)
    {
        StartCoroutine(ArremessoCoroutine(direcao));
    }

    private IEnumerator ArremessoCoroutine(Vector3 direcao)
    {
        sendoArremessado = true;
        podeAtacar = false;

        rb.gravityScale = 1f;
        rb.linearVelocity = Vector2.zero;

        // define o angulo do arremesso para 45 graus
        Vector2 dir45 = new Vector2(Mathf.Sign(direcao.x), 1).normalized;

        rb.AddForce(dir45 * distanciaArremesso, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.5f);

        podeAtacar = true;
        dashDisponivel = true;
        podeMover = true;

        yield return new WaitForSeconds(0.9f);
        sendoArremessado = false;
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

                break;
            case 2:
                hitboxAtual = hitboxPunch_02;
                tamanhoAtual = tamanhoAtaque_02;
                esperaAtaque = 0.2f;

                break;
            case 3:
                hitboxAtual = hitboxPunch_03;
                tamanhoAtual = tamanhoAtaque_03;
                esperaAtaque = 0.3f;

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

                SFX.instance.TocarSFX(bloqueioSFX, transform, 1f, 1.0f);

                Debug.Log("Freeze Frame ativado");
                return;
            }
        }

        foreach (Collider2D hitboxCorpo in acertos)
            {

                if (hitboxCorpo == hitboxCorpoPlayer || hitboxCorpo == hitboxCabecaPlayer || hitboxCorpo == hitboxPePlayer)
                {
                    StartCoroutine(cameraAutoFit.Shake(shakeDuracao, shakeMagnitude)); 
                    StartCoroutine(FreezeFrame(duracaoFreezeFrame));

                    SFX.instance.TocarSFX(socoSFX, transform, 1f, 1.0f);
                    arenaManager.ControleDano(0.11f, hitboxCabecaPlayer.gameObject.name);
                    arenaManager.ControleDano(0.11f, hitboxCorpoPlayer.gameObject.name);
                    arenaManager.ControleDano(0.11f, hitboxPePlayer.gameObject.name);
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
            Debug.Log($"{gameObject.name} entrou no limitador!");
            StartCoroutine(arenaManager.EfeitoKO(this.name));

        }
    }

    void OnPauseMenu()
    {
        arenaManager.PausarJogo(!arenaManager.jogoPausado);
    }

    public void falarTeste()
    {
        Debug.Log("Ai AI");
    }

    public IEnumerator limitaAcoes()
    {
        
        podeBloquear = false;
        podeMover = false;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        podeAtacar = false;
        dashDisponivel = false;
        yield return new WaitForSeconds(0.8f);
        podeBloquear = true;
        podeMover = true;
        podeAtacar = true;
        dashDisponivel = true;
    }

    IEnumerator cotroleAtaqueDefesa(string variavel, bool valor)
    {
        yield return new WaitForSeconds(0.6f);
        if (variavel == "defesa")
        {
            podeBloquear = valor;
        }
        else 
        { 
            podeAtacar = valor;
        }
    }

    public void Respaw()
    {
        rb.linearVelocity = Vector2.zero;
        transform.position = new Vector3(0f, 0f, 20f);
    }

    public void FinalPartida(bool vitoria)
    {
        podeMover = false;
        if (vitoria)
        {
            Debug.Log($"{this.name} Ganhei saporra");
        }
        else {
            Debug.Log($"{this.name} Perdi saporra");
        }
    }
}