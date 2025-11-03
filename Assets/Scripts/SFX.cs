using UnityEngine;

public class SFX : MonoBehaviour
{
    public static SFX instance;

    [SerializeField] private AudioSource sfxObject;

    private void Awake()
    { 
        if(instance == null)
        {
            instance = this;
        }
    }

    [HideInInspector] public void TocarSFX(AudioClip audioclip, Transform spawnTransform, float volume, float pitch)
    {
        AudioSource audioSource = Instantiate(sfxObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioclip;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.Play();

        float clipLenght = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLenght);
    }
}
