using UnityEngine;

public class EnemyContoller : MonoBehaviour, IDamageable
{
    [SerializeField] EnemyMovement movement;
    [SerializeField] EnemyFight fight;

    private void FixedUpdate()
    {
        movement.FollowPlayer();
    }

    public void TakeDamage(int damage)
    {
        fight.TakeDamage(damage);
    }
}
