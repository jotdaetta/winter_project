using UnityEngine;

public class EnemyContoller : MonoBehaviour
{
    [SerializeField] EnemyMovement movement;
    [SerializeField] EnemyFight fight;

    void Awake()
    {
        Transform playerTsf = GameObject.FindWithTag("Player").transform;
        movement.playerTransform = playerTsf;
        fight.playerTransform = playerTsf;
    }

    private void FixedUpdate()
    {
        if (!fight.isStunned)
        {
            movement.FollowPlayer();
            movement.TurnToPlayer();
        }
        else
        {
            movement.Stunned();
        }
    }
    void Update()
    {
        if (movement.getCombat && !fight.isStunned)
        {
            fight.OnCombat();
        }
    }
}
