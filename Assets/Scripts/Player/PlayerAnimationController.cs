using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] Animator animator;
    public void MoveAni(bool state)
    {
        animator.SetBool("move", state);
    }
    public void Fire()
    {
        animator.SetTrigger("fire");
    }
    public void KnifeHit()
    {
        animator.SetTrigger("knife");
    }
    public void IsExecution(bool parm)
    {
        animator.SetBool("execution", parm);
    }
}
