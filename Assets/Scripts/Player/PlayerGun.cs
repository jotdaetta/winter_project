using UnityEngine;

public class PlayerGun : Gun
{
    public void Shoot()
    {
        if (getCurAmmo < 1) return;
        Attack(transform.right);
    }

    public void Reloading(float reloading_time)
    {
    }
}
