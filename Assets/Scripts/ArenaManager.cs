using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class ArenaManager : MonoBehaviour
{
    // Todos os Debug.Log são de teste por enquanto 
    [SerializeField] private Transform jogador1Referencia;
    [SerializeField] private Transform jogador2Referencia;
    [SerializeField] private CameraAutoFit cameraAutoFit;


    [Header("Barra de Vida")]
    [SerializeField] private GameObject lifeBar;
    [SerializeField] private Image barraVidaP1;
    [SerializeField] private Image barraVidaP2;
    [SerializeField] private List<GameObject> koP1 = new List<GameObject>();
    [SerializeField] private List<GameObject> koP2 = new List<GameObject>();
    private int vidaJogador1 = 3;
    private int vidaJogador2 = 3;

    public bool jogoPausado = false;

    private Transform posP1;
    private Transform posP2;

    void Awake()
    {
        

        barraVidaP1 = lifeBar.transform.Find("P1/barraVida").GetComponent<UnityEngine.UI.Image>();
        barraVidaP2 = lifeBar.transform.Find("P2/barraVida").GetComponent<UnityEngine.UI.Image>();


        if (GameManager.instance == null)
        {
            Debug.LogError("❌ GameManager não encontrado!");
            return;
        }

        // Instancia o Player 1
        var player1Input = PlayerInput.Instantiate(
            GameManager.instance.jogador1,
            controlScheme: "Keyboard&Mouse",
            pairWithDevice: Keyboard.current,
            playerIndex: 0
        );
        player1Input.SwitchCurrentActionMap("Player1");

        player1Input.name = "Jogador1";
        GameManager.instance.jogador1 = player1Input.gameObject;
        posP1 = player1Input.transform;

        if (jogador1Referencia != null)
        {
            player1Input.transform.SetParent(jogador1Referencia, false);
            player1Input.transform.localPosition = Vector3.zero;
            player1Input.transform.localRotation = Quaternion.identity;
        }

        Debug.Log($"✅ Player 1 instanciado no {jogador1Referencia?.name ?? "sem referência"}");

        GameObject player2 = null;

        // MODO MULTIPLAYER: instanciar Player 2 com PlayerInput
        if (!GameManager.instance.singleMode)
        {

            var player2Input = PlayerInput.Instantiate(
                    GameManager.instance.jogador2,
                    controlScheme: "Virtual",
                    playerIndex: 1
                );
            player2Input.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current, Mouse.current);
            player2Input.SwitchCurrentActionMap("Player2");
            

            player2Input.name = "Jogador2";
            player2 = player2Input.gameObject;
            GameManager.instance.jogador2 = player2;
            posP2 = player2.transform;

            if (jogador2Referencia != null)
            {
                player2.transform.SetParent(jogador2Referencia, false);
                player2.transform.localPosition = Vector3.zero;
                player2.transform.localRotation = Quaternion.identity;
            }

            Debug.Log($"✅ Player 2 instanciado no {jogador2Referencia?.name ?? "sem referência"}");

        }
        else
        {
            // MODO SINGLEPLAYER: instanciar inimigo sem PlayerInput
            player2 = Instantiate(GameManager.instance.jogador2);
            player2.name = "Jogador2";
            GameManager.instance.jogador2 = player2;

            // Remove o PlayerInput caso o prefab tenha, só por garantia
            var input = player2.GetComponent<PlayerInput>();
            if (input != null)
            {
                Destroy(input);
            }

            if (jogador2Referencia != null)
            {
                player2.transform.SetParent(jogador2Referencia, false);
                player2.transform.localPosition = Vector3.zero;
                player2.transform.localRotation = Quaternion.identity;
            }

            Debug.Log($"🤖 Inimigo instanciado como Jogador2 n modo singleplayer");
        }

        StartCoroutine(setarCamera(player1Input.transform, player2?.transform));
    }

    private void Start()
    {
        
    }

    private IEnumerator setarCamera(Transform p1, Transform p2)
    {
        yield return new WaitForEndOfFrame();
        cameraAutoFit.SetPlayers(p1, p2);
    }

    public void PausarJogo(bool value)
    {
        jogoPausado = value;
        if (jogoPausado)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    private void ControlePartida(float dano, string jogador)
    {
        if (jogador == "Jogador1")
        {
            barraVidaP1.fillAmount -= dano;
            if (barraVidaP1.fillAmount <= 0 && vidaJogador1 > 0)
            {
                koP1[vidaJogador1].SetActive(false);
                vidaJogador1 -= 1;
                StartCoroutine(EfeitoKO("Jogador1"));
            }
            else if (barraVidaP1.fillAmount <= 0 && vidaJogador1 <= 0)
            {
                FinalGame();
            }
        }
        else 
        {
            barraVidaP2.fillAmount -= dano;
            if (barraVidaP2.fillAmount <= 0 && vidaJogador2 > 0)
            {
                koP2[vidaJogador1].SetActive(false);
                vidaJogador1 -= 1;
                StartCoroutine(EfeitoKO("Jogador2"));
            }else if (barraVidaP2.fillAmount <= 0 && vidaJogador2 <= 0)
            {
                FinalGame();
            }
        }
    }

    void FinalGame()
    {

    }

    private IEnumerator EfeitoKO(string jogador) 
    {
        yield return null;
    }
}
