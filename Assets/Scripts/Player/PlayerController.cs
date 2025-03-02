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
        }
    }

    void KeyControll()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl)) // 락온 토글
        {
            fighting.LockOn();
        }
        if (Input.GetKeyDown(KeyCode.Tab)) // 락온 변경
        {
            fighting.ChangeLockOn();
        }
        if (Input.GetKeyDown(KeyCode.I)) // 근접
        {
            if (fighting.knifeAble)
            {
                aniController.KnifeHit();
                fighting.KnifeAttack();
            }
        }
        if (Input.GetKeyDown(KeyCode.X)) // 처형 연타
        {
            fighting.ExecutionGaugeFill();
        }
        if (Input.GetKey(KeyCode.J)) // 원거리 키 누른거
        {
            if (!fighting.getShootable || fighting.onReloading) return;
            if (fighting.getCurAmmo > 0)
            {
                movement.Slow();
                aniController.Fire();
            }
            fighting.Shoot();
        }
        if (Input.GetKeyUp(KeyCode.J)) // 원거리 키 뗀거
        {
            movement.Slow(false);
        }
        if (Input.GetKeyDown(KeyCode.R)) // 재장전
        {
            if (fighting.onReloading || !fighting.CheckReloadable()) return;
            fighting.Reloading(fighting.getReloadTime);
            ui.ReloadSlider(fighting.getReloadTime);
        }
        if (Input.GetKeyDown(KeyCode.L) && !fighting.isStunned) // 대쉬
        {
            movement.Dash();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift)) // 회전 락
        {
            movement.StayLocked();
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
