using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ArenaManager : MonoBehaviour
{
    // Todos os Debug.Log são de teste por enquanto 
    [SerializeField] private Transform jogador1Referencia;
    [SerializeField] private Transform jogador2Referencia;
    [SerializeField] private CameraAutoFit cameraAutoFit;

    [SerializeField] private Temporizador temporizador;


    [Header("Barra de Vida")]
    [SerializeField] private GameObject lifeBar;
    [SerializeField] private Image barraVidaP1;
    [SerializeField] private Image barraVidaP2;
    [SerializeField] private Image barraDanoP1;
    [SerializeField] private Image barraDanoP2;
    [SerializeField] private List<GameObject> koP1 = new List<GameObject>();
    [SerializeField] private List<GameObject> koP2 = new List<GameObject>();
    [SerializeField] private AudioClip koSFX;
    private int vidaJogador1 = 3;
    private int vidaJogador2 = 3;

    public bool jogoPausado = false;

    private Transform posP1;
    private Transform posP2;

    private Movimentacao player1;
    private Movimentacao player2;

    string winer;
    bool jaTeveEmpate = false;

    [SerializeField] private GameObject canvasFinal;

    [SerializeField] private GameObject MenuPausa;

    [SerializeField] TextMeshProUGUI textWiner;
    [SerializeField] TextMeshProUGUI roundStart;
    public bool partidaIniciada = false;
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
        player1 = GameObject.Find("Jogador1")?.GetComponent<Movimentacao>();
        player2 = GameObject.Find("Jogador2")?.GetComponent<Movimentacao>();
        StartCoroutine(RoundStart());
    }

    private IEnumerator setarCamera(Transform p1, Transform p2)
    {
        yield return new WaitForEndOfFrame();
        cameraAutoFit.SetPlayers(p1, p2);
    }

    void OnPauseMenu()
    {
        PausarJogo(!jogoPausado);
    }

    public void PausarJogo(bool value)
    {

        jogoPausado = value;
        if (jogoPausado)
        {

            Time.timeScale = 0f;
            MenuPausa.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            MenuPausa.SetActive(false);
        }
    }

    public void ControleDano(float dano, string jogador)
    {
        if (jogador == "Jogador1" && !player1.emBlock)
        {
            barraVidaP1.fillAmount -= dano;
            controleBarraSegundaria(barraDanoP1, dano);
            StartCoroutine(controleBarraSegundaria(barraDanoP1, dano));
            if (barraVidaP1.fillAmount <= 0)
            {
                StartCoroutine(EfeitoKO("Jogador1"));
            }
        }
        else if(jogador == "Jogador2" && !player2.emBlock)
        {
            barraVidaP2.fillAmount -= dano;
            StartCoroutine(controleBarraSegundaria(barraDanoP2, dano));

            if (barraVidaP2.fillAmount <= 0)
            {
                StartCoroutine(EfeitoKO("Jogador2"));
            }
        }
    }

    IEnumerator controleBarraSegundaria(Image barraVida, float dano)
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < 10; i ++) 
        {
            barraVida.fillAmount -= 0.01f;
            yield return new WaitForSeconds (0.05f);
        }
    }

    public void VerificaVida()
    {

    }

    public IEnumerator FinalGame()
    {
        yield return null;
        if (vidaJogador1 > vidaJogador2 || (vidaJogador1 == vidaJogador2 && barraVidaP1.fillAmount > barraVidaP2.fillAmount))
        {
            winer = "Jogador1";
        }
        else if (vidaJogador1 < vidaJogador2 || (vidaJogador1 == vidaJogador2 && barraVidaP1.fillAmount < barraVidaP2.fillAmount))
        {
            winer = "Jogador2";
        }
        else 
        {
            winer = "Empate";
        }

        if (winer == "Jogador1")
        {
            player1.FinalPartida(true);
            GameManager.instance.vitoriasP1 += 1;
            player2.FinalPartida(false);
            textWiner.text = $"{player1.name} é o Vencendor";
            textWiner.gameObject.SetActive(true);
            yield return new WaitForSeconds(3);
            canvasFinal.gameObject.SetActive(true);

        }
        else if (winer == "Jogador2")
        {
            player2.FinalPartida(true);
            GameManager.instance.vitoriasP2 += 1;
            player1.FinalPartida(false);
            textWiner.text = $"{player2.name} é o Vencendor";
            textWiner.gameObject.SetActive(true);
            yield return new WaitForSeconds(3);
            canvasFinal.gameObject.SetActive(true);

        }
        else if (winer == "Empate" && !jaTeveEmpate)
        {
            
        }
    }


    public IEnumerator EfeitoKO(string jogador) 
    {

        
        if (jogador == "Jogador1")
        {
            SFX.instance.TocarSFX(koSFX, transform, 1f, 1.0f);
            if (vidaJogador1 > 1)
            {
                barraVidaP1.fillAmount += 1;
                barraDanoP1.fillAmount += 1; 
            }
            player1.Respaw();
            koP1[vidaJogador1 - 1].SetActive(false);
            vidaJogador1 -= 1;
            if (vidaJogador1 == 0)
            {
                StartCoroutine(FinalGame());
            }
            yield return null;
        }
        else if(jogador == "Jogador2")
        {
            SFX.instance.TocarSFX(koSFX, transform, 1f, 1.0f);
            if (vidaJogador2 > 1)
            {
                barraVidaP2.fillAmount += 1;
                barraDanoP2.fillAmount += 1;

            }
            player2.Respaw();
            koP2[vidaJogador2 - 1].SetActive(false);
            vidaJogador2 -= 1;
            if (vidaJogador2 == 0)
            {
                StartCoroutine(FinalGame());
            }
            yield return null;
        }
        
        
    }

    private IEnumerator RoundStart() 
    {
        player1.podeMover = false; player2.podeMover = false;
        player1.podeBloquear = false; player2.podeBloquear=false;
        player1.dashDisponivel = false; player2.dashDisponivel = false;
        for (int i = 3; i >= 1; i--) 
        {
            roundStart.text = $"{i}";
            roundStart.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            roundStart.gameObject.SetActive(false);
            if (i == 1)
            {
                roundStart.text = $"Comecem a Luta";
                roundStart.gameObject.SetActive(true);
                yield return new WaitForSeconds(1f);
                roundStart.gameObject.SetActive(false);
            }

        }
        player1.podeMover = true; player2.podeMover = true;
        player1.podeBloquear = true; player2.podeBloquear = true;
        player1.dashDisponivel = true; player2.dashDisponivel = true;
        partidaIniciada = true;
        temporizador.IniciarRelogio();

    }
}
