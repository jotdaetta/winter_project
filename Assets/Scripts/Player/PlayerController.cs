using UnityEngine;

public class PlayerController : MonoBehaviour
{
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
        SetUI();
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
        if (Input.GetKeyDown(KeyCode.J))
        {
            fighting.Shoot();
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
    void SetUI()
    {
        ui.SetAmmoText(fighting.getCurAmmo, fighting.getTotalAmmo);
        ui.SetHp(fighting.hp);
    }
}
