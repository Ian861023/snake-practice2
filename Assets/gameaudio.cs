using UnityEngine;

public class gameaudio : MonoBehaviour
{
    public AudioSource AudioPlayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void Replay()
    {
        AudioPlayer.Play();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
