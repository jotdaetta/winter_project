using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameProcessManager processManager;
    [SerializeField] PlayerAnimationController aniController;
    [SerializeField] PlayerMovement movement;
    [SerializeField] PlayerFight fighting;
    [SerializeField] PlayerUI ui;
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

    void KeyControll()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetButtonDown("LockOn")) // 락온 토글
        {
            fighting.LockOn();
        }
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetAxis("RightStickHorizontal") != 0 || Input.GetAxis("RightStickVertical") != 0) // 락온 변경
        {
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
        if (Input.GetKey(KeyCode.J) || Input.GetButton("RangeAttack")) // 원거리 키 누른거
        {
            if (!fighting.getShootable || fighting.onReloading) return;
            if (fighting.getCurAmmo > 0)
            {
                movement.Slow();
                aniController.Fire();
            }
            fighting.Shoot();
        }
        if (Input.GetKeyUp(KeyCode.J) || Input.GetButtonUp("RangeAttack")) // 원거리 키 뗀거
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

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == "EndPoint")
        {
            processManager.GameClear();
        }
    }
}
