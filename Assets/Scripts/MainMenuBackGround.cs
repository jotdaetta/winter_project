using UnityEngine;

public class MainMenuBackGround : MonoBehaviour
{
    [SerializeField] private float moveAmount = 10f; // 이동 최대 거리
    [SerializeField] private float smoothSpeed = 5f; // 부드러운 이동 속도
    [SerializeField] private float floatingValue = 0.2f; // 상하로 둥가둥가
    private Vector3 targetPosition;
    private Vector3 startPosition;

    float i = 0;

    void Start()
    {
        startPosition = transform.position; // 초기 위치 저장
    }

    void Update()
    {
        FixPosChanged();

        MoveBackground();
    }

    void FixPosChanged() {
        if (Vector2.Distance(startPosition, transform.position) > moveAmount + 1) {
            Debug.Log("changed");
            startPosition = transform.position;
        }
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
        targetPosition = startPosition + Vector3.ClampMagnitude(offset, 1);

        i += Time.deltaTime;

        // 부드럽게 이동 (Lerp 사용)
        transform.position = Vector3.Lerp(transform.position, targetPosition + new Vector3(0, Mathf.Sin(i) * floatingValue), Time.deltaTime * smoothSpeed);
    }
}
