using System.Collections;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

// Caso o objeto n�o tenha o componente, ele � criado em tempo de compila��o
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(BoxCollider2D))]
public class Movimentacao : MonoBehaviour
{
    // Vari�veis dedicadas � mec�nica de movimenta��o
    protected Rigidbody2D rb;
    protected Vector2 direcao;
    [SerializeField] private float velocidade = 3f;
    [SerializeField] private float forcaPulo = 3f;

    // Vari�veis dedicadas � mec�nica de dash
    private bool puloDuploHabilitado = false;
    private bool dashDisponivel = true;
    protected bool emDash = false;
    [SerializeField] private float forcaDash = 15f;

    // Vari�veis de controle de plataforma/ch�o/layers
    public bool estaNoChao = false;
    protected bool estaNaPlataforma = false;
    Transform peDoPersonagem;
    private LayerMask Ground;
    private LayerMask Platform;
    private LayerMask PlayerMask;
    RaycastHit2D hitPlataforma;
    [SerializeField] private float raioVerificaChao = 0.30f;
    [SerializeField] private float distanciaVerificaPlataforma = 0.20f;

    // Vari�veis de Animação
    [SerializeField] protected Animator oAnimator;

    // Vari�vel de controle para queda atrav�s da plataforma
    private bool descendoDaPlataforma = false;

    void Awake()
    {
        if (oAnimator == null)
        {
            oAnimator = GetComponent<Animator>();
        }
            
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Refer�ncia ao "p�" do personagem para checar contato com o ch�o/plataforma
        peDoPersonagem = transform.Find("pePlayer");

        // M�scaras de layer usadas nas verifica��es de colis�o
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
       
        // Verifica se o personagem est� no ch�o (retorna true se houver um colisor no raio definido)
        estaNoChao = Physics2D.OverlapCircle(peDoPersonagem.position, raioVerificaChao, Ground) != null;

        // Verifica se o personagem est� encostando em uma plataforma logo abaixo do p�
        hitPlataforma = Physics2D.Linecast(
            peDoPersonagem.position,
            (Vector2)peDoPersonagem.position + (Vector2.down * distanciaVerificaPlataforma),
            Platform
        );
        estaNaPlataforma = hitPlataforma.collider != null;

        // Controle do pulo duplo: recarrega ao tocar o ch�o ou uma plataforma
        if (estaNaPlataforma || estaNoChao)
        {
            puloDuploHabilitado = true;
        }

        // Controle de colis�o com as plataformas:
        // - Se n�o est� sobre a plataforma, ignora colis�o (permite atravessar de baixo para cima)
        // - Se est� sobre a plataforma e n�o est� descendo, reativa a colis�o
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
        // Se n�o estiver em dash, aplica a movimenta��o horizontal usando o valor armazenado em 'direcao' (setado em OnMove)
        if (!emDash)
        {
            rb.linearVelocity = new Vector2(direcao.x * velocidade, rb.linearVelocity.y);
        }
    }

    void OnMove(InputValue valor)
    {
        // L� o valor vindo do Input System e armazena a dire��o desejada
        direcao = valor.Get<Vector2>();
    }

    void OnJump()
    {
        // Pula se estiver no ch�o ou sobre uma plataforma; caso contr�rio, usa o pulo duplo se estiver habilitado
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
        // Inicia o dash se estiver dispon�vel
        if (dashDisponivel == true)
        {
            StartCoroutine(usarDash());
        }
    }

    void OnFallOfPlatform()
    {
        // Inicia a descida atravessando a plataforma (temporariamente ignora colis�o)
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

        // Desliga a gravidade durante o dash para um impulso mais �reto�
        rb.gravityScale = 0f;

        // Se o pulo duplo foi gasto, recarrega ao usar o dash (mec�nica intencional)
        if (!puloDuploHabilitado)
        {
            puloDuploHabilitado = true;
        }

        // Aplica o dash para a esquerda/direita conforme a dire��o horizontal
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

        // Tempo de recarga at� o pr�ximo dash
        yield return new WaitForSeconds(2f);
        dashDisponivel = true;
    }

    private IEnumerator descerPlataforma()
    {
        // Se est� sobre a plataforma, permite �cair� por ela ignorando a colis�o por um curto per�odo
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
}