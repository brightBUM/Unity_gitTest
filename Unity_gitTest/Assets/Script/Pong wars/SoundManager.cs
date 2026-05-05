using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Range(0f, 1f)]
    public float wallVolume = 0.4f;
    [Range(0f, 1f)]
    public float blockVolume = 0.25f;

    private AudioSource[] pool;
    private int poolSize = 10;
    private int poolIndex = 0;

    void Awake()
    {
        instance = this;

        // Create a pool of AudioSources on this GameObject
        pool = new AudioSource[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            pool[i] = gameObject.AddComponent<AudioSource>();
            pool[i].playOnAwake = false;
        }
    }

    public void PlayWall(AudioClip clip)
    {
        Play(clip, wallVolume);
    }

    public void PlayBlock(AudioClip clip)
    {
        Play(clip, blockVolume);
    }

    private void Play(AudioClip clip, float volume)
    {
        if (clip == null) return;

        // Round robin through the pool
        AudioSource source = pool[poolIndex];
        poolIndex = (poolIndex + 1) % poolSize;

        source.clip = clip;
        source.volume = volume;
        source.Play();
    }
}