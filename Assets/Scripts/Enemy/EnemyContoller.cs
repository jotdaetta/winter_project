using UnityEngine;

public class EnemyContoller : MonoBehaviour
{

    public EnemyAnimationController animations;
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
        animations.Move(movement.aniMoving);
        if (movement.inCombat && !fight.isStunned)
        {
            fight.OnCombat();
        }
    }

    public void SetCombat(bool parm)
    {
        movement.inCombat = parm;
    }
}
