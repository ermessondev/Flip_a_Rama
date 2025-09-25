using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class characterSelectManager : MonoBehaviour
{
    // Todos os Debug.Log são de teste por enquanto 
    [SerializeField] public List<Button> listaDePersonagens = new List<Button>();

    [SerializeField] private Canvas canvas;
    private PlayerInputManager pim;

    void Awake()
    {
        pim = GetComponent<PlayerInputManager>();
    }

    void OnEnable()
    {
        pim.onPlayerJoined += OnPlayerJoined;
    }

    void OnDisable()
    {
        pim.onPlayerJoined -= OnPlayerJoined;
    }

    private void Start()
    {
        // Instancia o seletor de personagens através do player input manager.
        if (GameManager.instance.singleMode)
        {
            pim.JoinPlayer(controlScheme: "Keyboard&Mouse", pairWithDevice: Keyboard.current);
        }
        else
        {
            // Player 1 → teclado
            pim.JoinPlayer(controlScheme: "Keyboard&Mouse", pairWithDevice: Keyboard.current);

            // Player 2 → gamepad
            if (Gamepad.all.Count > 0)
                pim.JoinPlayer(controlScheme: "Gamepad", pairWithDevice: Gamepad.all[0]);
            else
                Debug.LogWarning("⚠️ Nenhum Gamepad conectado!");
        }
    }


    private void OnPlayerJoined(PlayerInput player)
    {
        player.transform.SetParent(canvas.transform, false);
        Debug.Log($"✅ Player {player.playerIndex} entrou com scheme: {player.currentControlScheme}, device: {player.devices[0].displayName}");
    }



}
