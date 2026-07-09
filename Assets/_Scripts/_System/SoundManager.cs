using UnityEngine;

public class SoundManager : MonoBehaviour
{
  public static SoundManager Instance { get; private set; }

  [SerializeField] private SoundLibrary library;
  [SerializeField] private AudioSource sfxSource;
  [SerializeField] private AudioSource musicSource;

  private void Awake()
  {
    if (Instance != null)
    {
      Destroy(gameObject);
      return;
    }

    Instance = this;
    DontDestroyOnLoad(gameObject);
  }

  public void PlaySFX(AudioClip clip, float volume = 1f)
  {
    if (clip == null || sfxSource == null) return;
    sfxSource.PlayOneShot(clip, volume);
  }

  public void PlayTargetHit() => PlaySFX(library?.targetHit);
  public void PlayKnifeMiss() => PlaySFX(library?.knifeMiss);
  public void PlayKnifeThrow() => PlaySFX(library?.knifeThrow);
  // public void PlayLevelWin() => PlaySFX(library?.levelWin);
  // public void PlayLevelLose() => PlaySFX(library?.levelLose);
}
