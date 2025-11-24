using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Audio;
using TMPro;

public class MainMenu : MonoBehaviour
{
    // Todos os Debug.Log são de teste por enquanto 
    [SerializeField] Button jogar;
    [SerializeField] Button configuracoes;
    [SerializeField] Button creditos;
    [SerializeField] Button sair;
    [SerializeField] Button voltarAoMenu;
    [SerializeField] GameObject painelDeJogar;
    [SerializeField] GameObject painelDeConfiguracoes;
    [SerializeField] GameObject painelDeCreditos;

    //Botões do Painel de Jogar
    [SerializeField] Button arena;
    [SerializeField] Button treinamento;
    //Botões do Painel de Configurações
    [SerializeField] Toggle telaCheia;
    [SerializeField] TMP_Dropdown painelResolucao;
    Resolution[] resolucoesPossiveis;
    List<string> nomesDasResolucoes = new List<string>();

    // Áudio do Jogo
    [SerializeField] Slider volumeGeral;
    [SerializeField] Slider volumeEfeitos;
    [SerializeField] Slider volumeMusica;
    [SerializeField] AudioMixer mixer;

    const string MIXER_MASTER = "Master";
    const string MIXER_MUSICA = "Musica";
    const string MIXER_EFEITOS = "Efeitos";

    //Áudio dos botões do Menu
    [SerializeField] private AudioClip botaoAbrirMenuSFX;
    [SerializeField] private AudioClip botaoFecharMenuSFX;
    [SerializeField] private AudioClip botaoOnSFX;

    void Awake()
    {

    }

    private void OnEnable()
    {
        jogar.onClick.AddListener(() => AbrirJogar(true));
        configuracoes.onClick.AddListener(() => AbrirConfiguracoes(true));
        creditos.onClick.AddListener(() => AbrirCreditos(true));
        sair.onClick.AddListener(SairDoJogo);
        //solo.onClick.AddListener(() => modoDeJogo(true, "CharacterSelect"));
        treinamento.onClick.AddListener(() => modoDeJogo(true, "CharacterSelect", true));
        arena.onClick.AddListener(() => modoDeJogo(false, "CharacterSelect"));
        telaCheia.onValueChanged.AddListener(QuandoClicarTelaCheia);
        painelResolucao.onValueChanged.AddListener(QuandoSelecionarResolucao);
        volumeGeral.onValueChanged.AddListener(QuandoAletrarVolumeGeral);
        volumeEfeitos.onValueChanged.AddListener(QuandoAlterarVolumeEfeitos);
        volumeMusica.onValueChanged.AddListener(QuandoAlterarVolumeMusica);
    }
    private void OnDisable()
    {
        jogar.onClick.RemoveAllListeners();
        configuracoes.onClick.RemoveAllListeners();
        creditos.onClick.RemoveAllListeners();
        sair.onClick.RemoveAllListeners();
        treinamento.onClick.RemoveAllListeners();
        arena.onClick.RemoveAllListeners();
        telaCheia.onValueChanged.RemoveAllListeners();
        painelResolucao.onValueChanged.RemoveAllListeners();
        volumeGeral.onValueChanged.RemoveAllListeners();
        volumeEfeitos.onValueChanged.RemoveAllListeners();
        volumeMusica.onValueChanged.RemoveAllListeners();
    }

    void Start()
    {
        resolucoesPossiveis = Screen.resolutions;
        Resolution resAtual;
        for (int i = 0; i < resolucoesPossiveis.Length; i++)
        {
            resAtual = resolucoesPossiveis[i];
            nomesDasResolucoes.Add($"{resAtual.width} x {resAtual.height} ({resAtual.refreshRateRatio.value.ToString("0.00")}Hz)");
        }
        painelResolucao.ClearOptions();
        painelResolucao.AddOptions(nomesDasResolucoes);

        QuandoAletrarVolumeGeral(volumeGeral.value);
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
            TocarSomMenuAbrir();
            painelDeJogar.SetActive(true);
        }
        else
        {
            TocarSomMenuFechar();
            painelDeJogar.SetActive(false);
        }
        Debug.Log("abriu o menu de modos de jogo");
    }
    public void AbrirConfiguracoes(bool abrir)
    {
        if (abrir)
        {
            TocarSomMenuAbrir();
            painelDeConfiguracoes.SetActive(true);
        }
        else
        {
            TocarSomMenuFechar();
            painelDeConfiguracoes.SetActive(false);
        }
        Debug.Log("abriu o menu de configrações");
    }
    public void AbrirCreditos(bool abrir)
    {
        if (abrir)
        {
            TocarSomMenuAbrir();
            painelDeCreditos.SetActive(true);
        }
        else
        {
            TocarSomMenuFechar();
            painelDeCreditos.SetActive(false);
        }
        Debug.Log("abriu o menu dos créditos");
    }

    public void SairDoJogo()
    {
        Debug.Log("Saiu do Jogo");
        Application.Quit();
    }
    public void TocarSomMenuAbrir()
    {
        SFX.instance.TocarSFX(botaoAbrirMenuSFX, transform, 1, 1);
    }

    public void TocarSomMenuFechar()
    {
        SFX.instance.TocarSFX(botaoFecharMenuSFX, transform, 1, 1);
    }

    public void TocarSomConfiguracoes()
    {
        SFX.instance.TocarSFX(botaoOnSFX, transform, 1, 1);
    }

    // Coisas do Menu de Configurações
    public void QuandoClicarTelaCheia(bool estaEmTelaCheia)
    {
        TocarSomConfiguracoes();
        Debug.Log($"Valor tela cheia: {estaEmTelaCheia}");
        Screen.fullScreen = estaEmTelaCheia;
        // Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
    }
    public void QuandoSelecionarResolucao(int indiceDaResolucao)
    {
        Screen.SetResolution(
            resolucoesPossiveis[indiceDaResolucao].width,
            resolucoesPossiveis[indiceDaResolucao].height,
            Screen.fullScreenMode,
            resolucoesPossiveis[indiceDaResolucao].refreshRateRatio);
    }

    // Coisas do Menu de Pausa
    public void VoltandoParaMenuInicial()
    {
        Debug.Log("De volta para o Início");
        SceneManager.LoadScene("MainMenu");
    }

    // Coisas do Audio do Jogo
    public void QuandoAletrarVolumeGeral(float volumeAtual)
    {
        Debug.Log($"Slider com valor de: {volumeAtual}");
        mixer.SetFloat(MIXER_MASTER, Mathf.Log10(volumeAtual) * 20);
    }
    public void QuandoAlterarVolumeEfeitos(float volumeAtual)
    {
        Debug.Log($"Slider com valor de: {volumeAtual}");
        mixer.SetFloat(MIXER_EFEITOS, Mathf.Log10(volumeAtual) * 20);
    }
    public void QuandoAlterarVolumeMusica(float volumeAtual)
    {
        Debug.Log($"Slider com valor de: {volumeAtual}");
        mixer.SetFloat(MIXER_MUSICA, Mathf.Log10(volumeAtual) * 20);
    }

    private void Update()
    {
        
    }
}
