using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Todos os Debug.Log são de teste por enquanto 
    [SerializeField] Button singlePlayer;
    [SerializeField] Button treinamento;
    [SerializeField] Button multiPlayer;
    
    void Start()
    {
        singlePlayer.onClick.AddListener(() => modoDeJogo(true, "CharacterSelect"));
        treinamento.onClick.AddListener(() => modoDeJogo(true, "CharacterSelect", true));
        multiPlayer.onClick.AddListener(() => modoDeJogo(false, "CharacterSelect"));

    }

    void modoDeJogo(bool modoDeJogo, string scene, bool treinamento = false)
    {
        GameManager.instance.singleMode = modoDeJogo;
        GameManager.instance.treinamento = treinamento;
        SceneManager.LoadScene(scene);
        Debug.Log($"🎮 Modo selecionado: singleMode={GameManager.instance.singleMode}, treinamento={GameManager.instance.treinamento}");

    }
    void Update()
    {
     
    }
}
