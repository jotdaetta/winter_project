using UnityEngine;

public class EnemyContoller : MonoBehaviour
{
    [SerializeField] EnemyMovement movement;
    [SerializeField] EnemyFight fight;
    private void FixedUpdate()
    {
        movement.FollowPlayer();
    }
}
