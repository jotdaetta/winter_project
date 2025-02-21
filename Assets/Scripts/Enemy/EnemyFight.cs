using UnityEngine;

public class EnemyFight : MonoBehaviour, IDamageable
{
    public int hp = 10;
    public void TakeDamage(int damage)
    {
        hp -= damage;
        print("hp: " + hp);
    }
}
