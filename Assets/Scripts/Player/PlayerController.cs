using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerMovement movement;
    [SerializeField] PlayerFight fighting;
    [SerializeField] PlayerGun gun;
    [SerializeField] PlayerUI ui;
    private void FixedUpdate()
    {
        SetAim();
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
        if (Input.GetKeyDown(KeyCode.L))
        {
            movement.Dash();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            fighting.ExecutionGaugeFill();
        }
    }
    void SetUI()
    {
        int[] gunInfo = gun.GetGunInfo();
        ui.SetAmmoText(gunInfo[1], gunInfo[2]);
    }
    void SetAim()
    {
        gun.shootDir = fighting.aimDir;
    }
}
