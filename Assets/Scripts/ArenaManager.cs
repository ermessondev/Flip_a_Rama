using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArenaManager : MonoBehaviour
{
    // Todos os Debug.Log são de teste por enquanto 
    [SerializeField] private Transform jogador1Referencia;
    [SerializeField] private Transform jogador2Referencia;
    [SerializeField] private CameraAutoFit cameraAutoFit;

    void Awake()
    {
        if (GameManager.instance == null)
        {
            Debug.LogError("❌ GameManager não encontrado!");
            return;
        }

        // Instancia o Player 1
        var player1Input = PlayerInput.Instantiate(
            GameManager.instance.jogador1,
            controlScheme: "Keyboard&Mouse",
            pairWithDevice: Keyboard.current,
            playerIndex: 0
        );

        player1Input.name = "Jogador1";
        GameManager.instance.jogador1 = player1Input.gameObject;

        if (jogador1Referencia != null)
        {
            player1Input.transform.SetParent(jogador1Referencia, false);
            player1Input.transform.localPosition = Vector3.zero;
            player1Input.transform.localRotation = Quaternion.identity;
        }

        Debug.Log($"✅ Player 1 instanciado no {jogador1Referencia?.name ?? "sem referência"}");

        GameObject player2 = null;

        // MODO MULTIPLAYER: instanciar Player 2 com PlayerInput
        if (!GameManager.instance.singleMode)
        {
            if (Gamepad.all.Count > 0)
            {
                var player2Input = PlayerInput.Instantiate(
                    GameManager.instance.jogador2,
                    controlScheme: "Gamepad",
                    pairWithDevice: Gamepad.all[0],
                    playerIndex: 1
                );

                player2Input.name = "Jogador2";
                player2 = player2Input.gameObject;
                GameManager.instance.jogador2 = player2;

                if (jogador2Referencia != null)
                {
                    player2.transform.SetParent(jogador2Referencia, false);
                    player2.transform.localPosition = Vector3.zero;
                    player2.transform.localRotation = Quaternion.identity;
                }

                Debug.Log($"✅ Player 2 instanciado no {jogador2Referencia?.name ?? "sem referência"}");
            }
            else
            {
                Debug.LogWarning("⚠️ Modo Multiplayer ativo, mas nenhum Gamepad foi detectado!");
            }
        }
        else
        {
            // MODO SINGLEPLAYER: instanciar inimigo sem PlayerInput
            player2 = Instantiate(GameManager.instance.jogador2);
            player2.name = "Jogador2 (IA)";
            GameManager.instance.jogador2 = player2;

            // Remove o PlayerInput caso o prefab tenha, só por garantia
            var input = player2.GetComponent<PlayerInput>();
            if (input != null)
            {
                Destroy(input);
            }

            if (jogador2Referencia != null)
            {
                player2.transform.SetParent(jogador2Referencia, false);
                player2.transform.localPosition = Vector3.zero;
                player2.transform.localRotation = Quaternion.identity;
            }

            Debug.Log($"🤖 Inimigo instanciado como Jogador2 n modo singleplayer");
        }

        StartCoroutine(setarCamera(player1Input.transform, player2?.transform));
    }

    private IEnumerator setarCamera(Transform p1, Transform p2)
    {
        yield return new WaitForEndOfFrame();
        cameraAutoFit.SetPlayers(p1, p2);
    }
}
