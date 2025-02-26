using System.Collections;
using UnityEngine;

public class EnemyFight : Gun, IDamageable
{
    public int hp = 20;
    public bool isStunned;
    public bool onExecution;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] GameObject stunText;
    [SerializeField] Transform canvas;
    [SerializeField] EnemyContoller controller;
    public Transform playerTransform;

    void FixedUpdate()
    {
        if (stunObj != null) stunObj.transform.position = transform.position;
        if (onExecution)
        {
            isStunned = true;
            if (stunObj != null)
                Destroy(stunObj);
        }
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
}
