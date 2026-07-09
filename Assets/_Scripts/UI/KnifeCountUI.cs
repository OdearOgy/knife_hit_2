using UnityEngine;
using System.Collections.Generic;

public class KnifeCountUI : MonoBehaviour
{
  [Header("References")]
  [SerializeField] private Transform container;
  [SerializeField] private SpriteRenderer knifeIconPrefab;

  [Header("Appearance")]
  [SerializeField] private Color activeColor = Color.white;
  [SerializeField] private Color usedColor = new Color(0.4f, 0.4f, 0.4f, 0.6f);
  [SerializeField] private float iconSpacing = 0.5f;
  [SerializeField] private float iconScale = 0.3f;

  private List<SpriteRenderer> icons = new List<SpriteRenderer>();
  private int topIndex = 0;

  public void Setup(int totalKnives)
  {
    foreach (var icon in icons)
    {
      if (icon != null) Destroy(icon.gameObject);
    }
    icons.Clear();
    topIndex = 0;

    if (knifeIconPrefab == null)
    {
      Debug.LogWarning("[KnifeCountUI] Knife Icon Prefab is missing!");
      return;
    }
    if (container == null)
    {
      Debug.LogWarning("[KnifeCountUI] Container is missing!");
      return;
    }

    for (int i = 0; i < totalKnives; i++)
    {
      SpriteRenderer icon = Instantiate(knifeIconPrefab, container);
      icon.color = activeColor;

      Transform t = icon.transform;
      t.localPosition = new Vector3(0f, i * iconSpacing, 0f);
      t.localScale = Vector3.one * iconScale;

      icons.Add(icon);
    }
  }

  public void MarkOneUsed()
  {
    int index = icons.Count - 1 - topIndex;
    if (index >= 0 && index < icons.Count)
    {
      icons[index].color = usedColor;
      topIndex++;
    }
  }

  public void ResetAll()
  {
    topIndex = 0;
    foreach (var icon in icons)
    {
      if (icon != null) icon.color = activeColor;
    }
  }
}
