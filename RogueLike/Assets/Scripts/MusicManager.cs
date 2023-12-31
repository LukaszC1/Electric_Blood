using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Class that manages the music in the game.
/// </summary>
public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioClip musicOnStart;
    AudioSource audioSource;
    AudioClip switchAudio;
    [SerializeField] float timeToSwitch;
    float volume;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        Play(musicOnStart, false);
    }

    /// <summary>
    /// Method which plays the music.
    /// </summary>
    /// <param name="music"></param>
    /// <param name="interrupt"></param>
    public void Play(AudioClip music, bool interrupt)
    {
        if (interrupt == true)
        {
            audioSource.volume = 0.01f;
            audioSource.clip = music;
            audioSource.Play();

        }
        else
        {
            switchAudio = music;
            StartCoroutine(SmoothSwitch());
        }
    }

    IEnumerator SmoothSwitch()
    {
        volume = 0.01f;

        while (volume > 0f)
        {
            volume -= Time.deltaTime / timeToSwitch;

            if (volume < 0f) { volume = 0f; }
            audioSource.volume = volume;
            yield return new WaitForEndOfFrame();
        }

          Play(switchAudio, true);
    }
}
