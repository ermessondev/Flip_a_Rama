using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Revanche : MonoBehaviour
{
    [SerializeField] private Button btnSim;
    [SerializeField] private Button btnNao;
    [SerializeField] private GameObject canvasFinal;
    private void OnEnable()
    {
        btnSim.onClick.AddListener(() => RevancheDecisao(false, "CharacterSelect"));
        btnNao.onClick.AddListener(() => RevancheDecisao(true, "MainMenu"));
    }

    private void OnDisable()
    {
        btnSim.onClick.RemoveAllListeners();
        btnNao.onClick.RemoveAllListeners();
    }

    public void RevancheDecisao(bool modoDeJogo, string scene)
    {
        Debug.Log("Clicou");
        GameManager.instance.singleMode = modoDeJogo;
        SceneManager.LoadScene(scene);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
