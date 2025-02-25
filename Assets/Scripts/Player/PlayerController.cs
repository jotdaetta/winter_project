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
        movement.Turn(fighting.canAimToLockedEnemy, fighting.aimDir);
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
        // if (Input.GetKeyDown(KeyCode.T))
        // {
        //      fighting.Execute();
        // }
        if (Input.GetKeyDown(KeyCode.I))
        {
            fighting.KnifeAttack();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            fighting.ExecutionGaugeFill();
        }
        if (Input.GetKey(KeyCode.J))
        {
            if (!fighting.getShootable) return;
            if (fighting.getCurAmmo > 0)
            {
                movement.Slow();
                aniController.Fire();
            }
            fighting.Shoot();
        }
        if (Input.GetKeyUp(KeyCode.J))
        {
            movement.Slow(false);
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
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            movement.StayLocked();
        }
    }
    void Setting()
    {
        ui.SetAmmoText(fighting.getCurAmmo, fighting.getTotalAmmo);
        aniController.MoveAni(movement.isMoving);
    }
}
