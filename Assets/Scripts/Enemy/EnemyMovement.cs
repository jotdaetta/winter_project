using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

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
    public float combatSpeed = 5f;         // 전투 시 이동 속도
    public float minCombatDistance = 5f;   // 플레이어와 너무 가까우면 후퇴할 최소 거리
    public float maxCombatDistance = 7f;   // 플레이어와 너무 멀면 접근할 최대 거리
    public float findPlayerDistance = 10f; // 플레이어 탐지 거리
    public float missPlayerDistance = 10f; // 플레이어와의 거리가 이보다 멀면 전투 모드 종료

    [Header("References")]
    public Transform playerTransform;      // 플레이어 Transform

    private bool inCombat = false;         // 전투 모드 여부
    [SerializeField] Rigidbody2D rb;        // Rigidbody2D 컴포넌트 참조
    [SerializeField] CircleCollider2D circleCollider; // 플레이어 탐지를 위한 콜라이더

    // A* 경로 찾기를 위한 변수들
    private List<Node> path;               // 현재 경로 (노드 리스트)
    private int currentPathIndex = 0;      // 경로상의 현재 타겟 노드 인덱스
    [SerializeField] Vector2 gridWorldSize = new Vector2(20, 20); // 그리드 전체 크기
    [SerializeField] float nodeRadius = 0.5f;                     // 노드 반지름 (노드 크기의 반)
    [SerializeField] float pathUpdateInterval = 0.5f;             // 경로 업데이트 간격 (초)

    void Start()
    {
        SetCollider();
        StartCoroutine(UpdatePath());
        // StartPatrol();
    }

    void FixedUpdate()
    {
        if (inCombat && playerTransform != null)
        {
            FollowPath();
        }
    }

    // 플레이어 탐지를 위한 콜라이더의 반지름을 설정
    void SetCollider()
    {
        if (circleCollider != null)
        {
            circleCollider.radius = findPlayerDistance;
        }
    }

    // 경로를 따라 이동하는 함수
    void FollowPath()
    {
        if (path != null && path.Count > 0)
        {
            // 현재 목표 노드의 월드 위치 (2D 환경이므로 z는 0)
            Vector2 targetPos = path[currentPathIndex].worldPosition;
            Vector2 currentPos = rb.position;
            Vector2 moveDir = (targetPos - currentPos).normalized;
            rb.linearVelocity = moveDir * combatSpeed;

            // 목표 노드에 충분히 가까워지면 다음 노드로 진행
            if (Vector2.Distance(currentPos, targetPos) < 0.1f)
            {
                currentPathIndex++;
                if (currentPathIndex >= path.Count)
                {
                    // 경로의 끝에 도달하면 이동을 멈춤
                    rb.linearVelocity = Vector2.zero;
                }
            }
        }
        else
        {
            // 경로가 없으면 기존 전투 이동 로직 사용 (참고용)
            HandleCombatMovement();
        }
    }

    // 기존 전투 이동 로직 (경로가 없을 경우의 대체 동작)
    void HandleCombatMovement()
    {
        Vector2 toPlayer = (Vector2)playerTransform.position - rb.position;
        Vector2 direction = toPlayer.normalized;
        float distance = toPlayer.magnitude;
        if (distance > missPlayerDistance)
        {
            inCombat = false;
            rb.linearVelocity = Vector2.zero;
            return;
        }
        if (distance < minCombatDistance)
            rb.linearVelocity = -direction * combatSpeed;
        else if (distance > maxCombatDistance)
            rb.linearVelocity = direction * combatSpeed;
        else
            rb.linearVelocity = Vector2.zero;
    }

    // 일정 주기마다 현재 적의 위치와 플레이어의 위치를 기준으로 A* 알고리즘을 사용해 경로를 계산
    IEnumerator UpdatePath()
    {
        while (true)
        {
            if (inCombat && playerTransform != null)
            {
                // 적의 현재 위치를 중심으로 그리드 생성
                Grid grid = new Grid(rb.position, gridWorldSize, nodeRadius, wallMask);
                Node startNode = grid.NodeFromWorldPoint(rb.position);
                Node targetNode = grid.NodeFromWorldPoint(playerTransform.position);
                List<Node> newPath = AStar.FindPath(startNode, targetNode, grid);
                if (newPath != null && newPath.Count > 0)
                {
                    path = newPath;
                    currentPathIndex = 0;
                }
            }
            yield return new WaitForSeconds(pathUpdateInterval);
        }
    }

    // 플레이어와 충돌 중일 때 호출됨 (플레이어 탐지)
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("player"))
        {
            // 적과 플레이어 사이에 wallMask에 해당하는 장애물이 있는지 확인
            RaycastHit2D hit = Physics2D.Linecast(transform.position, playerTransform.position, wallMask);
            if (hit.collider != null)
            {
                return;
            }
            inCombat = true;
            // 필요시 순찰 중단 등의 추가 처리를 할 수 있음
        }
    }
}

