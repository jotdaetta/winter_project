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
        movement.FollowPlayer();
        movement.TurnToPlayer();
    }
    void Update()
    {
        if (movement.getCombat)
        {
            fight.OnCombat();
        }
    }
}
