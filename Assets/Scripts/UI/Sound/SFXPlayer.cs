using UnityEngine;
using UnityEngine.UI;

public class SFXPlayer : MonoBehaviour
{
    public static SFXPlayer Instance;
    public AudioSource audioSource;
    public Slider sfxVolSlider;
    private float volume = 0.2f;
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
        audioSource.volume = volume*sfxVolSlider.value;
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