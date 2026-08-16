using UnityEngine;

public class ProceduralSoftCircle : MonoBehaviour
{
  [SerializeField] private int resolution = 128;
  [SerializeField] private float softness = 0.5f;

  private void Start()
  {
    SpriteRenderer sr = GetComponent<SpriteRenderer>();
    if (sr == null) return;

    Texture2D tex = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
    Color white = Color.white;
    Color clear = new Color(1f, 1f, 1f, 0f);
    float radius = resolution / 2f;
    float center = resolution / 2f;

    for (int y = 0; y < resolution; y++)
    {
      for (int x = 0; x < resolution; x++)
      {
        float dist = Mathf.Sqrt((x - center) * (x - center) + (y - center) * (y - center));
        float t = Mathf.Clamp01((dist / radius - softness) / (1f - softness));
        tex.SetPixel(x, y, Color.Lerp(white, clear, t));
      }
    }

    tex.Apply();
    tex.wrapMode = TextureWrapMode.Clamp;
    tex.filterMode = FilterMode.Bilinear;

    Sprite sprite = Sprite.Create(tex, new Rect(0, 0, resolution, resolution), new Vector2(0.5f, 0.5f), 100f);
    sr.sprite = sprite;
  }
}
