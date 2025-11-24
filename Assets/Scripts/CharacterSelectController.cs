using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;

public class CharacterSelectController : MonoBehaviour
{
    [SerializeField] private int colunas = 1; // número de colunas da grade

    private int personagemAtual;
    private Image img;
    private PlayerInput playerInput;
    private List<Button> listaPersonagens;
    private Vector2 direcao;

    private bool selecionado = false;
    private bool confirmado = false;

    private characterSelectManager manager;

    private string jogador;
    void Awake()
    {
        img = GetComponent<Image>();
        playerInput = GetComponent<PlayerInput>();

        playerInput.actions = Instantiate(playerInput.actions);
    }

    void Start()
    {
        // Pega a lista do manager 
        manager = FindFirstObjectByType<characterSelectManager>();
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

        jogador = this.name == "PlayerSelector1(Clone)" ? "Jogador1" : "Jogador2";
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

    void OnCancel()
    {
        if (!selecionado && !confirmado)
        {
            SceneManager.LoadScene("MainMenu");
        }

        if (selecionado && confirmado) 
        {
            return;
        }
        selecionado = false;
        confirmado = false;
        manager.ConfirmaPersonagem(jogador, true);

    }

    void OnConfirm()
    {
        if (personagemAtual == 2 || personagemAtual == 3)
        {
            manager.Bloqueado(jogador);
            return; 
        }

        if (!confirmado && !selecionado)
        {
            selecionado = !selecionado;
            manager.ConfirmaPersonagem(jogador, false);
            return;
        }

        if (selecionado && !confirmado) 
        {
            confirmado = true;
            manager.ConfirmaPersonagem(jogador, true);
        }

        if (playerInput.playerIndex == 0 && confirmado)
        {
            GameManager.instance.setarJogadores(personagemAtual, 0);
            print($"Jogador {playerInput.playerIndex} selecionou {personagemAtual}");
            manager.listaDePersonagens[personagemAtual].image.color = new Color(0f, 0f, 0f, 0.6f);
        }
        else if (playerInput.playerIndex == 1 && confirmado)
        {
            GameManager.instance.setarJogadores(personagemAtual, 1);
            print($"Jogador {playerInput.playerIndex} selecionou {personagemAtual}");
            manager.listaDePersonagens[personagemAtual].image.color = new Color(0f, 0f, 0f, 0.6f);
        }

        // Tenta carregar a arena só quando os jogadores estiverem prontos
        GameManager.instance.TentarIniciarPartida();
    }
}
