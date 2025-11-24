using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Audio;
using TMPro;

public class PainelDeConfiguracoes : MonoBehaviour
{
    [SerializeField] public Slider volumeGeral;
    [SerializeField] public Slider volumeEfeitos;
    [SerializeField] public Slider volumeMusica;
    [SerializeField] public AudioMixer mixer;

    const string MIXER_MASTER = "Master";
    const string MIXER_MUSICA = "Musica";
    const string MIXER_EFEITOS = "Efeitos";

    [SerializeField] private AudioClip botaoAbrirMenuSFX;
    [SerializeField] private AudioClip botaoFecharMenuSFX;
    [SerializeField] private AudioClip botaoOnSFX;

    [SerializeField] Toggle telaCheia;
    [SerializeField] TMP_Dropdown painelResolucao;
    Resolution[] resolucoesPossiveis;
    List<string> nomesDasResolucoes = new List<string>();

    private void OnEnable()
    {
        telaCheia.onValueChanged.AddListener(QuandoClicarTelaCheia);
        painelResolucao.onValueChanged.AddListener(QuandoSelecionarResolucao);
        volumeGeral.onValueChanged.AddListener(QuandoAletrarVolumeGeral);
        volumeEfeitos.onValueChanged.AddListener(QuandoAlterarVolumeEfeitos);
        volumeMusica.onValueChanged.AddListener(QuandoAlterarVolumeMusica);
    }
    private void OnDisable()
    {
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

        QuandoAletrarVolumeGeral(GameManager.instance.volumeGeral);
    }

    public void TocarSomConfiguracoes()
    {
        SFX.instance.TocarSFX(botaoOnSFX, transform, 1, 1);
    }
    
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

    public void VoltandoParaMenuInicial()
    {
        Debug.Log("De volta para o Início");
        SceneManager.LoadScene("MainMenu");
    }

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
}
