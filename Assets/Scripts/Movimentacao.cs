using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

// Caso o objeto não tenha o componente, ele é criado em tempo de compilação
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(BoxCollider2D))]
public class Movimentacao : MonoBehaviour
{
    // Variáveis dedicadas à mecânica de movimentação
    private Rigidbody2D rb;
    private Vector2 direcao;
    [SerializeField] private float velocidade = 3f;
    [SerializeField] private float forcaPulo = 3f;

    // Variáveis dedicadas à mecânica de dash
    private bool puloDuploHabilitado = false;
    private bool dashDisponivel = true;
    private bool emDash = false;
    [SerializeField] private float forcaDash = 15f;

    // Variáveis de controle de plataforma/chão/layers
    private bool estaNoChao = false;
    private bool estaNaPlataforma = false;
    Transform peDoPersonagem;
    private LayerMask Ground;
    private LayerMask Platform;
    private LayerMask PlayerMask;
    RaycastHit2D hitPlataforma;
    [SerializeField] private float raioVerificaChao = 0.30f;
    [SerializeField] private float distanciaVerificaPlataforma = 0.20f;

    // Variável de controle para queda através da plataforma
    private bool descendoDaPlataforma = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Referência ao "pé" do personagem para checar contato com o chão/plataforma
        peDoPersonagem = transform.Find("pePlayer");

        // Máscaras de layer usadas nas verificações de colisão
        Ground = LayerMask.GetMask("Ground");
        Platform = LayerMask.GetMask("Platform");
        PlayerMask = LayerMask.GetMask("Player");
    }

    void Update()
    {
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

        // Controle de colisão com as plataformas:
        // - Se não está sobre a plataforma, ignora colisão (permite atravessar de baixo para cima)
        // - Se está sobre a plataforma e não está descendo, reativa a colisão
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
        // Se não estiver em dash, aplica a movimentação horizontal usando o valor armazenado em 'direcao' (setado em OnMove)
        if (!emDash)
        {
            rb.linearVelocity = new Vector2(direcao.x * velocidade, rb.linearVelocity.y);
        }
    }

    void OnMove(InputValue valor)
    {
        // Lê o valor vindo do Input System e armazena a direção desejada
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

        // Desliga a gravidade durante o dash para um impulso mais “reto”
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

        // Duração do dash
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
        // Se está sobre a plataforma, permite “cair” por ela ignorando a colisão por um curto período
        if (estaNaPlataforma == true)
        {
            descendoDaPlataforma = true;
            Physics2D.IgnoreLayerCollision(7, 8, true);
            yield return new WaitForSeconds(0.5f);
            descendoDaPlataforma = false;
        }
    }
}
