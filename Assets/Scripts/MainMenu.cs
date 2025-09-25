using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Todos os Debug.Log são de teste por enquanto 
    [SerializeField] Button singlePlayer;
    [SerializeField] Button multiPlayer;
    void Start()
    {
        singlePlayer.onClick.AddListener(() => modoDeJogo(true));
        multiPlayer.onClick.AddListener(() => modoDeJogo(false));
    }

    void modoDeJogo (bool modoDeJogo) 
    { 
        GameManager.instance.singleMode = modoDeJogo;
        SceneManager.LoadScene("CharacterSelect");
    }
    void Update()
    {
     
    }
}
