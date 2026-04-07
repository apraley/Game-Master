using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip holdMusicLoop;
    [SerializeField] private float volume = 0.35f;

    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        source.loop = true;
        source.playOnAwake = true;
        source.volume = volume;

        if (holdMusicLoop != null)
        {
            source.clip = holdMusicLoop;
        }
    }

    private void Start()
    {
        if (source.clip != null && !source.isPlaying)
        {
            source.Play();
        }
    }
}
