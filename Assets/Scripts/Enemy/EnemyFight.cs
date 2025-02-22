using UnityEngine;

public class EnemyFight : Gun, IDamageable
{
    public int hp = 10;
    public void TakeDamage(int damage)
    {
        hp -= damage;
        print("hp: " + hp);
    }
}
