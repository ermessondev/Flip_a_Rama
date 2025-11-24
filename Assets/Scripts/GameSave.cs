using UnityEngine;

public class GameSave
{
    const string VOLUME1 = "VOLUME1";

    public static void SalvaVolumeGeral(float volumeAtual)
    {
        Debug.Log($"Salvando volume atual: {volumeAtual}");
        PlayerPrefs.SetFloat(VOLUME1, volumeAtual);
    }

    public static float CarregarVolumeGeral(float volumePadrao)
    {
        float volumeSalvoGeral = PlayerPrefs.GetFloat(VOLUME1, volumePadrao);
        Debug.Log($"Carregando volume: {volumeSalvoGeral}");
        return volumeSalvoGeral;
    }

    const string VOLUME2 = "VOLUME2";

    public static void SalvaVolumeEfeitos(float volumeAtual)
    {
        Debug.Log($"Salvando volume atual: {volumeAtual}");
        PlayerPrefs.SetFloat(VOLUME2, volumeAtual);
    }

    public static float CarregarVolumeEfeitos(float volumePadrao)
    {
        float volumeSalvoEfeitos = PlayerPrefs.GetFloat(VOLUME2, volumePadrao);
        Debug.Log($"Carregando volume: {volumeSalvoEfeitos}");
        return volumeSalvoEfeitos;
    }

    const string VOLUME3 = "VOLUME3";

    public static void SalvaVolumeMusica(float volumeAtual)
    {
        Debug.Log($"Salvando volume atual: {volumeAtual}");
        PlayerPrefs.SetFloat(VOLUME3, volumeAtual);
    }

    public static float CarregarVolumeMusica(float volumePadrao)
    {
        float volumeSalvoMusica = PlayerPrefs.GetFloat(VOLUME3, volumePadrao);
        Debug.Log($"Carregando volume: {volumeSalvoMusica}");
        return volumeSalvoMusica;
    }

    const string RESOLUCAO = "RESOLUCO";

    public static void SalvaResolucao(int indiceDaResolucao)
    {
        Debug.Log($"Salvando resolução do indice: {indiceDaResolucao}");
        PlayerPrefs.SetInt(RESOLUCAO, indiceDaResolucao);
    }

    public static int CarregarResolucao(int indiceDaResolucaoPadrao)
    {
        int indiceSalvo = PlayerPrefs.GetInt(RESOLUCAO, indiceDaResolucaoPadrao);
        Debug.Log($"Carregando resolução do indice: {indiceSalvo}");
        return indiceSalvo;
    }

    const string TELA_CHEIA = "TELA_CHEIA";

    public static void SalvaTelaCheia(bool estaEmTelaCheia)
    {
        Debug.Log($"Salvando tela cheia: {estaEmTelaCheia}");
        if (estaEmTelaCheia)
        {
            PlayerPrefs.SetInt(TELA_CHEIA, 1);
        }
        else
        {
            PlayerPrefs.SetInt(TELA_CHEIA, 0);
        }

    }
    
    public static bool CarregaTelaCheia(bool comecaEmTelaCheia)
    {
        int telaCheiaSalva = PlayerPrefs.GetInt(TELA_CHEIA, comecaEmTelaCheia ? 1 : 0);
        if(telaCheiaSalva == 1)
        {
            Debug.Log($"Carregando em tela cheia");
            return true;
        }
        else
        {
            Debug.Log($"Carregando em modo janela");
            return false;
        }
    }
}