using UnityEngine;

public sealed class MusicService
{
    private readonly AudioSource _audioSource;
    private const float BaseVolume = 0.6f;

    public MusicService(AudioClip musicClip)
    {
        var go = new GameObject("[MusicService]");
        Object.DontDestroyOnLoad(go);

        _audioSource = go.AddComponent<AudioSource>();
        _audioSource.clip = musicClip;
        _audioSource.loop = true;
        _audioSource.playOnAwake = false;
        _audioSource.volume = 0f;
    }

    public void Play()
    {
        if (!_audioSource.isPlaying)
            _audioSource.Play();

        ApplySettings();
    }

    public void ApplySettings()
    {
        _audioSource.volume = GameSettings.MusicEnabled
            ? BaseVolume
            : 0f;
    }

    public void SetVolume(float normalizedVolume)
    {
        _audioSource.volume = Mathf.Clamp01(normalizedVolume) * BaseVolume;
    }
}
