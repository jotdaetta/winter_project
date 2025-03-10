using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFight : Gun, IDamageable
{
    public int hp = 10;
    public bool isStunned;
    bool distroied;
    [SerializeField] float mujukTime;
    [SerializeField] float stunTime = 0.5f;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] PlayerController controller;
    public Vector2 aimDir = new();

    delegate void MyFunc();
    [SerializeField] CamManager camManager;

    private void FixedUpdate()
    {
        ExecutionGaugeSet();
        if (stunObj != null) stunObj.transform.position = transform.position;
    }
    private void Update()
    {
        AimableToLockedEnemy();
        CheckTargetChangeCondition();
        SetAim();
        // DDIS()
        if (distroied && stunObj != null)
        {
            Destroy(stunObj);
        }
    }
    // void DDIS(MyFunc func)
    // {
    // }
    #region Gun
    bool onReload;
    public void Shoot()
    {
        if (onReload) return;
        Attack(transform.right);
    }
    public bool onReloading { get { return onReload; } }
    public void Reloading(float reloading_time)
    {
        SoundManager.Instance.Play("sfx.reload");

        onReload = true;
        StartCoroutine(Reloading_(reloading_time));
    }
    IEnumerator Reloading_(float reloading_time)
    {
        yield return new WaitForSeconds(reloading_time);
        onReload = false;
        Reload();
    }
    #endregion
    #region Damage
    public bool TakeDamage(int damage, bool isknife = false)
    {
        hp -= damage;
        StartCoroutine(Mujuk());
        return false;
    }
    #endregion
    #region Lock On
    [Header("록온")]
    [SerializeField] LayerMask WallLayer;
    [SerializeField] float LockOnMaxLostDistance = 15;
    List<GameObject> enemies = new();
    SpriteRenderer targetRenderer = null;
    public bool canAimToLockedEnemy;
    void AimableToLockedEnemy()
    {
        canAimToLockedEnemy = lockedOn && enemies.Count > 0;
    }
    bool lockedOn;
    float findingInterval;
    int index, renderStack = 0;
    public void LockOn()
    {
        if (lockedOn)
        {
            lockedOn = false;
            SetRenderer(true);
            return;
        }
        enemies = new();
        renderStack = 3;
        lockedOn = true;
        SetLockOn();
    }
    public void ChangeLockOn()
    {
        if (!lockedOn) return;
        if (index < enemies.Count)
            enemies[index] = null;
        SetLockOn();
    }
    void SetLockOn()
    {
        enemies = enemies.Where(e => e != null).ToList();
        if (enemies.Count == 0)
            enemies = DeleteBeyondWallEnemy(GameObject.FindGameObjectsWithTag("enemy"));
        if (enemies.Count == 0) return;
        index = FindMinDistEnemy();
        if (index == -1)
        {
            SetRenderer(true);
            return;
        }
        SetRenderer();
    }
    void SetRenderer(bool off = false)
    {
        if (targetRenderer != null)
        {
            targetRenderer.color = Color.black;
            if (off) return;
        }
        if (index >= 0 && index < enemies.Count)
        {
            SpriteRenderer renderer = enemies[index].transform.GetChild(0).GetComponent<SpriteRenderer>();
            if (targetRenderer != null && renderer == targetRenderer && renderStack < 3)
            {
                renderStack++;
                ChangeLockOn();
                return;
            }
            renderStack = 0;
            targetRenderer = renderer;
            targetRenderer.color = Color.white;
        }
    }
    void CheckTargetChangeCondition()
    {
        if (!lockedOn) return;
        // enemy zero
        if (enemies.Count == 0)
        {
            SetRenderer(true);
            EnemyFindingCondition();
        }
        //enemy dead
        if (targetRenderer == null)
        {
            SetLockOn();
            return;
        }
        // Beyond Distance
        if (Vector2.Distance(transform.position, targetRenderer.transform.position) > LockOnMaxLostDistance)
        {
            ChangeLockOn();
        }
        // wallbang
        if (CheckWallBang(transform.position, targetRenderer.transform.position))
        {
            ChangeLockOn();
        }
    }
    void EnemyFindingCondition()
    {
        findingInterval += Time.deltaTime;
        if (findingInterval > 0.2f)
        {
            findingInterval = 0;
            SetLockOn();
        }
    }
    List<GameObject> DeleteBeyondWallEnemy(GameObject[] enemies)
    {
        for (int i = enemies.Length - 1; i >= 0; i--)
        {
            if (CheckWallBang(transform.position, enemies[i].transform.position) || Vector2.Distance(transform.position, enemies[i].transform.position) > maxDistance)
                enemies[i] = null;
        }
        return enemies.Where(e => e != null).ToList();
    }
    int FindMinDistEnemy()
    {
        float min = float.MaxValue;
        int index = 0;
        for (int i = 0; i < enemies.Count; i++)
        {
            float dist = Vector2.Distance(transform.position, enemies[i].transform.position);
            if (min > dist)
            {
                min = dist;
                index = i;
            }
        }
        return index;
    }
    bool CheckWallBang(Vector2 start_pos, Vector2 target_pos)
    {
        return Physics2D.Linecast(start_pos, target_pos, WallLayer).collider != null;
    }
    #endregion
    #region Execution
    [Header("처형")]
    [SerializeField] GameObject ex_UI;
    [SerializeField] GameObject stunText;
    [SerializeField] float ex_enemyStunRecoverTime = 1f;
    [SerializeField] Image ex_processImage;
    [SerializeField] float ex_time = 3;
    [SerializeField] float ex_processSub = 0.7f;
    [SerializeField] float ex_processAdd = 0.15f;
    [SerializeField] float ex_shakeStrength = 1;
    [SerializeField] Transform canvas;
    [SerializeField] float ex_chance = 50;
    GameObject stunObj;
    EnemyFight ex_target;
    float ex_process;
    public bool executing;
    float ex_eclipsed;
    public bool Chance(float probability)
    {
        return UnityEngine.Random.value < probability / 100f;
    }
    public void Execute(EnemyFight enemy)
    {
        if (executing) return;
        if (Chance(ex_chance))
        {
            isStunned = true;
            ex_target = enemy;
            gameObject.layer = 8;
            executing = true;
            ex_eclipsed = 0;
            ex_process = 0;
            ex_UI.SetActive(true);
            camManager.Shake(ex_shakeStrength, ex_time);
            camManager.CloseUp(2, 1.5f, ex_time);
        }
        else
        {
            enemy.Killed();
        }
    }
    public void ExecutionGaugeFill()
    {
        if (!executing) return;
        ex_process += ex_processAdd;
    }
    [SerializeField] float afterExetutionMujuk = 0.5f;
    void ExecutionGaugeSet()
    {
        // x = Mathf.Clamp(x, 0, gridSizeX - 1);
        if (!executing) return;
        ex_eclipsed += Time.deltaTime;
        ex_process -= Time.deltaTime * ex_processSub;
        ex_process = Mathf.Clamp(ex_process, 0, 1);
        float gb = 1 - ex_process;
        ex_processImage.fillAmount = ex_process;
        Color color = new Color(1, gb, gb);
        ex_processImage.color = color;
        if (ex_process == 1 || ex_eclipsed >= ex_time)
        {
            StartCoroutine(GiveDelay(() => gameObject.layer = 3, afterExetutionMujuk));
            executing = false;
            camManager.StopShake();
            camManager.CloseOut(0.3f);
            StartCoroutine(GiveDelay(() => ex_UI.SetActive(false), 0.5f));
            if (ex_process != 1)
            {
                ex_processImage.color = Color.gray;
                stunObj = Instantiate(stunText, canvas);
                StartCoroutine(GiveDelay(() => { isStunned = false; if (stunObj != null) Destroy(stunObj); }, stunTime));
                StartCoroutine(GiveDelay(() => ex_target.StunRecover(), ex_enemyStunRecoverTime));
            }
            else
            {
                isStunned = false;
                ex_target.Killed();
            }
        }
    }
    #endregion
    #region Extra
    IEnumerator GiveDelay(MyFunc func, float delay)
    {
        yield return new WaitForSeconds(delay);
        func();
    }
    IEnumerator Mujuk()
    {
        gameObject.layer = 8;
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        yield return new WaitForSeconds(mujukTime);
        gameObject.layer = 3;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }
    void SetAim()
    {
        if (targetRenderer == null) return;
        aimDir = targetRenderer.transform.position;
    }
    public void DelStunObj()
    {
        distroied = true;
    }
    #endregion
}