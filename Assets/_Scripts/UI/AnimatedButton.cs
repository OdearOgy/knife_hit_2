using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AnimatedButton : Button
{
  [SerializeField] private float pressedScale = 0.9f;
  [SerializeField] private float animDuration = 0.1f;

  private Vector3 originalScale;
  private Coroutine currentAnimation;

  protected override void Awake()
  {
    base.Awake();
    originalScale = transform.localScale;
  }

  public override void OnPointerDown(PointerEventData eventData)
  {
    base.OnPointerDown(eventData);
    AnimateTo(originalScale * pressedScale);
  }

  public override void OnPointerUp(PointerEventData eventData)
  {
    base.OnPointerUp(eventData);
    AnimateTo(originalScale);
  }

  private void AnimateTo(Vector3 target)
  {
    if (currentAnimation != null) StopCoroutine(currentAnimation);
    currentAnimation = StartCoroutine(ScaleAnim(target));
  }

  private System.Collections.IEnumerator ScaleAnim(Vector3 target)
  {
    Vector3 start = transform.localScale;
    float elapsed = 0f;

    while (elapsed < animDuration)
    {
      transform.localScale = Vector3.Lerp(start, target, elapsed / animDuration);
      elapsed += Time.deltaTime;
      yield return null;
    }

    transform.localScale = target;
    currentAnimation = null;
  }
}
