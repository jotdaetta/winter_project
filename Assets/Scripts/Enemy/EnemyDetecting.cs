using UnityEngine;

public class EnemyDetecting : MonoBehaviour
{
    [SerializeField] EnemyMovement fight;

    void OnTriggerStay2D(Collider2D collision)
    {
        fight.ColliderStay(collision);
    }
}
