using System.Collections;
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
            var player1 = pim.JoinPlayer(playerIndex: 0, controlScheme: "Keyboard&Mouse", pairWithDevice: Keyboard.current);
        }
        else
        {
            // Player 1 → teclado
            var player1 = pim.JoinPlayer(playerIndex: 0, controlScheme: "Keyboard&Mouse", pairWithDevice: Keyboard.current);
            StartCoroutine(SetActionMapNextFrame(player1));


            var player2 = pim.JoinPlayer(playerIndex: 1, controlScheme: "Virtual");
            player2.user.UnpairDevices();
            player2.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current, Mouse.current);
            StartCoroutine(SetActionMapNextFrame(player2));
            



        }


    }


    private void OnPlayerJoined(PlayerInput player)
    {
        player.transform.SetParent(canvas.transform, false);
        Debug.Log($"✅ Player {player.playerIndex} entrou com scheme: {player.currentControlScheme}, device: {player.devices[0].displayName}");
    }


    private IEnumerator SetActionMapNextFrame(PlayerInput player)
    {
        yield return null; // espera 1 frame
        if (player.playerIndex == 0)
        {
            player.SwitchCurrentActionMap("CharacterSelection");
            Debug.Log($"✅ ActionMap trocado com sucesso para {player.currentActionMap.name}");
        }
        else if (player.playerIndex == 1)
        {
            player.SwitchCurrentActionMap("CharacterSelectionPlayer2");
            Debug.Log($"✅ ActionMap trocado com sucesso para {player.currentActionMap.name}, e ControlScheme para {player.currentControlScheme}");
            
        } 
        
    }
}
