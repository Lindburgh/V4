using UnityEngine;

  public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    public bool allowHorizontal = true;  // ←→
    public bool allowVertical = true;    // ↑↓

    void Update()
    {
        float moveX = allowHorizontal ? Input.GetAxisRaw("Horizontal") : 0f;
        float moveY = allowVertical ? Input.GetAxisRaw("Vertical") : 0f;

        Vector3 move = new Vector3(moveX, moveY, 0f).normalized;
        transform.position += move * moveSpeed * Time.deltaTime;
    }
}
