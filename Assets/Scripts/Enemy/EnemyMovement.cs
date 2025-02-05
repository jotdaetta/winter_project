using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor; // Gizmo 시각화를 위해 추가

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] LayerMask wallMask;

    [System.Serializable]
    public class PatrolPoint
    {
        public Vector2 position;
        public float moveTime = 1f;
        public float waitTime = 1f;
    }

    [Header("Patrol Settings")]
    public List<PatrolPoint> patrolPath = new List<PatrolPoint>();

    [Header("Combat Settings")]
    public float combatSpeed = 5f;
    public float minCombatDistance = 2f;
    public float maxCombatDistance = 10f;
    public float sightRadius = 10f;

    [Header("References")]
    public Transform playerTransform;

    private bool inCombat = false;
    private Coroutine patrolCoroutine;
    private CircleCollider2D sightCollider;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        InitializeSightCollider();
        StartPatrol();
    }

    void InitializeSightCollider()
    {
        sightCollider = GetComponent<CircleCollider2D>();
        if (sightCollider == null)
        {
            sightCollider = gameObject.AddComponent<CircleCollider2D>();
        }
        sightCollider.radius = sightRadius;
        sightCollider.isTrigger = true;
    }

    void StartPatrol()
    {
        if (patrolPath.Count > 0 && !inCombat)
        {
            patrolCoroutine = StartCoroutine(PatrolRoutine());
        }
    }

    IEnumerator PatrolRoutine()
    {
        int currentIndex = 0;

        while (true)
        {
            Vector2 startPos = rb.position;
            PatrolPoint target = patrolPath[currentIndex];
            float elapsed = 0f;

            // 부드러운 이동
            while (elapsed < target.moveTime)
            {
                if (inCombat) yield break;

                rb.MovePosition(Vector2.Lerp(
                    startPos,
                    target.position,
                    elapsed / target.moveTime
                ));
                elapsed += Time.deltaTime;
                yield return null;
            }

            rb.MovePosition(target.position);

            // 대기 시간
            yield return new WaitForSeconds(target.waitTime);

            currentIndex = (currentIndex + 1) % patrolPath.Count;
        }
    }

    void FixedUpdate()
    {
        if (inCombat && playerTransform != null)
        {
            HandleCombatMovement();
        }
    }

    void HandleCombatMovement()
    {
        Vector2 toPlayer = (Vector2)playerTransform.position - rb.position;
        float distance = toPlayer.magnitude;
        Vector2 direction = toPlayer.normalized;

        // 벽 충돌 검사
        RaycastHit2D wallHit = Physics2D.Raycast(rb.position, direction, combatSpeed * Time.fixedDeltaTime, wallMask);

        if (wallHit.collider != null)
        {
            // 벽이 있으면 멈춤
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (distance < minCombatDistance)
        {
            // 플레이어 반대 방향으로 이동
            rb.linearVelocity = -direction * combatSpeed;
        }
        else if (distance > maxCombatDistance)
        {
            // 플레이어 방향으로 이동
            rb.linearVelocity = direction * combatSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("player")) // 태그 대소문자 확인
        {
            // 플레이어와 적 사이에 벽이 있는지 확인
            RaycastHit2D hit = Physics2D.Linecast(transform.position, playerTransform.position, wallMask);

            if (hit.collider != null) // 벽이 감지되면
            {
                Debug.Log("플레이어 감지 실패! 벽에 가려짐: " + hit.collider.name);
                return; // 감지 중단
            }

            Debug.Log("플레이어 감지, 전투 시작!");
            inCombat = true;
            if (patrolCoroutine != null)
            {
                StopCoroutine(patrolCoroutine);
            }
        }
    }
    void OnDrawGizmos()
    {
        if (playerTransform != null)
        {
            // 기본 라인 색상 (녹색: 플레이어가 보이는 상태)
            Gizmos.color = Color.green;

            // 벽이 가로막고 있는지 확인
            RaycastHit2D hit = Physics2D.Linecast(transform.position, playerTransform.position, wallMask);

            if (hit.collider != null) // 벽이 감지되면 빨간색으로 변경
            {
                Gizmos.color = Color.red;
            }

            // 적과 플레이어 사이 라인 그리기
            Gizmos.DrawLine(transform.position, playerTransform.position);
        }
    }


    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("player"))
        {
            inCombat = false;
            rb.linearVelocity = Vector2.zero;
            StartPatrol();
        }
    }

    // 인스펙터에서 경로 시각화
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        foreach (PatrolPoint point in patrolPath)
        {
            Gizmos.DrawWireSphere(point.position, 0.3f);
#if UNITY_EDITOR
            Handles.Label(point.position, $"Point {patrolPath.IndexOf(point)}");
#endif
        }
    }
}