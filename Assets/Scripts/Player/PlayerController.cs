using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerMovement movement;
    [SerializeField] PlayerFight fighting;
    private void FixedUpdate()
    {
        movement.Move();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            fighting.LockOn();
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            fighting.ChangeLockOn();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            movement.Dash();
        }
    }
}
