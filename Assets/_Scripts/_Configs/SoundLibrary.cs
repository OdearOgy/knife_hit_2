using UnityEngine;

[CreateAssetMenu(fileName = "SoundLibrary", menuName = "Knife Hit/Sound Library")]
public class SoundLibrary : ScriptableObject
{

  [Header("Target Hit Sound")]
  public AudioClip targetHit;

  [Header("Knife clash sound")]
  public AudioClip knifeMiss;

  [Header("Knife throw sound")]
  public AudioClip knifeThrow;

  [Header("Log Breaking sound")]
  public AudioClip logBreaking;
}
