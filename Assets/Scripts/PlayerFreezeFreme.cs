using UnityEngine;

public class PlayerFreezeFreme : MonoBehaviour
{
    public Movimentacao playerScript;

    public void AtivarFreezeFrame()
    {
        if (playerScript != null)
        {
            playerScript.AtivarFreezeFrame();
        }
    }
    
}