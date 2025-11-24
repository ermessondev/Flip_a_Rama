using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Audio;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private AudioClip botaoAbrirMenuSFX;
    [SerializeField] private AudioClip botaoFecharMenuSFX;
    [SerializeField] private AudioClip botaoOnSFX;

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
    }
    private void OnDisable()
    {
        jogar.onClick.RemoveAllListeners();
        configuracoes.onClick.RemoveAllListeners();
        creditos.onClick.RemoveAllListeners();
        sair.onClick.RemoveAllListeners();
        treinamento.onClick.RemoveAllListeners();
        arena.onClick.RemoveAllListeners();
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

}
