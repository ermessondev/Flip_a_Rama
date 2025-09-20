using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    // True  = 1 Player
    // False = 2 Players
    [HideInInspector] public bool singleMode;
    
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
