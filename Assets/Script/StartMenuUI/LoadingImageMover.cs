using UnityEngine;

public class LoadingImageMover : MonoBehaviour
{
    [SerializeField] private RectTransform imageRect;  // 이미지를 가진 RectTransform
    [SerializeField] private RectTransform viewportRect;  // 이미지를 표시할 뷰포트의 RectTransform
    [SerializeField] private float moveSpeed = 10f;  // 움직임 속도

    private Vector2 minPosition;  // 이미지가 이동할 수 있는 최소 좌표
    private Vector2 maxPosition;  // 이미지가 이동할 수 있는 최대 좌표
    private Vector2 direction = Vector2.right;  // 움직임 방향, 상하 움직임은 미구현

    void Start()
    {
        // 이미지가 뷰포트를 벗어나지 않도록 제한하는 좌표 설정
        float viewportWidth = viewportRect.rect.width;
        float viewportHeight = viewportRect.rect.height;
        float imageWidth = imageRect.rect.width;
        float imageHeight = imageRect.rect.height;

        minPosition = new Vector2(viewportWidth / 2 - imageWidth / 2, viewportHeight / 2 - imageHeight / 2);
        maxPosition = new Vector2(imageWidth / 2 - viewportWidth / 2, imageHeight / 2 - viewportHeight / 2);
    }

    void Update()
    {
        // 시간에 따라 이미지 이동
        Vector2 newPosition = imageRect.anchoredPosition + direction * moveSpeed * Time.deltaTime;
        
        // 경계 검사 및 방향 반전
        if (newPosition.x < minPosition.x || newPosition.x > maxPosition.x)
        {
            direction.x *= -1;
            newPosition.x = Mathf.Clamp(newPosition.x, minPosition.x, maxPosition.x);
        }

        if (newPosition.y < minPosition.y || newPosition.y > maxPosition.y)
        {
            direction.y *= -1;
            newPosition.y = Mathf.Clamp(newPosition.y, minPosition.y, maxPosition.y);
        }

        imageRect.anchoredPosition = newPosition;
    }
}
