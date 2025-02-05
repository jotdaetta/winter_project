using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerMovement movement;
    [SerializeField] PlayerFight fight;
    private void FixedUpdate()
    {
        movement.Move();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            fight.LockOn();
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            fight.ChangeLockOn();
        }
    }
}
