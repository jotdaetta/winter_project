using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerMovement movement;
    private void FixedUpdate()
    {
        movement.Move();
    }
}
