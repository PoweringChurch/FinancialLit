using UnityEngine;
using UnityEngine.UI;

public class BackgroundMusicPlayer : MonoBehaviour
{
    public AudioClip[] musicTracks;
    public AudioSource audioSource;
    private float volume = 0.15f;
    public Slider musicMultSlider;
    
    private int currentTrackIndex = 0;
    
    void Start()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        audioSource.loop = false;
        audioSource.volume = volume;
        
        if (musicTracks.Length > 0)
        {
            PlayTrack(0);
        }
    }
    
    void Update()
    {
        // Update volume in real-time if changed in inspector
        audioSource.volume = volume*musicMultSlider.value;
        // Check if current track finished, play next
        if (!audioSource.isPlaying && musicTracks.Length > 0)
        {
            NextTrack();
        }
    }
    
    private void PlayTrack(int index)
    {
        if (index < 0 || index >= musicTracks.Length) return;
        
        currentTrackIndex = index;
        audioSource.clip = musicTracks[currentTrackIndex];
        audioSource.Play();
    }
    
    private void NextTrack()
    {
        currentTrackIndex = (currentTrackIndex + 1) % musicTracks.Length;
        PlayTrack(currentTrackIndex);
    }
}