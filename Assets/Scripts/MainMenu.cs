using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Todos os Debug.Log são de teste por enquanto 
    [SerializeField] Button singlePlayer;
    [SerializeField] Button treinamento;
    [SerializeField] Button multiPlayer;

    [SerializeField] Button voltarMenu;
    [SerializeField] GameObject onlineMenu;

    [SerializeField] Button btnCriarSala;

    private void OnEnable()
    {
        singlePlayer.onClick.AddListener(() => modoDeJogo(true, "CharacterSelect"));
        treinamento.onClick.AddListener(() => modoDeJogo(true, "Treinamento"));
        multiPlayer.onClick.AddListener(() => enableDisblePanel(onlineMenu, true));
        voltarMenu.onClick.AddListener(() => enableDisblePanel(onlineMenu, false));
    }

    private void OnDisable()
    {
        singlePlayer.onClick.RemoveAllListeners();
        treinamento.onClick.RemoveAllListeners();
        multiPlayer.onClick.RemoveAllListeners();
        voltarMenu.onClick.RemoveAllListeners();
    }

    void modoDeJogo (bool modoDeJogo, string scene) 
    { 
        GameManager.instance.singleMode = modoDeJogo;
        SceneManager.LoadScene(scene);
    }
    
    void enableDisblePanel(GameObject panel, bool status )
    {
        panel.SetActive(status);
    }
}
