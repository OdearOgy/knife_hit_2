using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Knife Hit/Level Config")]
public class LevelConfig : ScriptableObject
{
    [Header("Identity")]
    public int levelNumber;
    public string levelName;

    [Header("Knives")]
    public int knivesToThrow;
    public float[] stuckKnifeAngles;

    [Header("Apples")]
    public int minApples;
    public int maxApples;

    [Header("Rotation")]
    public float rotationSpeed;
    public bool clockwise;
    public bool changesDirection;

    [Header("Visual")]
    public Sprite targetSprite;

    public static bool IsBoss(int levelNumber) => levelNumber % 5 == 0;
}
