using UnityEngine;

public class SFX : MonoBehaviour
{
    [SerializeField] AudioSource caixaDeSom;


    public void PlaySound(AudioClip som, float pitchMinimo, float pitchMaximo)
    {
        Debug.Log($"Ta tocando esse som aqui: {som.name}");
        caixaDeSom.pitch = Random.Range(pitchMinimo, pitchMaximo);
        caixaDeSom.PlayOneShot(som);
    }
}
