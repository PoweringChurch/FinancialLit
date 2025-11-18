using UnityEngine;
using UnityEngine.UI;

public class SFXPlayer : MonoBehaviour
{
    public static SFXPlayer Instance;
    public AudioSource audioSource;
    
    private float volume = 0.15f;
    //public Slider sfxVolSlider;
    
    void Awake()
    {
        Instance = this;
        
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }
    
    void Update()
    {
        // Update volume in real-time if changed in inspector
        audioSource.volume = volume;//*sfxVolSlider.value;
    }
    
    public void Play(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }
    
    public void Play(AudioClip clip, float volumeMultiplier)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip, volume * volumeMultiplier);
        }
    }
}