using UnityEngine;

public class Knife : MonoBehaviour
{

    [SerializeField] private float speed = 12f;
    private bool isMoving = false;

    [ContextMenu("Launch knife")]
    public void Launch() {
      isMoving = true;
    }

    void Update()
    {
        if (!isMoving) {
          return;

        }


        transform.position += Vector3.up * speed * Time.deltaTime;
    }
}
