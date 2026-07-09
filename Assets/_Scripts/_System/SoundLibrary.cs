using UnityEngine;

[CreateAssetMenu(fileName = "SoundLibrary", menuName = "Knife Hit/Sound Library")]
public class SoundLibrary : ScriptableObject
{
  public AudioClip targetHit;
  public AudioClip knifeMiss;
  public AudioClip knifeThrow;
  public AudioClip levelWin;
  public AudioClip levelLose;
}
