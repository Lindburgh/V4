using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public bool allowHorizontal = true;
    public bool allowVertical = true;

    private Vector2 lastTouchPos;
    private bool isDragging = false;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        Vector3 move = Vector3.zero;

        // --- PCのキーボード入力 ---
        float moveX = allowHorizontal ? Input.GetAxisRaw("Horizontal") : 0f;
        float moveY = allowVertical ? Input.GetAxisRaw("Vertical") : 0f;
        move += new Vector3(moveX, moveY, 0f);

        // --- スマホのタッチ入力 ---
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPos = mainCam.ScreenToWorldPoint(touch.position);

            if (touch.phase == TouchPhase.Began)
            {
                lastTouchPos = touchPos;
                isDragging = true;
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                Vector2 delta = touchPos - lastTouchPos;

                float touchMoveX = allowHorizontal ? delta.x : 0f;
                float touchMoveY = allowVertical ? delta.y : 0f;
                move += new Vector3(touchMoveX, touchMoveY, 0f);

                lastTouchPos = touchPos;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
            }
        }

        // --- 移動適用 ---
        Vector3 newPos = transform.position + move * moveSpeed * Time.deltaTime;

        // --- 画面外に出ないよう制限 ---
        Vector3 clampedPos = ClampToScreenBounds(newPos);
        transform.position = clampedPos;
    }

    Vector3 ClampToScreenBounds(Vector3 position)
    {
        Vector3 viewPos = mainCam.WorldToViewportPoint(position);
        viewPos.x = Mathf.Clamp01(viewPos.x);
        viewPos.y = Mathf.Clamp01(viewPos.y);
        Vector3 clampedWorldPos = mainCam.ViewportToWorldPoint(viewPos);
        clampedWorldPos.z = 0f; // 2D用にz固定
        return clampedWorldPos;
    }
}
