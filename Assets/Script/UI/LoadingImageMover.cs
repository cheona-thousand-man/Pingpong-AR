using UnityEngine;

public class LoadingImageMover : MonoBehaviour
{
    public RectTransform imageRectTransform;
    public float moveSpeed = 50f; // 이동 속도
    public float moveRange = 100f; // 이동 범위
    public float changeDirectionInterval = 1.0f; // 방향 변경 간격

    private Vector2 originalPosition;
    private Vector2 targetPosition;
    private float timer;
    private int moveStep = 0;

    void Start()
    {
        if (imageRectTransform == null)
        {
            imageRectTransform = GetComponent<RectTransform>();
        }
        originalPosition = imageRectTransform.anchoredPosition;
        SetNextTargetPosition();
    }

    void Update()
    {
        MoveImage();
        timer += Time.deltaTime;
        if (timer >= changeDirectionInterval)
        {
            SetNextTargetPosition();
            timer = 0;
        }
    }

    void MoveImage()
    {
        imageRectTransform.anchoredPosition = Vector2.MoveTowards(imageRectTransform.anchoredPosition, targetPosition, moveSpeed * Time.deltaTime);
    }

    void SetNextTargetPosition()
    {
        switch (moveStep)
        {
            case 0: // Move right
                targetPosition = new Vector2(originalPosition.x + moveRange, originalPosition.y);
                break;
            case 1: // Move down
                targetPosition = new Vector2(originalPosition.x + moveRange, originalPosition.y - moveRange);
                break;
            case 2: // Move left
                targetPosition = new Vector2(originalPosition.x, originalPosition.y - moveRange);
                break;
            case 3: // Move up (back to original position)
                targetPosition = originalPosition;
                break;
        }

        moveStep = (moveStep + 1) % 4;
    }
}
