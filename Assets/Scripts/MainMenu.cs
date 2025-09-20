using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
