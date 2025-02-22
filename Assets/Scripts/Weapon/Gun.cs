using System.Collections;
using UnityEngine;

public class Gun : Weapon
{
    [SerializeField] private int maxAmmoCount = 8;
    [SerializeField] private int curAmmoCount;
    [SerializeField] private int totalAmmoCount;
    [SerializeField] private int ammoPerShot = 1;
    [SerializeField] private float fireSpeed = 1;
    [SerializeField] private float reloadingTime = 0.1f;

    // 총알 이펙트 프리팹 (LineRenderer가 포함된 프리팹)
    [SerializeField] private GameObject bulletTrailPrefab;
    // 총알 트레일 지속 시간 (초)
    [SerializeField] private float trailDuration = 0.05f;

    public virtual bool Attack(Vector2 direction)
    {

        if (curAmmoCount <= 0)
        {
            return false; // 총알이 없으면 공격 실패
        }

        // 사용 가능한 총알 계산
        int shootingAmmoCount = Mathf.Min(ammoPerShot, curAmmoCount);
        curAmmoCount -= shootingAmmoCount;

        // 레이캐스트로 타격 처리
        Vector2 startPoint = (Vector2)transform.position;
        RaycastHit2D hit = Physics2D.Raycast(startPoint, direction.normalized, maxDistance, detectLayer);

        // 명중 위치 계산: 타격한 대상이 있으면 hit.point, 없으면 최대 거리까지
        Vector2 endPoint = hit ? hit.point : startPoint + direction.normalized * maxDistance;

        // Bullet Trail 효과 생성
        CreateBulletTrail(startPoint, endPoint);

        if (hit)
        {
            if (hit.transform.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                damageable.TakeDamage(weaponDamage * shootingAmmoCount);
            }
        }

        return true; // 공격 성공
    }

    private void CreateBulletTrail(Vector2 startPoint, Vector2 endPoint)
    {
        // 프리팹 인스턴스화
        GameObject trail = Instantiate(bulletTrailPrefab, startPoint, Quaternion.identity);
        LineRenderer lr = trail.GetComponent<LineRenderer>();

        if (lr != null)
        {
            lr.SetPosition(0, startPoint);
            lr.SetPosition(1, endPoint);
        }
        // 트레일을 trailDuration 후 제거
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
}
