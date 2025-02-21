using UnityEngine;

public class PlayerGun : Gun
{
    public Vector2 shootDir = new();
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            Attack(shootDir);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }
}
