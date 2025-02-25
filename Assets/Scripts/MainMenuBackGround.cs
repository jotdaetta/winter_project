using UnityEngine;

public class MainMenuBackGround : MonoBehaviour
{
    [SerializeField] private float moveAmount = 10f; // 이동 최대 거리
    [SerializeField] private float smoothSpeed = 5f; // 부드러운 이동 속도
    private Vector3 targetPosition;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position; // 초기 위치 저장
    }

    void Update()
    {
        MoveBackground();
    }

    void MoveBackground()
    {
        // 마우스 위치를 (-1,1) 범위로 정규화 (0,0 기준)
        Vector3 mousePosition = Input.mousePosition;
        Vector3 normalizedMousePos = new Vector3(
            (mousePosition.x / Screen.width) * 2 - 1,
            (mousePosition.y / Screen.height) * 2 - 1,
            0
        );

        // 마우스 반대 방향으로 이동 (음수 곱하기)
        Vector3 offset = new Vector3(-normalizedMousePos.x, -normalizedMousePos.y, 0) * moveAmount;
        targetPosition = startPosition + offset;

        // 부드럽게 이동 (Lerp 사용)
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
    }
}
