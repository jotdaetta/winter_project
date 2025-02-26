using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    [SerializeField] Animator animator;

    public void Move(bool parm)
    {
        animator.SetBool("move", parm);
    }
    public void Attack()
    {
        animator.SetTrigger("fire");
    }
}
