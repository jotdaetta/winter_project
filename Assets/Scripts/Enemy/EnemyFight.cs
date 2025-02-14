using UnityEngine;

public class EnemyFight : MonoBehaviour
{
    [SerializeField] int hp = 10;
    public void TakeDamage(int damage)
    {
        hp -= damage;
        print("hp: " + hp);
    }
}
