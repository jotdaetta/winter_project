using System.Collections;
using UnityEngine;

public class EnemyFight : Gun, IDamageable
{
    public int hp = 5;
    public bool isStunned;
    public bool IDead;
    public bool onExecution;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] GameObject stunText;
    [SerializeField] Transform canvas;
    [SerializeField] EnemyContoller controller;
    [SerializeField] GameProcessManager processManager;
    [SerializeField] SpriteRenderer myRenderer;
    [SerializeField] private float laserDuration = 0.1f;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] float captureToShootDelay = 0.5f; // 처음 보고 쏘기까지 딜레이
    public Transform playerTransform;

    void Start()
    {
        if (lineRenderer != null)
            lineRenderer.enabled = false;
        canvas = GameObject.Find("StunTransform").transform;
        processManager = GameObject.Find("GameProcessManager").GetComponent<GameProcessManager>();
    }
    void FixedUpdate()
    {
        if (hp <= 0) StartCoroutine(OnDead());
        if (stunObj != null) stunObj.transform.position = transform.position;
        if (onExecution)
        {
            isStunned = true;
            if (stunObj != null)
                Destroy(stunObj);
        }
        Lasor();
    }

    #region InCombat
    public void OnCombat()
    {
        if (!ThereIsWall())
        {
            if (getCurAmmo > 0)
            {
                if (getShootable && CheckAim())
                {
                    shootable = false;
                    Shoot();
                }
            }
            else
            {
                if (!onReload)
                {
                    onReload = true;
                    StartCoroutine(Reloading(getReloadTime));
                }
            }
        }
    }
    #endregion
    #region Gun
    bool onReload;
    public void Shoot()
    {
        if (!onExecution)
        {
            if (getCurAmmo > 0)
            {
                LaserAndShoot();

            }
        }
    }
    IEnumerator Reloading(float reloading_time)
    {
        yield return new WaitForSeconds(reloading_time);
        Reload();
        onReload = false;
    }
    #endregion
    #region Damage
    int stunCount;
    [SerializeField] float stunTime;
    GameObject stunObj;
    public bool TakeDamage(int damage, bool isknife = false)
    {
        controller.SetCombat(true);
        if (isknife && isStunned)
        {
            onExecution = true;
            return true;
        }
        StartCoroutine(HitRenderer());
        hp -= damage;
        if (!isStunned && ++stunCount == 2)
        {
            stunCount = 0;
            isStunned = true;
            StartCoroutine(Stun());
            stunObj = Instantiate(stunText, canvas);
        }
        return false;
    }
    [SerializeField] Color HitColor = Color.red;
    IEnumerator HitRenderer()
    {
        myRenderer.color = HitColor;
        yield return new WaitForSeconds(0.3f);
        myRenderer.color = Color.white;
    }
    IEnumerator Stun()
    {
        yield return new WaitForSeconds(stunTime);
        if (stunObj != null)
            Destroy(stunObj);
        isStunned = false;
    }
    #endregion
    #region  Extra
    bool ThereIsWall()
    {
        if (playerTransform == null) return true;
        Vector2 myPos = transform.position;
        Debug.DrawLine(myPos, playerTransform.position, Color.green, 0.1f);
        return Physics2D.Linecast(myPos, playerTransform.position, wallLayer).collider != null;
    }
    bool CheckAim()
    {
        if (playerTransform == null) return false;

        Vector2 direction = playerTransform.position - transform.position;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // 실제 각도 값(도 단위)을 얻기 위해 transform.eulerAngles.z 사용
        float myAngle = transform.eulerAngles.z;

        // 두 각도의 차이를 계산 (회전 각도의 최소 차이를 반환)
        float angleDifference = Mathf.DeltaAngle(myAngle, targetAngle);

        // 차이가 +-15도 이내면 true 반환
        return Mathf.Abs(angleDifference) <= 15f;
    }
    public void StunRecover()
    {
        onExecution = false;
        isStunned = false;
        if (stunObj != null) Destroy(stunObj);
    }
    #endregion
    #region Lasor
    void Lasor()
    {
        Vector2 startPoint = (Vector2)transform.position + (Vector2)transform.right * shootOffset.y + (Vector2)transform.up * shootOffset.x;
        Vector2 direction = transform.right;

        RaycastHit2D hit = Physics2D.Raycast(startPoint, direction, maxDistance, detectLayer);
        Vector2 endPoint = hit ? hit.point : startPoint + direction * maxDistance;

        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);
    }
    public void LaserAndShoot()
    {
        if (lineRenderer == null)
        {
            controller.animations.Attack();
            Attack(transform.right);
            return;
        }
        StartCoroutine(LaserRoutine());
    }
    IEnumerator LaserRoutine()
    {
        yield return new WaitForSeconds(captureToShootDelay);
        lineRenderer.enabled = true;

        yield return new WaitForSeconds(laserDuration);
        lineRenderer.enabled = false;
        if (isStunned) yield break;
        controller.animations.Attack();
        Attack(transform.right);
    }
    #endregion
    #region  Death
    float disappearTime = 0.6f;
    public void Killed()
    {
        IDead = true;
        StartCoroutine(OnDead());
    }
    IEnumerator OnDead()
    {
        isStunned = true;
        float elapsed = 0f;

        if (stunObj != null)
        {
            Destroy(stunObj);
        }

        transform.GetChild(0).TryGetComponent<SpriteRenderer>(out SpriteRenderer renderer);
        renderer.color = new Color(0, 0, 0, 0);
        while (elapsed < disappearTime)
        {
            elapsed += Time.deltaTime;
            myRenderer.color = new Color(1, 1, 1, Mathf.Lerp(1, 0f, elapsed / disappearTime));
            yield return null;
        }
        Destroy(gameObject);
    }
    void OnDestroy()
    {
        processManager.enemyCount--;
    }
    #endregion
}
