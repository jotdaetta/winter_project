using System.Collections;
using UnityEngine;

public class EnemyFight : Gun, IDamageable
{
    public int hp = 2;
    public int failedExecuteHp = 1;
    public bool IsStunned;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] EnemyContoller contoller;
    public Transform playerTransform;

    #region InCombat
    public void OnCombat()
    {
        if (!ThereIsWall())
        {
            if (getCurAmmo > 0)
            {
                if (getShootable && CheckAim())
                {
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
        Attack(transform.right);
    }
    IEnumerator Reloading(float reloading_time)
    {
        yield return new WaitForSeconds(reloading_time);
        Reload();
        onReload = false;
    }
    #endregion
    #region Damage
    public void TakeDamage(int damage, bool isknife = false)
    {
        hp -= damage;
        print("hp: " + hp);
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
    #endregion
}
