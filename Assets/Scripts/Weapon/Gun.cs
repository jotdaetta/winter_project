using System.Collections;
using UnityEngine;

public class Gun : Weapon
{
    #region Gun

    [Header("Gun")]
    [SerializeField] private int maxAmmoCount = 8;
    [SerializeField] private int curAmmoCount;
    [SerializeField] private int totalAmmoCount;
    [SerializeField] private int ammoPerShot = 1;
    [SerializeField] private float shootingSpeed = 0.4f;
    [SerializeField] private float reloadingTime = 1;

    [SerializeField] private GameObject bulletTrailPrefab;
    [SerializeField] private float trailDuration = 0.05f;

    public Vector2 shootOffset = new();

    public bool shootable = true;


    public virtual bool Attack(Vector2 direction)
    {
        knifeAble = false;
        shootable = false;
        StartCoroutine(ShootingCool());

        if (curAmmoCount <= 0)
        {
            return false;
        }

        SoundManager.Instance.Play("sfx.pistol");

        int shootingAmmoCount = Mathf.Min(ammoPerShot, curAmmoCount);
        curAmmoCount -= shootingAmmoCount;

        Vector2 startPoint = (Vector2)transform.position + (Vector2)transform.right * shootOffset.y + (Vector2)transform.up * shootOffset.x;
        RaycastHit2D hit = Physics2D.Raycast(startPoint, direction.normalized, maxDistance, detectLayer);
        Vector2 endPoint = hit ? hit.point : startPoint + direction.normalized * maxDistance;

        CreateBulletTrail(startPoint, endPoint);

        if (hit)
        {
            print(hit.collider.gameObject.name);
            if (hit.collider.transform.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                damageable.TakeDamage(weaponDamage * shootingAmmoCount);
            }
        }

        return true;
    }

    private void CreateBulletTrail(Vector2 startPoint, Vector2 endPoint)
    {
        GameObject trail = Instantiate(bulletTrailPrefab, startPoint, Quaternion.identity);
        LineRenderer lr = trail.GetComponent<LineRenderer>();

        if (lr != null)
        {
            lr.SetPosition(0, startPoint);
            lr.SetPosition(1, endPoint);
        }
        lr.sortingLayerName = "effect";
        lr.sortingOrder = 0;
        StartCoroutine(DestroyTrail(trail, trailDuration));
    }

    private IEnumerator DestroyTrail(GameObject trail, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(trail);
    }

    public virtual void Reload()
    {
        int neededCount = maxAmmoCount - curAmmoCount; // 최대 총알 수를 채우기 위해 필요한 총알 수

        // 인벤토리에 충분한 총알이 있는 경우
        if (totalAmmoCount >= neededCount)
        {
            curAmmoCount += neededCount;  // 현재 총알을 최대치로 채움
            totalAmmoCount -= neededCount; // 인벤토리에서 총알 차감
        }
        else
        {
            // 인벤토리에 총알이 부족한 경우
            curAmmoCount += totalAmmoCount; // 인벤토리에 남은 총알만큼 현재 총알을 채움
            totalAmmoCount = 0;              // 인벤토리 총알은 모두 사용됨
        }
    }

    IEnumerator ShootingCool()
    {
        yield return new WaitForSeconds(shootingSpeed);
        knifeAble = true;
        shootable = true;
    }

    public bool CheckReloadable()
    {
        return maxAmmoCount != curAmmoCount;
    }

    public int getCurAmmo
    {
        get { return curAmmoCount; }
    }
    public int getTotalAmmo
    {
        get { return totalAmmoCount; }
    }
    public float getReloadTime
    {
        get { return reloadingTime; }
    }
    public bool getShootable
    {
        get { return shootable; }
    }
    #endregion
    #region Knife
    [Header("Knife")]
    [SerializeField] private Vector2 boxSize = new Vector2(0.5f, 0.5f); // 사각형 크기 조절 가능
    [SerializeField] private float attackDistance = 1f; // 공격 거리
    [SerializeField] private float attackDelay = 0.5f;
    [SerializeField] private float attackAfterDelay = 1f;
    [SerializeField] private Vector2 hitOffset = new Vector2(0f, 0f); // 히트 위치 오프셋
    public bool knifeAble = true;

    public void KnifeAttack()
    {
        knifeAble = false;
        shootable = false;
        StartCoroutine(KnifeAttack_());
    }

    IEnumerator KnifeAttack_()
    {
        yield return new WaitForSeconds(attackDelay);
        Vector2 origin = (Vector2)transform.position + (Vector2)transform.right * attackDistance * 0.5f + hitOffset; // 오프셋 적용
        RaycastHit2D hit = Physics2D.BoxCast(origin, boxSize, 0f, transform.right, attackDistance, detectLayer);

        if (hit.collider != null)
        {
            if (Vector2.Distance(transform.position, hit.collider.transform.position) < attackDistance)
            {
                if (hit.transform.TryGetComponent<IDamageable>(out IDamageable damageable))
                {
                    if (damageable.TakeDamage(weaponDamage, true))
                    {
                        transform.TryGetComponent<PlayerFight>(out PlayerFight fight);
                        hit.transform.TryGetComponent<EnemyFight>(out EnemyFight enfight);
                        fight.Execute(enfight);
                    }
                }
            }

        }
        yield return new WaitForSeconds(attackAfterDelay);
        shootable = true;
        knifeAble = true;
    }

    private void OnDrawGizmosSelected()
    {
        // if (Application.isPlaying) return; // 플레이 모드에서만 표시

        Gizmos.color = Color.red; // 박스 색상 지정
        Vector2 origin = (Vector2)transform.position + (Vector2)transform.right * attackDistance * 0.5f + hitOffset; // 오프셋 적용
        Gizmos.matrix = Matrix4x4.TRS(origin, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, (Vector3)boxSize);
    }
    #endregion
}
