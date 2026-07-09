using UnityEngine;

public class SoundManager : MonoBehaviour
{
  public static SoundManager Instance { get; private set; }

  [SerializeField] private SoundLibrary library;
  [SerializeField] private int sfxPoolSize = 4;

  private AudioSource[] sfxSources;
  private int sfxIndex = 0;

  private void Awake()
  {
    if (Instance != null)
    {
      Destroy(gameObject);
      return;
    }

    Instance = this;
    DontDestroyOnLoad(gameObject);

    sfxSources = new AudioSource[sfxPoolSize];
    for (int i = 0; i < sfxPoolSize; i++)
    {
      AudioSource src = gameObject.AddComponent<AudioSource>();
      src.playOnAwake = false;
      sfxSources[i] = src;
    }
  }

  public void PlaySFX(AudioClip clip, float volume = 1f)
  {
    if (clip == null || sfxSources == null || sfxSources.Length == 0) return;

    AudioSource src = sfxSources[sfxIndex];
    sfxIndex = (sfxIndex + 1) % sfxSources.Length;

    src.clip = clip;
    src.volume = volume;
    src.Play();
  }

  public void PlayTargetHit() => PlaySFX(library?.targetHit);
  public void PlayKnifeMiss() => PlaySFX(library?.knifeMiss);
  public void PlayKnifeThrow() => PlaySFX(library?.knifeThrow);
}
