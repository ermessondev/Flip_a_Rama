using UnityEngine;
using UnityEngine.InputSystem;

public class VirtualInputDevices : MonoBehaviour
{
    public static Keyboard VirtualKeyboard { get; private set; }
    public static Gamepad VirtualGamepad { get; private set; }

    void Awake()
    {
        // Evita duplicar
        if (FindAnyObjectByType<VirtualInputDevices>() != this)
        {
            if (VirtualKeyboard == null)
            {
                VirtualKeyboard = InputSystem.AddDevice<Keyboard>("VirtualKeyboard");
                Debug.Log("⌨️ Virtual Keyboard criado com sucesso!");
            }

            if (VirtualGamepad == null)
            {
                VirtualGamepad = InputSystem.AddDevice<Gamepad>("VirtualGamepad");
                Debug.Log("🎮 Virtual Gamepad criado com sucesso!");
            }

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
