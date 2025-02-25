using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKnife : Weapon
{
    [SerializeField] private Vector2 boxSize = new Vector2(1f, 0.5f); // 사각형 크기 조절 가능
    [SerializeField] private float attackDistance = 1f; // 공격 거리

    public void Attack()
    {
        Vector2 origin = (Vector2)transform.position + (Vector2)transform.right * attackDistance * 0.5f;
        RaycastHit2D hit = Physics2D.BoxCast(origin, boxSize, 0f, transform.right, attackDistance, detectLayer);

        if (hit.collider != null)
        {
            if (hit.transform.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                damageable.TakeDamage(weaponDamage, true);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 origin = (Vector2)transform.position + (Vector2)transform.right * attackDistance * 0.5f;
        Gizmos.DrawWireCube(origin, boxSize);
    }
}