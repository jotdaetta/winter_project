using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameProcessManager processManager;
    [SerializeField] PlayerAnimationController aniController;
    [SerializeField] PlayerMovement movement;
    [SerializeField] PlayerFight fighting;
    [SerializeField] PlayerUI ui;
    void Start()
    {
        ui.PadExecution(false);
    }
    private void FixedUpdate()
    {
        if (!fighting.isStunned)
        {
            movement.Move();
            movement.Turn(fighting.canAimToLockedEnemy, fighting.aimDir);
        }
        if (fighting.executing) movement.RbStop();
    }
    private void Update()
    {
        Setting();
        CheckEnd();
        KeyControll();
        UIPadSet();
    }
    void Setting()
    {
        ui.SetAmmoText(fighting.getCurAmmo, fighting.getTotalAmmo);
        aniController.IsExecution(fighting.executing);
        aniController.MoveAni(movement.isMoving);
    }
    void CheckEnd()
    {
        if (fighting.hp <= 0)
        {
            processManager.GameOver();
            fighting.DelStunObj();
        }
    }

    [SerializeField] float rsDeadZone = 0.2f;
    [SerializeField] Text rsrsrs;
    bool flag;
    void KeyControll()
    {
        float rsX = Input.GetAxis("RightStickHorizontal");
        float rsY = Input.GetAxis("RightStickVertical");
        if (Mathf.Abs(rsX) > rsDeadZone || Mathf.Abs(rsY) > rsDeadZone)
        {
            Debug.Log("오른쪽 스틱이 움직였습니다!");
        }
        rsrsrs.text = $"X : {rsX} |  Y : {rsY}";
        rsX = Mathf.Abs(rsX) > rsDeadZone ? 1 : 0;
        rsY = Mathf.Abs(rsY) > rsDeadZone ? 1 : 0;
        print($"RS_X : {rsX} | RS_Y {rsY}");
        if (rsX == 0 && rsY == 0) flag = false;
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetButtonDown("LockOn")) // 락온 토글
        {
            print("락온켜짐");
            fighting.LockOn();
        }
        if ((Input.GetKeyDown(KeyCode.Tab) || rsX == 1 || rsY == 1) && flag == false) // 락온 변경
        {
            flag = true;
            print("변경!");
            fighting.ChangeLockOn();
        }
        if (Input.GetKeyDown(KeyCode.I) || Input.GetButtonDown("MeleeAttack")) // 근접
        {
            fighting.ExecutionGaugeFill();
            if (fighting.knifeAble && !fighting.executing)
            {
                aniController.KnifeHit();
                fighting.KnifeAttack();
            }
        }
        if (Input.GetKey(KeyCode.J) || Input.GetAxisRaw("RangeAttack") != 0) // 원거리 키 누른거
        {
            if (!fighting.getShootable || fighting.onReloading) return;
            if (fighting.getCurAmmo > 0)
            {
                movement.Slow();
                aniController.Fire();
            }
            fighting.Shoot();
        }
        if (Input.GetKeyUp(KeyCode.J) || Input.GetAxisRaw("RangeAttack") == 0) // 원거리 키 뗀거
        {
            movement.Slow(false);
        }
        if (Input.GetKeyDown(KeyCode.R) || Input.GetButtonDown("Reload")) // 재장전
        {
            if (fighting.onReloading || !fighting.CheckReloadable()) return;
            fighting.Reloading(fighting.getReloadTime);
            ui.ReloadSlider(fighting.getReloadTime);
        }
        if ((Input.GetKeyDown(KeyCode.L) || Input.GetButtonDown("Dodge")) && !fighting.isStunned) // 대쉬
        {
            movement.Dash();
        }
        // if (Input.GetKeyDown(KeyCode.LeftShift)) // 회전 락
        // {
        // movement.StayLocked();
        // }
    }
    bool isPad;
    void UIPadSet()
    {
        if (isPad) return;

        if (Input.GetButtonDown("Dodge") ||
        Input.GetButtonDown("Reload") ||
        Input.GetAxisRaw("RangeAttack") != 0 ||
        Input.GetButtonDown("MeleeAttack") ||
         Mathf.Abs(Input.GetAxis("RightStickHorizontal")) > 0.1f ||
         Mathf.Abs(Input.GetAxis("RightStickVertical")) > 0.1f ||
         Input.GetButtonDown("LockOn")
        )
        {
            isPad = true;
            ui.PadExecution(true);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == "EndPoint")
        {
            processManager.GameClear();
        }
    }
}
