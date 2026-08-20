using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Knife Hit/Player Config")]
public class PlayerConfig : ScriptableObject
{
    [Header("Selected Knife")]
    public Knife defaultKnife;
    public Knife playerKnife;

    [Header("Testing")]
    public LevelConfig forcedLevel;


}
