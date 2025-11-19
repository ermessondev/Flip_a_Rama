using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class MainMenu : MonoBehaviour
{
    // Todos os Debug.Log são de teste por enquanto 
    [SerializeField] Button jogar;
    [SerializeField] Button configuracoes;
    [SerializeField] Button creditos;
    [SerializeField] Button sair;
    [SerializeField] GameObject painelDeJogar;
    [SerializeField] GameObject painelDeConfiguracoes;
    [SerializeField] GameObject painelDeCreditos;

    //Botões do Painel de Jogar
    [SerializeField] Button arena;
    [SerializeField] Button treinamento;
    //Botões do Painel de Configurações

    //Áudio dos botões do Menu
    [SerializeField] private AudioClip botaoAbrirMenuSFX;
    [SerializeField] private AudioClip botaoFecharMenuSFX;
    [SerializeField] private AudioClip botaoOnSFX;
    [SerializeField] private AudioClip botaoOffSFX;

    void Awake()
    {
        AbrirJogar(false);
        AbrirConfiguracoes(false);
        AbrirCreditos(false);
    }

    private void OnEnable()
    {
    //    jogar.onClick.AddListener
    }
    private void OnDisable()
    {
    //    jogar.onClick.RemoveAllListeners
    }

    void Start()
    {
        jogar.onClick.AddListener(() => AbrirJogar(true));
        configuracoes.onClick.AddListener(() => AbrirConfiguracoes(true));
        creditos.onClick.AddListener(() => AbrirCreditos(true));
        //solo.onClick.AddListener(() => modoDeJogo(true, "CharacterSelect"));
        treinamento.onClick.AddListener(() => modoDeJogo(true, "CharacterSelect", true));
        arena.onClick.AddListener(() => modoDeJogo(false, "CharacterSelect"));
    }

    void modoDeJogo(bool modoDeJogo, string scene, bool treinamento = false)
    {
        GameManager.instance.singleMode = modoDeJogo;
        GameManager.instance.treinamento = treinamento;
        SceneManager.LoadScene(scene);
        Debug.Log($"🎮 Modo selecionado: singleMode={GameManager.instance.singleMode}, treinamento={GameManager.instance.treinamento}");

    }
    public void AbrirJogar(bool abrir)
    {
        if (abrir)
        {
            painelDeJogar.SetActive(true);
        }
        else
        {
            painelDeJogar.SetActive(false);
        }
        Debug.Log("abriu o menu de modos de jogo");
    }
    public void AbrirConfiguracoes(bool abrir)
    {
        if (abrir)
        {
            painelDeConfiguracoes.SetActive(true);
        }
        else
        {
            painelDeConfiguracoes.SetActive(false);
        }
        Debug.Log("abriu o menu de configrações");
    }
    public void AbrirCreditos(bool abrir)
    {
        if (abrir)
        {
            painelDeCreditos.SetActive(true);
        }
        else
        {
            painelDeCreditos.SetActive(false);
        }
        Debug.Log("abriu o menu dos créditos");
    }
    public void SairdoJogo()
    {
        Debug.Log("saiu do jogo");
    }

    void FecharMenuSFX()
    {
        SFX.instance.TocarSFX(botaoOnSFX, transform, 1, 1);

        SFX.instance.TocarSFX(botaoOffSFX, transform, 1, 1);

        SFX.instance.TocarSFX(botaoAbrirMenuSFX, transform, 1, 1);

        SFX.instance.TocarSFX(botaoFecharMenuSFX, transform, 1, 1);
    }

    void Update()
    {

    }
}
