using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Knife Hit/Level Config")]
public class LevelConfig : ScriptableObject
{
    [Header("Target")]
    public TargetController targetPrefab;

    [Header("Knives")]
    public int knivesToThrow;
    public int minStuckKnives;
    public int maxStuckKnives;
    public float[] stuckKnifeAngles;

    [Header("Apples")]
    public int minApples;
    public int maxApples;
    public float[] appleAngles;

    [Header("Rotation")]
    public float rotationSpeed;
    public bool clockwise;
    public bool changesDirection;
    public float directionChangeInterval;
}
