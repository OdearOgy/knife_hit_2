using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Knife Hit/Level Config")]
public class LevelConfig : ScriptableObject
{
    [Header("Knives")]
    [Min(1)] public int knifeCount = 8;

    [Header("Target")]
    public float rotationSpeed = 50f;
    public bool rotateClockwise = true;

    [Header("Optional")]
    public Sprite targetSprite;
}
