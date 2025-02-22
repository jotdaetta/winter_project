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
}
