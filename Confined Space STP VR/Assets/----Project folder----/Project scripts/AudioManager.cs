using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource instructionSource;
    public AudioSource sfxSource;
    public AudioSource bgSource;

    void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ---------------------------
    // 🎙 INSTRUCTION AUDIO
    // ---------------------------
    public void PlayInstruction(AudioClip clip, bool interrupt = true)
    {
        if (clip == null) return;

        if (interrupt)
            instructionSource.Stop();

        instructionSource.clip = clip;
        instructionSource.loop = false;
        instructionSource.Play();
    }

    public void StopInstruction()
    {
        instructionSource.Stop();
    }

    public bool IsInstructionPlaying()
    {
        return instructionSource.isPlaying;
    }

    // ---------------------------
    // 🔊 SFX AUDIO
    // ---------------------------
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;

        sfxSource.PlayOneShot(clip);
    }

    public void StopSFX()
    {
        sfxSource.Stop();
    }

    // ---------------------------
    // 🎵 BACKGROUND AUDIO
    // ---------------------------
    public void PlayBackground(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;

        bgSource.clip = clip;
        bgSource.loop = loop;
        bgSource.Play();
    }

    public void StopBackground()
    {
        bgSource.Stop();
    }

    public void PauseBackground()
    {
        bgSource.Pause();
    }

    public void ResumeBackground()
    {
        bgSource.UnPause();
    }

    // ---------------------------
    // 🔊 VOLUME CONTROLS
    // ---------------------------
    public void SetInstructionVolume(float volume)
    {
        instructionSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }

    public void SetBGVolume(float volume)
    {
        bgSource.volume = volume;
    }

    // ---------------------------
    // 🔇 STOP ALL
    // ---------------------------
    public void StopAllAudio()
    {
        instructionSource.Stop();
        sfxSource.Stop();
        bgSource.Stop();
    }
}
