using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

    public class CharacterSelectController : MonoBehaviour
{
    [SerializeField] private int colunas = 1; // número de colunas da grade

    private int personagemAtual;
    private Image img;
    private PlayerInput playerInput;
    private List<Button> listaPersonagens;
    private Vector2 direcao;

    private bool selecionado = false;

    private characterSelectManager manager;


    void Awake()
    {
        img = GetComponent<Image>();
        playerInput = GetComponent<PlayerInput>();

        playerInput.actions = Instantiate(playerInput.actions);
    }

    void Start()
    {
        // Pega a lista do manager 
        var manager = FindFirstObjectByType<characterSelectManager>();
        if (manager != null)
        {
            listaPersonagens = manager.listaDePersonagens;
        }

        // Define índice inicial e cor dependendo do player
        if (playerInput != null)
        {
            if (playerInput.playerIndex == 0)
            {
                personagemAtual = 0;
                //img.color = Color.blue;
            }
            else if (playerInput.playerIndex == 1)
            {
                personagemAtual = 3;
                //.color = Color.red;
            }
        }

        // Posiciona no botão inicial
        if (listaPersonagens != null && listaPersonagens.Count > personagemAtual)
        {
            MoverParaBotao(listaPersonagens[personagemAtual]);
        }
    }

    //movimentação exclusiva para HUD
    public void OnNavigate(InputValue valor)
    {
        if (selecionado == false)
        {
            direcao = valor.Get<Vector2>();

            if (listaPersonagens == null || listaPersonagens.Count == 0) return;

            int total = 4;

            if (direcao.x > 0.5f) // direita
                personagemAtual = (personagemAtual + 1 + total) % total;
            else if (direcao.x < -0.5f) // esquerda
                personagemAtual = (personagemAtual - 1 + total) % total;
            else if (direcao.y > 0.5f) // cima
                personagemAtual = (personagemAtual - colunas + total) % total;
            else if (direcao.y < -0.5f) // baixo
                personagemAtual = (personagemAtual + colunas) % total;

            MoverParaBotao(listaPersonagens[personagemAtual]);
        }

    }

    private void MoverParaBotao(Button botao)
    {
        transform.SetParent(botao.transform, false);
        transform.SetAsLastSibling(); // mantém o seletor na frente
    }

    void OnConfirm()
    {
        if (personagemAtual == 2 || personagemAtual == 3)
        {
            StartCoroutine(manager.charBloqueado());
            return; 
        }
        selecionado = !selecionado;

        if (playerInput.playerIndex == 0)
        {
            GameManager.instance.setarJogadores(personagemAtual, 0);
            print($"Jogador {playerInput.playerIndex} selecionou {personagemAtual}");
        }
        else if (playerInput.playerIndex == 1)
        {
            GameManager.instance.setarJogadores(personagemAtual, 1);
            print($"Jogador {playerInput.playerIndex} selecionou {personagemAtual}");
        }

        // Tenta carregar a arena só quando os jogadores estiverem prontos
        GameManager.instance.TentarIniciarPartida();
    }
}
