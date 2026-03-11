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
        // safety
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        
        audioSource.loop = false;
        audioSource.volume = volume;
        // safety
        if (musicTracks.Length > 0)
            PlayTrack(0);
    }
    
    void Update()
    {
        audioSource.volume = volume*musicMultSlider.value;
        // check if current track finished, play next
        if (!audioSource.isPlaying && musicTracks.Length > 0)
            NextTrack();
    }
    
    private void PlayTrack(int index)
    {
        // ensure were within the bounds of the songs array
        if (index < 0 || index >= musicTracks.Length) return;
        
        currentTrackIndex = index;
        audioSource.clip = musicTracks[currentTrackIndex];
        audioSource.Play();
    }
    
    private void NextTrack()
    {
        // ensure tracks can loop
        currentTrackIndex = (currentTrackIndex + 1) % musicTracks.Length;
        PlayTrack(currentTrackIndex);
    }
}