// ------------------------------
// A* 알고리즘 관련 클래스들
// ------------------------------

// A* 알고리즘에서 사용되는 노드 클래스 (각 그리드 셀)
public class Node
{
    public bool walkable;         // 해당 노드가 이동 가능한지 여부
    public Vector2 worldPosition; // 노드의 월드 좌표
    public int gridX;             // 그리드 상의 X 인덱스
    public int gridY;             // 그리드 상의 Y 인덱스

    public int gCost;             // 시작 노드부터 이 노드까지의 이동 비용
    public int hCost;             // 목표 노드까지의 예상 비용
    public Node parent;           // 경로 추적을 위한 부모 노드

    public int fCost { get { return gCost + hCost; } } // 총 비용

    public Node(bool walkable, Vector2 worldPosition, int gridX, int gridY)
    {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
        this.gridX = gridX;
        this.gridY = gridY;
        gCost = int.MaxValue; // 초기값은 매우 큰 값으로 설정
        hCost = 0;
        parent = null;
    }
}

// 월드 공간을 일정 영역의 그리드로 분할하여 각 셀을 노드로 관리하는 클래스
public class Grid
{
    public Node[,] nodes;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public float nodeDiameter;
    public int gridSizeX, gridSizeY;
    private Vector2 origin;
    private LayerMask wallMask;

    public Grid(Vector2 origin, Vector2 gridWorldSize, float nodeRadius, LayerMask wallMask)
    {
        this.origin = origin;
        this.gridWorldSize = gridWorldSize;
        this.nodeRadius = nodeRadius;
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        this.wallMask = wallMask;
        CreateGrid();
    }

    // 그리드를 생성하여 각 노드의 위치와 이동 가능 여부를 결정
    void CreateGrid()
    {
        nodes = new Node[gridSizeX, gridSizeY];
        Vector2 bottomLeft = origin - gridWorldSize / 2;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 worldPoint = bottomLeft + Vector2.right * (x * nodeDiameter + nodeRadius) +
                                     Vector2.up * (y * nodeDiameter + nodeRadius);
                // 해당 지점에 wallMask에 걸리는 장애물이 있으면 이동 불가능
                bool walkable = (Physics2D.OverlapCircle(worldPoint, nodeRadius, wallMask) == null);
                nodes[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    // 월드 좌표에 해당하는 노드를 반환
    public Node NodeFromWorldPoint(Vector2 worldPosition)
    {
        float percentX = (worldPosition.x - (origin.x - gridWorldSize.x / 2)) / gridWorldSize.x;
        float percentY = (worldPosition.y - (origin.y - gridWorldSize.y / 2)) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return nodes[x, y];
    }

    // 주어진 노드의 인접(대각선 포함) 노드들을 반환
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(nodes[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }
}

// 간단한 A* 경로 찾기 알고리즘 구현 클래스
public static class AStar
{
    // 시작 노드부터 목표 노드까지의 최적 경로(노드 리스트)를 반환
    public static List<Node> FindPath(Node startNode, Node targetNode, Grid grid)
    {
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);
        startNode.gCost = 0;
        startNode.hCost = GetDistance(startNode, targetNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost ||
                   (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                    continue;
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;
                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
        return null; // 경로를 찾지 못함
    }

    // 두 노드 간의 이동 비용 계산 (대각선 이동 비용 포함)
    static int GetDistance(Node a, Node b)
    {
        int dstX = Mathf.Abs(a.gridX - b.gridX);
        int dstY = Mathf.Abs(a.gridY - b.gridY);
        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    // 시작 노드부터 목표 노드까지의 경로를 역추적하여 반환
    static List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }
}
