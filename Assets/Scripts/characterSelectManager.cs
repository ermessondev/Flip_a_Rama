using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;


public class characterSelectManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textoBloqueado;
    [SerializeField] private TextMeshProUGUI textoJogador1Confirmado;
    [SerializeField] private TextMeshProUGUI textoJogador2Confirmado;


    // Todos os Debug.Log são de teste por enquanto 
    [SerializeField] public List<Button> listaDePersonagens = new List<Button>();

    [SerializeField] private Canvas canvas;
    private PlayerInputManager pim;

    [SerializeField] private GameObject seletorP1;
    [SerializeField] private GameObject seletorP2;

    void Awake()
    {
        pim = GetComponent<PlayerInputManager>();
    }

    void OnEnable()
    {
        pim.onPlayerJoined += OnPlayerJoined;
    }

    void OnDisable()
    {
        pim.onPlayerJoined -= OnPlayerJoined;
    }
    public void Bloqueado(string jogador)
    {
        StartCoroutine(CharBloqueado(jogador));
    }
    private IEnumerator CharBloqueado(string jogador)
    {
        Debug.Log("Personagem Bloqueado");
        textoBloqueado.text = $"{jogador} este personagem está bloqueado, Escolha outro";
        textoBloqueado.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        textoBloqueado.gameObject.SetActive(false);
    }

    public void ConfirmaPersonagem(string jogador, bool valor)
    {
        StartCoroutine(confirmaPersonagemCoroutine(jogador, valor));
    }

    private IEnumerator confirmaPersonagemCoroutine(string jogador, bool valor)
    {
        if (jogador == "Jogador1" && !valor)
        {
            Debug.Log("Personagem Bloqueado");
            textoJogador1Confirmado.text = $"{jogador} Deseja confirmar o personagem? F para sim ESC para nao";
            textoJogador1Confirmado.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
        }else if (jogador == "Jogador1" && valor)
        {
            textoJogador1Confirmado.gameObject.SetActive(false);
        }

        if (jogador == "Jogador2" && !valor)
        {
            Debug.Log("Personagem Bloqueado");
            textoJogador2Confirmado.text = $"{jogador} Deseja confirmar o personagem? P para sim BACKSPACE para nao";
            textoJogador2Confirmado.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
        }
        else if (jogador == "Jogador2" && valor)
        {
            textoJogador2Confirmado.gameObject.SetActive(false);
        }
    }


    private void Start()
    {
        // Instancia o seletor de personagens através do player input manager.
        if (GameManager.instance.singleMode)
        {
            var player1 = PlayerInput.Instantiate(seletorP1, playerIndex: 0, controlScheme: "Keyboard&Mouse", pairWithDevice: Keyboard.current);
        }
        else
        {
            // Player 1 → teclado
            var player1 = PlayerInput.Instantiate(seletorP1, playerIndex: 0, controlScheme: "Keyboard&Mouse", pairWithDevice: Keyboard.current);
            
            StartCoroutine(SetActionMapNextFrame(player1));


            var player2 = PlayerInput.Instantiate(seletorP2, playerIndex: 1, controlScheme: "Virtual");
            player2.user.UnpairDevices();
            player2.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current, Mouse.current);
            StartCoroutine(SetActionMapNextFrame(player2));
        }


    }


    private void OnPlayerJoined(PlayerInput player)
    {
        player.transform.SetParent(canvas.transform, false);
        Debug.Log($"✅ Player {player.playerIndex} entrou com scheme: {player.currentControlScheme}, device: {player.devices[0].displayName}");
    }


    private IEnumerator SetActionMapNextFrame(PlayerInput player)
    {
        yield return null; // espera 1 frame
        if (player.playerIndex == 0)
        {
            player.SwitchCurrentActionMap("CharacterSelection");
            Debug.Log($"✅ ActionMap trocado com sucesso para {player.currentActionMap.name}");
        }
        else if (player.playerIndex == 1)
        {
            player.SwitchCurrentActionMap("CharacterSelectionPlayer2");
            Debug.Log($"✅ ActionMap trocado com sucesso para {player.currentActionMap.name}, e ControlScheme para {player.currentControlScheme}");

        }

    }
}
