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
        treinamento.onClick.AddListener(() => modoDeJogo(true, "Treinamento"));
        multiPlayer.onClick.AddListener(() => modoDeJogo(false, "CharacterSelect"));

    }

    void modoDeJogo (bool modoDeJogo, string scene) 
    { 
        GameManager.instance.singleMode = modoDeJogo;
        SceneManager.LoadScene(scene);
    }
    void Update()
    {
     
    }
}
