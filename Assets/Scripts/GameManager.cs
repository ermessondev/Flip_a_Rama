using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using UnityEngine.Audio;

[DefaultExecutionOrder(-1000)]
public class GameManager : MonoBehaviour
{
    [SerializeField] public List<GameObject> CharacterDatabase = new List<GameObject>();
    [HideInInspector] public GameObject jogador1;
    [HideInInspector] public GameObject jogador2;

    public static GameManager instance;

    // True  = 1 Player
    // False = 2 Players
    [HideInInspector] public bool singleMode;
    [HideInInspector] public bool treinamento;

    [HideInInspector] public int vitoriasP1;
    [HideInInspector] public int vitoriasP2;
    [HideInInspector] public int raudsTotais;

    [SerializeField] public float volumeGeral;
    [SerializeField] public float volumeEfeitos;
    [SerializeField] public float volumeMusica;


    void Awake()
    {
        treinamento = false;
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Fun��o para setar jogador selecionado na Scene "CharacterSelection"
    public void setarJogadores(int character, int jogador)
    {
        if (jogador == 0)
        {
            jogador1 = CharacterDatabase[character];
        }
        else if (jogador == 1)
        {
            jogador2 = CharacterDatabase[character];
        }
    }

    //Fun��o de controle para tentativa de inicativa de partida, controla a troca de stagio single e coop.
    public void TentarIniciarPartida()
    {
        if (singleMode && jogador1 != null && treinamento == false)
        {
            jogador2 = CharacterDatabase[0];
            SceneManager.LoadScene("StageTwo");
        }
        else if (!singleMode && jogador1 != null && jogador2 != null && treinamento == false)
        {
            SceneManager.LoadScene("StageTwo");
        }
        else if (singleMode && jogador1 != null && treinamento == true)
        {
            jogador2 = CharacterDatabase[8];
            SceneManager.LoadScene("StageTwo");
        }
    }

}
