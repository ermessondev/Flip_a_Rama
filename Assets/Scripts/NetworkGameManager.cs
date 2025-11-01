using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-950)]
public class NetworkGameManager : MonoBehaviour
{
    public static NetworkGameManager Instance { get; private set; }

    [Header("Cenas do fluxo online")]
    [SerializeField] private SceneRef cenaSelecao;   // CharacterSelect
    [SerializeField] private SceneRef cenaArena;     // StageOne

    private NetworkRunner _runner;
    private bool partidaIniciada;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // =============================================================
    // 🔹 Configuração
    // =============================================================
    public void Configurar(NetworkRunner runner)
    {
        _runner = runner;
        partidaIniciada = false;
        Debug.Log("🌐 NetworkGameManager configurado com Runner ativo.");
    }

    // =============================================================
    // 🚀 Fluxo de cenas
    // =============================================================

    /// <summary>
    /// Leva todos os jogadores para a tela de seleção sincronizada.
    /// </summary>
    public void IrParaSelecao()
    {
        if (_runner == null)
        {
            Debug.LogError("❌ NetworkRunner não configurado!");
            return;
        }

        Debug.Log("🔁 Indo para a cena de seleção de personagens (sincronizada).");
        _runner.LoadScene(cenaSelecao);
    }

    /// <summary>
    /// Inicia a partida e leva todos para a arena.
    /// </summary>
    public void IniciarPartida()
    {
        if (_runner == null)
        {
            Debug.LogError("❌ NetworkRunner não configurado!");
            return;
        }

        if (partidaIniciada)
        {
            Debug.Log("⚠️ A partida já foi iniciada.");
            return;
        }

        Debug.Log("🎮 Carregando cena da arena (StageOne)...");
        _runner.LoadScene(cenaArena);

        partidaIniciada = true;
        Debug.Log("✅ Cena sincronizada — partida iniciada!");
    }

    /// <summary>
    /// Encerra e volta ao menu principal.
    /// </summary>
    public void VoltarAoMenu()
    {
        if (_runner != null)
        {
            _runner.Shutdown();
        }

        partidaIniciada = false;
        SceneManager.LoadScene("MainMenu");

        Debug.Log("🏁 Partida encerrada e retornando ao menu.");
    }
}
