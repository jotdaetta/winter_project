using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerAnimationController aniController;
    [SerializeField] PlayerMovement movement;
    [SerializeField] PlayerFight fighting;
    [SerializeField] PlayerUI ui;
    private void FixedUpdate()
    {
        movement.Move();
        movement.Turn(fighting.lockedOn, fighting.aimDir);
    }
    private void Update()
    {
        Setting();
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            fighting.LockOn();
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            fighting.ChangeLockOn();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            fighting.Execute();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            fighting.ExecutionGaugeFill();
        }
        if (Input.GetKey(KeyCode.J))
        {
            if (!fighting.getShootable) return;
            movement.moveSpeedMul = movement.moveSlow;
            if (fighting.getCurAmmo > 0)
                aniController.Fire();
            fighting.Shoot();
        }
        if (Input.GetKeyUp(KeyCode.J))
        {
            movement.moveSpeedMul = 1;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (fighting.onReloading || !fighting.CheckReloadable()) return;
            fighting.Reloading(fighting.getReloadTime);
            ui.ReloadSlider(fighting.getReloadTime);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            movement.Dash();
        }
    }
    void Setting()
    {
        ui.SetAmmoText(fighting.getCurAmmo, fighting.getTotalAmmo);
        ui.SetHp(fighting.hp);

        aniController.MoveAni(movement.isMoving);
    }
}
