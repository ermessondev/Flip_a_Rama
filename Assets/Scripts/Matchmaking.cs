using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public enum Mode : int { x2, x4 }

public class Matchmaking : MonoBehaviour, INetworkRunnerCallbacks
{
    public static Matchmaking Instance { get; private set; }

    private NetworkRunner _runner;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (_runner == null)
            _runner = GetComponent<NetworkRunner>();

        // Registra os callbacks do Fusion
        _runner.AddCallbacks(this);
    }

    public NetworkRunner Runner => _runner;

    public void OnClickCallRoom()
    {
        _ = EnterRoom(_runner, Mode.x2, "Vem X1", 2, true);
    }

    public async Task EnterRoom(NetworkRunner runner, Mode gameType, string roomName, int maxPlayers, bool isVisible)
    {
        var customPros = new Dictionary<string, SessionProperty>();
        customPros["GameMode"] = (int)gameType;

        var startGameArgs = new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionProperties = customPros,
            SessionName = roomName,
            PlayerCount = maxPlayers,
            IsOpen = true,
            IsVisible = isVisible,
            EnableClientSessionCreation = true
        };

        var result = await runner.StartGame(startGameArgs);

        if (result.Ok)
        {
            Debug.Log("✅ Sala criada com sucesso!");
            Debug.Log($"Nome: {runner.SessionInfo.Name}");
            Debug.Log($"Players: {runner.SessionInfo.PlayerCount}");

            // 🔹 Configura o NetworkGameManager com o runner ativo
            NetworkGameManager.Instance?.Configurar(_runner);
        }
        else
        {
            Debug.LogError($"❌ Falha ao iniciar: {result.ShutdownReason}");
        }
    }

    // =============================================================
    // 🔹 Callbacks Fusion
    // =============================================================

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"👤 Player conectado: {player.PlayerId}");
        Debug.Log($"📊 Total de players na sala: {runner.SessionInfo.PlayerCount}");

        // Se for o host e já houver 2 jogadores → inicia a partida
        if (runner.IsServer && runner.SessionInfo.PlayerCount >= 2)
        {
            Debug.Log("🚀 Dois jogadores conectados — iniciando partida!");
            NetworkGameManager.Instance?.IniciarPartida();
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"👋 Player saiu: {player.PlayerId}");
    }

    // Outros callbacks obrigatórios (podem ficar vazios por enquanto)
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason reason) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        throw new System.NotImplementedException();
    }
}
